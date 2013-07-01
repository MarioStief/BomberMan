using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public abstract class NET_Message
{
    public const int MSG_STARTGAME  = 10;
    public const int MSG_SPAWN_ENTITY = 20;
    public const int MSG_DESTROY_ENTITY = 21;
    public const int MSG_PLANT_BOMB = 22;
    public const int MSG_DESTROY_CELL = 23;
    public const int MSG_ENTITY_SET_ACTIVE = 24;
    public const int MSG_GENERATE_AREA = 30;

    public static string IDToString(int msgID)
    {
        switch(msgID) {
            case MSG_STARTGAME: return "MSG_STARTGAME";
            case MSG_SPAWN_ENTITY: return "MSG_SPAWNENTITY";
            case MSG_DESTROY_ENTITY: return "MSG_DESTROY_ENTITY";
            case MSG_PLANT_BOMB: return "MSG_PLANT_BOMB";
            case MSG_DESTROY_CELL: return "MSG_DESTROY_CELL";
            case MSG_ENTITY_SET_ACTIVE: return "MSG_ENTITY_SET_ACTIVE";
            case MSG_GENERATE_AREA: return "MSG_GENERATE_AREA";
        }
        return "<unkown msg type>";
    }

    private static int reqCnt = 0;

    private int req;
    private int msgID;

    public bool resend;

    public NET_Message(int msgID)
    {
        this.req = reqCnt++;
        this.msgID = msgID;
    }

    public int GetMsgID()
    {
        return msgID;
    }

    public void Serialize(BitStream stream)
    {
        stream.Serialize(ref req);
        DoSerialize(stream);
    }

    protected abstract void DoSerialize(BitStream stream);

    public static NET_Message CreateFromID(int msgID)
    {
        switch (msgID)
        {
            case MSG_STARTGAME: return new NET_MSG_StartGame();
            case MSG_SPAWN_ENTITY: return new NET_MSG_SpawnEntity();
            case MSG_DESTROY_ENTITY: return new NET_MSG_DestroyEntity();
            case MSG_PLANT_BOMB: return new NET_MSG_PlantBomb();
            case MSG_DESTROY_CELL: return new NET_MSG_DestroyCell();
            case MSG_ENTITY_SET_ACTIVE: return new NET_MSG_EntitySetActive();
            case MSG_GENERATE_AREA: return new NET_MSG_GenerateArea();
        }

        return null;
    }
}

public class NET_MSG_StartGame : NET_Message
{
    public NET_MSG_StartGame() : base(MSG_STARTGAME)
    {
    }

    protected override void DoSerialize(BitStream stream)
    {
        // nothing to do here...
    }
}

public class NET_MSG_SpawnEntity : NET_Message
{
    public int type;
    public int svid;
    public int pid;
    public NetworkViewID viewID;
    public int bpos, lpos;
    public GM_World.Entity.Props props = new GM_World.Entity.Props();

    public NET_MSG_SpawnEntity() : base(MSG_SPAWN_ENTITY)
    {
    }

    protected override void DoSerialize(BitStream stream)
    {
        stream.Serialize(ref type);
        stream.Serialize(ref svid);
        stream.Serialize(ref pid);
        stream.Serialize(ref viewID);
        stream.Serialize(ref bpos);
        stream.Serialize(ref lpos);

        if (GM_World.ENT_BOMB == type) stream.Serialize(ref props.flamePower);
    }
}

public class NET_MSG_DestroyEntity : NET_Message
{
    public int svid;

    public NET_MSG_DestroyEntity() : base(MSG_DESTROY_ENTITY)
    {
    }

    protected override void DoSerialize(BitStream stream)
    {
        stream.Serialize(ref svid);
    }
}

public class NET_MSG_PlantBomb : NET_Message
{
    public int pid;
    public float time;
    public Rink.Pos rpos;

    public NET_MSG_PlantBomb() : base(MSG_PLANT_BOMB)
    {
    }

    protected override void DoSerialize(BitStream stream)
    {
        stream.Serialize(ref pid);
        stream.Serialize(ref time);
        stream.Serialize(ref rpos.bpos);
        stream.Serialize(ref rpos.lpos);
    }
}

public class NET_MSG_DestroyCell : NET_Message
{
    public int row, col;

    public NET_MSG_DestroyCell() : base(MSG_DESTROY_CELL)
    {
    }

    protected override void DoSerialize(BitStream stream)
    {
        stream.Serialize(ref row);
        stream.Serialize(ref col);
    }
}

public class NET_MSG_EntitySetActive : NET_Message
{
    public int svid;
    public bool active;

    public NET_MSG_EntitySetActive()
        : base(MSG_ENTITY_SET_ACTIVE)
    {
    }

    protected override void DoSerialize(BitStream stream)
    {
        stream.Serialize(ref svid);
        stream.Serialize(ref active);
    }

}

public class NET_MSG_GenerateArea : NET_Message
{
    public int seed;

    public NET_MSG_GenerateArea() : base(MSG_GENERATE_AREA)
    {
    }

    protected override void DoSerialize(BitStream stream)
    {
        stream.Serialize(ref seed);
    }
}