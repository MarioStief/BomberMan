using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class GM_World {

    // gamearea parameters
    public const int N_B = 30;
    public const int N_L = 20;

    public static Vector3 GRAVITY_VEC = new Vector3(0.0f, -10.0f, 0.0f);
    public static Vector3 TO_HELL = new Vector3(0.0f, -1000.0f,  0.0f);
    public const float TIMESTEP = 0.01f;
    public const float ACTOR_SPEED = 5.0f;
    public const float RESPAWN_TIME = 3.0f;

    public static float explosionRadius = 2.5f;
    public static float explosionPlaneSize = 0.2f;

    // entity types
    public const int ENT_ACTOR = 1;
    public const int ENT_BOMB = 2;
    public const int ENT_POWERUP = 3;

    public class Entity
    {
        public int type = 0;
        public int svid = 0;
        public int clid = 0;
        public int pid  = 0;
        public bool isActive = true;
        public bool isDead = false;
        public GameObject obj = null;
        public NetworkViewID viewID;

        public Rink.Pos rpos;

        // additional properties of entities,
        // not necessarily used by all types
        public struct Props
        {
            // ENT_BOMB
            public int flamePower;

            // ENT_POWERUP
            public PowerupType puType;
        }
        public Props props = new Props();

        public Vector3 lastPosition = Vector3.zero;
    }

    protected LinkedList<Entity> entities = new LinkedList<Entity>();

    protected Entity ByPID(int pid)
    {
        foreach (Entity entity in entities)
            if (entity.pid == pid) return entity;
        return null;
    }

    protected Entity BySVID(int svid)
    {
        foreach (Entity entity in entities)
            if (entity.svid == svid) return entity;
        return null;
    }
}
