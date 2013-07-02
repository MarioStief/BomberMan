using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class NET_SV_Movable : MonoBehaviour {

    public Rink.Pos rpos = new Rink.Pos();
    public int resId = 0; // for owning player only
    public float time = 0.0f;

    public void SetResponseID(int resId)
    {
        this.resId = resId;
    }

    public void SetServerTime(float time)
    {
        this.time = time;
    }

    public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        stream.Serialize(ref resId);
        stream.Serialize(ref time);
        stream.Serialize(ref rpos.bpos);
        stream.Serialize(ref rpos.lpos);
        stream.Serialize(ref rpos.xoff);
        stream.Serialize(ref rpos.yoff);
    }
	
}
