using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class NET_CL_Static : NET_CL_Entity {

    private Rink.Pos rpos = new Rink.Pos();

    public override void SetPosition(Rink.Pos rpos)
    {
        this.rpos = rpos;
    }

    public override void SetPosition(Vector3 position)
    {
        // deprecated
    }

    public override Vector3 GetPosition()
    {
        return Static.rink.GetPosition(rpos);
    }

    public override Vector3 GetServerPosition()
    {
        return Static.rink.GetPosition(rpos);
    }
	
}
