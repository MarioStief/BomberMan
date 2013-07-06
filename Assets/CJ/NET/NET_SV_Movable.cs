using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class NET_SV_Movable : MonoBehaviour {

    public NET_ActorState.Message actorState = null;
    public float time = 0.0f;

    public void SetServerTime(float time)
    {
        this.time = time;
    }

    public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        if (null != actorState)
        {
            stream.Serialize(ref time);
            NET_ActorState.Message.Serialize(stream, actorState);
        }
        
        
    }
	
}
