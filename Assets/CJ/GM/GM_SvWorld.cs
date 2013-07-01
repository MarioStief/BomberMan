using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using AssemblyCSharp;

public class GM_SvWorld : GM_World {

    private class SvActorEntity : Entity
    {
        public Player player = null;
        public NET_SV_Movable scr_moveable = null;
        public InputHandler scr_inputHandler = null;
    }

    private class SvBombEntity : Entity
    {
        private const float BOMB_TIMEOUT = 3.0f;

        public GM_GA_Cell cell = null;
        public float spawnTime = 0.0f;

        public bool IsDead(float time)
        {
            return (time - spawnTime) > BOMB_TIMEOUT;
        }
    }

    private GM_GameArea gameArea = null;

    private Rink rink = null;

    private NET_Server scr_netServer = null;

    private int svidCnt = 0;

    float time = 0.0f;
    float accu = 0.0f;

    public void Genesis(NET_Server src_netServer)
    {
        this.scr_netServer = src_netServer;

        GameObject obj_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        obj_mainCamera.AddComponent<GM_SpectatorCam>();

        int seed = (int)(Random.value * 1000.0f);
        Random.seed = seed;

        rink = new Rink(N_B, N_L, Vector2.zero);

        NET_MSG_GenerateArea genAreaMsg = new NET_MSG_GenerateArea();
        genAreaMsg.seed = seed;
        scr_netServer.Broadcast(genAreaMsg);

        foreach(NET_Server.Client client in scr_netServer.Clients()) {
            client.isDead = false;

            int lpos = N_L / 2 - 1;
            int bpos = N_B / 4;

            Spawn(ENT_ACTOR, client.pid, bpos, lpos);
        }
    }

    public Entity Spawn(int type, int pid, int bpos, int lpos)
    {
        Entity entity = null;

        if (ENT_ACTOR == type)
        {
            SvActorEntity entActor = new SvActorEntity();

            entActor.obj = new GameObject();
            entActor.obj.tag = "ENT_ACTOR";

            entActor.player = new Player();
            entActor.scr_moveable = entActor.obj.AddComponent<NET_SV_Movable>();
            // entActor.scr_inputHandler = entActor.obj.AddComponent<InputHandler>();
            
            Parcel currCell = Static.rink.gameArea[lpos][bpos];
            entActor.player.setCurrentParcel(currCell);

            entActor.viewID = Network.AllocateViewID();
            NetworkView netView = entActor.obj.AddComponent<NetworkView>();
            netView.viewID = entActor.viewID;
            netView.observed = entActor.scr_moveable;
            netView.stateSynchronization = NetworkStateSynchronization.Unreliable;

            NET_Server.Client client = scr_netServer.ClientByPID(pid);
            netView.SetScope(client.netPlayer, false);

            entity = entActor;
        }
        if (ENT_BOMB == type)
        {
            SvBombEntity entBomb = new SvBombEntity();

            entBomb.obj = (GameObject)GameObject.Instantiate(Resources.Load("Bomb"));

            entity = entBomb;
        }

        entity.type = type;
        entity.svid = ++svidCnt;
        entity.pid = pid;

        GM_SvidTag scr_svidTag = entity.obj.AddComponent<GM_SvidTag>();
        scr_svidTag.svid = entity.svid;

        entities.AddLast(entity);

        NET_MSG_SpawnEntity spawnEntityMsg = new NET_MSG_SpawnEntity();
        spawnEntityMsg.type = type;
        spawnEntityMsg.svid = entity.svid;
        spawnEntityMsg.pid = entity.pid;
        spawnEntityMsg.viewID = entity.viewID;
        spawnEntityMsg.bpos = bpos;
        spawnEntityMsg.lpos = lpos;
        scr_netServer.Broadcast(spawnEntityMsg);

        return entity;
    }

    public void DestroyEntity(int svid)
    {
        Entity entity = BySVID(svid);
        GameObject.Destroy(entity.obj);

        entity.isDead = true;

        NET_MSG_DestroyEntity destroyEntityMsg = new NET_MSG_DestroyEntity();
        destroyEntityMsg.svid = entity.svid;
        scr_netServer.Broadcast(destroyEntityMsg);
    }

    public void DestroyCell(int row, int col)
    {
        gameArea.getCell(row, col).setType(0);

        NET_MSG_DestroyCell destroyCellMsg = new NET_MSG_DestroyCell();
        destroyCellMsg.row = row;
        destroyCellMsg.col = col;
        scr_netServer.Broadcast(destroyCellMsg);
    }

    public void SetActive(int svid, bool active)
    {
        Entity entity = BySVID(svid);

        if (entity.isActive == active)
            return;

        entity.isActive = active;

        if (!active)
        {
            entity.lastPosition = entity.obj.transform.position;
            entity.obj.transform.position = TO_HELL;
        }
        else entity.obj.transform.position = entity.lastPosition;

        NET_MSG_EntitySetActive setActiveMsg = new NET_MSG_EntitySetActive();
        setActiveMsg.svid = svid;
        setActiveMsg.active = active;
        scr_netServer.Broadcast(setActiveMsg);
    }

    public Entity[] ShootDeathRays(Vector3 orig, Vector3 dir)
    {
        float size = explosionPlaneSize;

        Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);

        Vector3 perp = Vector3.Normalize(Vector3.Cross(dir, up));

        Vector3[] origs =
        {
            orig,
            orig + size * perp + size * up,
            orig - size * perp + size * up,
            orig + size * perp - size * up,
            orig - size * perp - size * up
        };
        int numRays = origs.Length;

        Entity[] hitEntities = new Entity[numRays];
        int numHits = 0;

        for (int i = 0; i < numRays; ++i) hitEntities[i] = null;

        for (int i = 0; i < numRays; ++i)
        {
            Ray ray = new Ray(origs[i], dir);

            Debug.DrawLine(ray.origin, ray.origin + ray.direction * explosionRadius, Color.red, 5.0f);

            RaycastHit info;
            if (Physics.Raycast(ray, out info, explosionRadius))
            {
                GameObject obj = info.collider.gameObject;
                if ("ENT_ACTOR" == obj.tag)
                {
                    hitEntities[numHits++] = BySVID(obj.GetComponent<GM_SvidTag>().svid);
                }
            }
        }

        return hitEntities;
    }

    public void HandleMessage(NET_Message msg)
    {
        if (NET_Message.MSG_PLANT_BOMB == msg.GetMsgID())
        {
            /*
            Debug.Log("GM_SvWorld::HandleMessage: received msg of type MSG_PLANT_BOMB");

            NET_MSG_PlantBomb plantBombMsg = (NET_MSG_PlantBomb)msg;
            Entity entity = ByPID(plantBombMsg.pid);
            GM_GA_Cell cell = gameArea.getCell(entity.obj.transform.position.x, entity.obj.transform.position.z);

            entity.charCtrl.Move(new Vector3(0.0f, 1.5f, 0.0f));

            Vector3 position = gameArea.CenterOf(cell);
            position.y = -0.2f; // accounts for playfield height and bomb size

            SvBombEntity entBomb = (SvBombEntity)Spawn(ENT_BOMB, plantBombMsg.pid, position);
            entBomb.cell = cell;
            entBomb.spawnTime = plantBombMsg.time;
            */
        }
    }

    public void Update()
    {
        time += Time.deltaTime;

        // BURRY THE DEAD

        LinkedListNode<Entity> entIt = entities.First;
        while (null != entIt)
        {
            LinkedListNode<Entity> next = entIt.Next;
            if (entIt.Value.isDead) entities.Remove(entIt);
            entIt = next;
        }

        foreach (NET_Server.Client client in scr_netServer.Clients())
        {
            SvActorEntity entity = (SvActorEntity)ByPID(client.pid);
            if (null != entity)
            {
                entity.scr_moveable.SetServerTime(time);

                NET_SV_ActorState state = client.scr_actorState;
                NET_ActorState.Message msg = state.GetInput();
                if (null != msg)
                {
                    entity.scr_moveable.rpos = msg.rpos;
                }
            }

            if (client.isDead)
            {
                client.time += Time.deltaTime;
                if (RESPAWN_TIME <= client.time)
                {
                    Debug.Log("GM_SvWorld: respawning player with pid = " + client.pid);

                    client.isDead = false;
                    SetActive(entity.svid, true);
                }
            }
        }

        /*
        foreach (SvEntity entity in entities)
        {
            entity.obj.transform.position = new Vector3(
                entity.scr_svEntity.position.x,
                entity.obj.transform.position.y,
                entity.scr_svEntity.position.y);
        }
        */
    }
}
