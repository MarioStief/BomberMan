using UnityEngine;
using System.Collections;

public class GM_ActorCam : MonoBehaviour {

    private GameObject obj_actor = null;
    private Vector3 offset = new Vector3(0.0f, 15.0f, -8.0f);

    public void Init(GameObject obj_actor)
    {
        this.obj_actor = obj_actor;
    }

    public void SetIdle()
    {
        Init(null);
    }

    public void Update()
    {
        if (obj_actor && GM_World.TO_HELL.y < obj_actor.transform.position.y)
        {
            transform.position = obj_actor.transform.position + offset;
            transform.rotation = Quaternion.LookRotation(-offset);
        }
    }
	
}
