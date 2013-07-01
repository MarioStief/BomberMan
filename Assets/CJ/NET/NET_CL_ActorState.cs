using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class NET_CL_ActorState : MonoBehaviour {

    private NET_ActorState.Message msg = null;

    public void AddState(Rink.Pos rpos) 
    {
        msg = new NET_ActorState.Message();
        msg.rpos = rpos;
    }

    public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        if (null != msg)
        {
            Rink.Pos.Serialize(ref msg.rpos, stream);
            msg = null;
        }
    }

}
