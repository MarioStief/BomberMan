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
        public NET_CL_Entity scr_clEntity = null;       // remote actors
        public InputHandler scr_inputHandler = null;    // local actor

        public GameObject obj_ghost = null;
    }

    private int clid = 0;

    private GM_GameArea gameArea = null;

    private SphereBuilder scr_sphereBuilder = null;

    private Player localPlayer = new Player();
    private ClEntity localActor = null;

    public void Genesis()
    {
        GameObject obj_gameController = (GameObject)GameObject.FindGameObjectWithTag("GameController");
        scr_netClient = obj_gameController.GetComponent<NET_Client>();
        scr_localInput = scr_netClient.GetLocalInput();
    }

    public void ToggleGhosts()
    {
        foreach(ClEntity entity in entities) 
            entity.obj_ghost.renderer.enabled = !entity.obj_ghost.renderer.enabled;
    }

    private Object PowerupPrefabFromType(PowerupType puType)
    {
        Object powerup = null;
        /*
        if (puType == PowerupType.BOMB_UP)
            powerup = Static.bombUpPowerupPrefab;
        else if (puType == PowerupType.BOMB_DOWN)
            powerup = Static.bombDownPowerupPrefab;
        else if (puType == PowerupType.FLAME_UP)
            powerup = Static.flameUpPowerupPrefab;
        else if (puType == PowerupType.FLAME_DOWN)
            powerup = Static.flameDownPowerupPrefab;
        else if (puType == PowerupType.PLAYER_SPEED_UP)
            powerup = Static.playerSpeedUpPowerupPrefab;
        else if (puType == PowerupType.PLAYER_SPEED_DOWN)
            powerup = Static.playerSpeedDownPowerupPrefab;
        else if (puType == PowerupType.DELAY_SPEED_UP)
            powerup = Static.playerSpeedUpPowerupPrefab;
        else if (puType == PowerupType.DELAY_SPEED_DOWN)
            powerup = Static.playerSpeedDownPowerupPrefab;
        else if (puType == PowerupType.GOLDEN_FLAME)
            powerup = Static.goldenFlamePowerupPrefab;
        else if (puType == PowerupType.SUPERBOMB)
            powerup = Static.superbombPowerupPrefab;
        if (null == powerup) Debug.Log("GM_ClWorld: warning, invalid powerup mesh!");
         */

        // cj: okay, use the same mesh for testing purposes...
        powerup = Static.powerupPrefab;

        return powerup;
    }

    public void HandleMessage(NET_Message msg)
    {
        if (NET_Message.MSG_SPAWN_ENTITY == msg.GetMsgID())
        {
            NET_MSG_SpawnEntity spawnEntityMsg = (NET_MSG_SpawnEntity)msg;

            Rink.Pos rpos = new Rink.Pos(spawnEntityMsg.bpos, spawnEntityMsg.lpos, 0, 0);

            ClEntity entity = null;
            NetworkView netView = null;

            entity = new ClEntity();
            entity.type = spawnEntityMsg.type;
            entity.svid = spawnEntityMsg.svid;
            entity.clid = ++clid;
            entity.pid = spawnEntityMsg.pid;
            entity.viewID = spawnEntityMsg.viewID;

            entity.rpos = rpos;

            entity.props = spawnEntityMsg.props;

            if (ENT_ACTOR == spawnEntityMsg.type)
            {
                entity.obj = (GameObject)GameObject.Instantiate(Resources.Load("Actor"));
                entity.obj_ghost = (GameObject)GameObject.Instantiate(Resources.Load("GhostActor"));
                entity.obj_ghost.GetComponent<MeshRenderer>().enabled = false;

                if (spawnEntityMsg.pid == scr_netClient.GetLocalPID())
                {
                    entity.scr_inputHandler = entity.obj.AddComponent<InputHandler>();
                    entity.scr_inputHandler.SetPlayer(localPlayer);
                    entity.scr_inputHandler.SetPosition(rpos);
                    entity.obj.transform.position = Static.rink.GetPosition(rpos);

                    localActor = entity;
                    entity.obj.tag = "Player";
                }
                else
                {
                    entity.scr_clEntity = entity.obj.AddComponent<NET_CL_Moveable>();
                    entity.scr_clEntity.SetPosition(Static.rink.GetPosition(rpos));
                }

                netView = entity.obj.AddComponent<NetworkView>();
                netView.viewID = entity.viewID;
                netView.observed = entity.scr_clEntity;
                netView.stateSynchronization = NetworkStateSynchronization.Unreliable;
            }
            if (ENT_BOMB == spawnEntityMsg.type)
            {
                entity.obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/bombPrefab"));

                entity.scr_clEntity = entity.obj.AddComponent<NET_CL_Static>();
                entity.scr_clEntity.SetPosition(rpos);
            }
            if (ENT_POWERUP == spawnEntityMsg.type)
            {
                entity.obj = (GameObject)GameObject.Instantiate(PowerupPrefabFromType(spawnEntityMsg.props.puType));
                entity.scr_clEntity = entity.obj.AddComponent<NET_CL_Static>();
                entity.scr_clEntity.SetPosition(rpos);
            }

            entities.AddLast(entity);
        }

        if (NET_Message.MSG_DESTROY_ENTITY == msg.GetMsgID())
        {
            NET_MSG_DestroyEntity destroyEntityMsg = (NET_MSG_DestroyEntity)msg;

            Entity entity = BySVID(destroyEntityMsg.svid);

            if (ENT_BOMB == entity.type)
            {
                Parcel cell = Static.rink.GetCell(entity.rpos);
				int type = 0;
				if (Static.player.getSuperbomb())
					type = 1;
				if (Static.player.getTriggerbomb())
					type = 2;
				if (Static.player.getSuperbomb() && Static.player.getTriggerbomb())
					type = 3;

                Explosion.createExplosionOnCell(cell, entity.props.flamePower, 0.2f, type, false);
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
                if (entity.scr_clEntity)
                {
                    entity.obj.transform.position = entity.scr_clEntity.GetPosition();

                    if (entity.obj_ghost)
                    {
                        entity.obj_ghost.transform.position = entity.scr_clEntity.GetServerPosition();
                    }
                }
            }

            entIt = next;
        }

        scr_netClient.GetLocalState().AddState(localPlayer.GetPosition());
    }
	
}
