using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class NET_SV_ActorState : MonoBehaviour {

    List<NET_ActorState.Message> buffer = new List<NET_ActorState.Message>();

    public List<NET_ActorState.Message> GetInput()
    {
        List<NET_ActorState.Message> ret = new List<NET_ActorState.Message>(buffer);
        buffer.Clear();
        return ret;
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
        Common.Assert(0 < numMsgs, "NET_SV_ActorState: 0 < numMsgs");
        for (int i = 0; i < numMsgs; ++i)
        {
            NET_ActorState.Message msg = new NET_ActorState.Message();
            NET_ActorState.Message.Serialize(stream, msg);
            buffer.Add(msg);
        }
        buffer.Sort(new CompareTime());
    }

}
