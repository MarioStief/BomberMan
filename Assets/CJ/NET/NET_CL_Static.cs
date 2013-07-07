using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class NET_CL_Static : NET_CL_Entity {

    Rink.Pos rpos = new Rink.Pos();

    public void SetPosition(Rink.Pos rpos)
    {
        this.rpos = rpos;
    }

    public override Vector3 GetPosition(InputHandler inputHandler)
    {
        return Static.rink.GetPosition(rpos);
    }
	
}
