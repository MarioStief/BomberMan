using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class NET_SV_ActorState : MonoBehaviour {

    int maxReqID = 0; // most recently received package
    NET_ActorState.Message msg = null;

    public NET_ActorState.Message GetInput()
    {
        return msg;
    }

    public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        NET_ActorState.Message msg = new NET_ActorState.Message();
        NET_ActorState.Message.Serialize(stream, msg);
        if (maxReqID < msg.reqID)
        {
            maxReqID = msg.reqID;
            this.msg = msg;
        }
    }

}
