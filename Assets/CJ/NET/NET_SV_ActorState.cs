using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class NET_SV_ActorState : MonoBehaviour {

    NET_ActorState.Message msg = null;

    public NET_ActorState.Message GetInput()
    {
        return msg;
    }

    public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        msg = new NET_ActorState.Message();
        Rink.Pos.Serialize(ref msg.rpos, stream);
    }

}
