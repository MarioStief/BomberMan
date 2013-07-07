using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class NET_CL_Moveable : NET_CL_Entity {

    public static float IP_LAG = 0.2f; // 200ms

    private List<NET_ActorState.Message> buffer = new List<NET_ActorState.Message>();

    private NET_ActorState.Message state = new NET_ActorState.Message();
    private NET_ActorState.Message serverState = new NET_ActorState.Message();

    private static NET_ActorState.Message Lerp(NET_ActorState.Message lhp, NET_ActorState.Message rhp, float t)
    {
        NET_ActorState.Message state = new NET_ActorState.Message();
        state.position = Vector3.Lerp(lhp.position, rhp.position, t);
        state.vertAng = (1.0f - t) * lhp.vertAng + t * rhp.vertAng;
        state.horzAng = (1.0f - t) * lhp.horzAng + t * rhp.horzAng;
        return state;
    }

    public override void SetPosition(Vector3 position)
    {
        state.position = position;
    }

    public override Vector3 GetPosition()
    {
        return state.position;
    }

    public override float GetVerticalAngle()
    {
        return state.vertAng;
    }

    public override float GetHorizontalAngle()
    {
        return state.horzAng;
    }

    private static float Ease(float t) 
    {
        return 0.5f * (Mathf.Sin(t * Mathf.PI - 0.5f * Mathf.PI) + 1.0f);
    }

    public void Update()
    {
        float dtime = NET_Client.GetTime() - IP_LAG;

        int i = buffer.Count - 1;
        for (; 0 <= i && buffer[i].time > dtime; --i) ;
        if (0 <= i)
        {
            if (i + 1 < buffer.Count)
            {
                float t = (dtime - buffer[i].time) / (buffer[i + 1].time - buffer[i].time);
                state = Lerp(buffer[i], buffer[i + 1], t);
            }
            if(0 < i) buffer.RemoveRange(0, i - 1);
        }
    }

    private class CompareTime : IComparer<NET_ActorState.Message>
    {
        public int Compare(NET_ActorState.Message lhp, NET_ActorState.Message rhp)
        {
            return System.Math.Sign(lhp.time - rhp.time);
        }
    }

    public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        int numMsgs = -1;
        stream.Serialize(ref numMsgs);
        Common.Assert(0 < numMsgs, "NET_CL_Moveable: 0 < numMsgs");
        for (int i = 0; i < numMsgs; ++i)
        {
            NET_ActorState.Message msg = new NET_ActorState.Message();
            NET_ActorState.Message.Serialize(stream, msg);
            buffer.Add(msg);
        }
        buffer.Sort(new CompareTime());
    }
	
}
