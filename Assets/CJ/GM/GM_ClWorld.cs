using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class GM_ClWorld : GM_World {

    private NET_Client scr_netClient = null;
    private NET_CL_Input scr_localInput = null;

    private GM_ActorCam scr_actorCam = null;

    private class ClEntity : Entity
    {
        public NET_CL_Entity scr_clEntity = null; // is always null for local actor
    }

    private int clid = 0;

    private GM_GameArea gameArea = null;

    private SphereBuilder scr_sphereBuilder = null;

    private Player localPlayer = new Player();
    private ClEntity localActor = null;
    private InputHandler localInputHandler = null;

    public void Genesis()
    {
        GameObject obj_gameController = (GameObject)GameObject.FindGameObjectWithTag("GameController");
        scr_netClient = obj_gameController.GetComponent<NET_Client>();
        scr_localInput = scr_netClient.GetLocalInput();

        obj_gameController.GetComponent<Cameras>().SetCamera(Cameras.CAM_TYPE.GAME);
    }

    // REMOVEME
    public void ToggleGhosts()
    {
    }

    public void HandleMessage(NET_Message msg)
    {
        if (NET_Message.MSG_SPAWN_ENTITY == msg.GetMsgID())
        {
            NET_MSG_SpawnEntity spawnEntityMsg = (NET_MSG_SpawnEntity)msg;

            Rink.Pos rpos = new Rink.Pos(spawnEntityMsg.bpos, spawnEntityMsg.lpos, 0, 0);

            ClEntity entity = new ClEntity();
            NetworkView netView = null;

            if (ENT_ACTOR == spawnEntityMsg.type)
            {
                entity.obj = (GameObject)GameObject.Instantiate(Resources.Load("Actor"));

                if (spawnEntityMsg.pid == scr_netClient.GetLocalPID()) // spawn local actor
                {
                    localInputHandler = entity.obj.AddComponent<InputHandler>();
                    localInputHandler.SetPlayer(localPlayer);
                    localInputHandler.SetPosition(rpos);
                    entity.obj.transform.position = Static.rink.GetPosition(rpos);

                    localActor = entity;
                    entity.obj.tag = "Player";
                }
                else // spawn remote actor
                {
                    NET_CL_Moveable scr_clMoveable = entity.obj.AddComponent<NET_CL_Moveable>();
                    scr_clMoveable.SetPosition(Static.rink.GetPosition(rpos));
                    entity.scr_clEntity = scr_clMoveable;
                }

                netView = entity.obj.AddComponent<NetworkView>();
                netView.viewID = spawnEntityMsg.viewID;
                netView.observed = entity.scr_clEntity;
                netView.stateSynchronization = NetworkStateSynchronization.Unreliable;
            }
            if (ENT_BOMB == spawnEntityMsg.type)
            {
                entity.obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/bombPrefab"));
                NET_CL_Static scr_clStatic = entity.obj.AddComponent<NET_CL_Static>();
                scr_clStatic.SetPosition(rpos);
                entity.scr_clEntity = scr_clStatic;
                // cell.setObject(entity.obj) would orphan a powerup on this cell.
                // so don't do this right now
            }
            if (ENT_POWERUP == spawnEntityMsg.type)
            {
                entity.obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/powerupPrefab"));
                PowerupTexture scr_powerupTex = entity.obj.GetComponent<PowerupTexture>();
                scr_powerupTex.setType(spawnEntityMsg.props.puType);
                Parcel cell = Static.rink.GetCell(rpos);
                cell.setGameObject(entity.obj);
                cell.getMeshManipulator().liftObject(1.05f); // ms: Sonst ist das Powerup halb im Boden
            }

            entity.type = spawnEntityMsg.type;
            entity.svid = spawnEntityMsg.svid;
            entity.clid = ++clid;
            entity.pid = spawnEntityMsg.pid;
            entity.viewID = spawnEntityMsg.viewID;

            entity.rpos = rpos;

            entity.props = spawnEntityMsg.props;

            entities.AddLast(entity);
        }

        if (NET_Message.MSG_DESTROY_ENTITY == msg.GetMsgID())
        {
            NET_MSG_DestroyEntity destroyEntityMsg = (NET_MSG_DestroyEntity)msg;

            Entity entity = BySVID(destroyEntityMsg.svid);

            if (ENT_BOMB == entity.type)
            {
                Parcel cell = Static.rink.GetCell(entity.rpos);
				int extra = 0;
				if (Static.player.getTriggerbomb())
					extra = 1;
				if (Static.player.getContactMine())
					extra = 2;

                Explosion.createExplosionOnCell(cell, entity.props.flamePower, 0.2f, entity.props.isSuperbomb, extra, false);
            }

            GameObject.Destroy(entity.obj);
            entity.isDead = true;
        }

        if (NET_Message.MSG_DESTROY_CELL == msg.GetMsgID())
        {
            NET_MSG_DestroyCell destroyCellMsg = (NET_MSG_DestroyCell)msg;
            gameArea.getCell(destroyCellMsg.row, destroyCellMsg.col).setType(0);
        }

        if (NET_Message.MSG_ENTITY_SET_ACTIVE == msg.GetMsgID())
        {
            NET_MSG_EntitySetActive setActiveMsg = (NET_MSG_EntitySetActive)msg;

            Entity entity = BySVID(setActiveMsg.svid);
            bool active = setActiveMsg.active;

            if (entity.isActive == active)
                return;

            entity.isActive = active;

            if (!active)
            {
                entity.lastPosition = entity.obj.transform.position;
                entity.obj.transform.position = TO_HELL;

                if (entity.svid == localActor.svid) scr_actorCam.SetIdle();
            }
            else
            {
                entity.obj.transform.position = entity.lastPosition;

                if (entity.svid == localActor.svid) scr_actorCam.Init(localActor.obj);
            }
        }

        if (NET_Message.MSG_GENERATE_AREA == msg.GetMsgID())
        {
            NET_MSG_GenerateArea genAreaMsg = (NET_MSG_GenerateArea)msg;
            Random.seed = genAreaMsg.seed;

            GameObject obj_sphereBuilder = new GameObject("SphereBuilder");
            scr_sphereBuilder = obj_sphereBuilder.AddComponent<SphereBuilder>();
            scr_sphereBuilder.SetSize(N_B, N_L);
            scr_sphereBuilder.Init();
        }
    }

    public void Update()
    {
        LinkedListNode<Entity> entIt = entities.First;
        while (null != entIt)
        {
            LinkedListNode<Entity> next = entIt.Next;

            ClEntity entity = (ClEntity)entIt.Value;
            if (entity.isDead) entities.Remove(entIt);
            else
            {
                if (null != entity.scr_clEntity) 
                    entity.obj.transform.position = entity.scr_clEntity.GetPosition(localInputHandler);
            }

            entIt = next;
        }

        scr_netClient.GetLocalState().AddState(
            localActor.obj.transform.position, 
            localInputHandler.GetVerticalAngle(), 
            localInputHandler.GetHorizontalAngle());
    }
	
}
