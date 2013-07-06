using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class NET_CL_ActorState : MonoBehaviour {

    private NET_ActorState.Message msg = null;

    public void AddState(Vector3 position, float vertAng, float horzAng) 
    {
        msg = new NET_ActorState.Message();
        msg.position = position;
        msg.vertAng = vertAng;
        msg.horzAng = horzAng;
        msg.AssignID();
    }

    public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        if (null != msg)
        {
            NET_ActorState.Message.Serialize(stream, msg);
            msg = null;
        }
    }

}
