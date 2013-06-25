using UnityEngine;
using System.Collections;

public abstract class NET_CL_Entity : MonoBehaviour
{
    public struct State
    {
        public int reqId;
        public float time;
        public Vector3 position;
    }

    // position in xz-plane
    public abstract void SetPosition(Vector3 position);
    public abstract Vector3 GetPosition();

    // raw server responses
    public abstract Vector3 GetServerPosition();

}
