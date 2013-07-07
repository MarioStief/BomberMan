using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class NET_CL_ActorState : MonoBehaviour {

    // the idea of sending a sequence of states (instead of sending only the most recent state)
    // is to provide other clients with more states for interpolation, thus reducing jitterish
    // movement

    private List<NET_ActorState.Message> buffer = new List<NET_ActorState.Message>();

    public void AddState(Vector3 position, float vertAng, float horzAng) 
    {
        NET_ActorState.Message msg = new NET_ActorState.Message();
        msg.time = NET_Client.GetTime();
        msg.position = position;
        msg.vertAng = vertAng;
        msg.horzAng = horzAng;
        buffer.Add(msg);
    }

    public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        int numMsgs = buffer.Count;
        if (0 < numMsgs)
        {
            stream.Serialize(ref numMsgs);
            foreach(NET_ActorState.Message msg in buffer) 
            {
                NET_ActorState.Message.Serialize(stream, msg);
            }
            buffer.Clear();
        }
    }

}
