using UnityEngine;
using System.Collections;

public class NET_CL_Static : NET_CL_Entity {

    private Vector3 position = Vector3.zero;

    public override void SetPosition(Vector3 position)
    {
        this.position = position;
    }

    public override Vector3 GetPosition()
    {
        return position;
    }

    public override Vector3 GetServerPosition()
    {
        return position;
    }
	
}
