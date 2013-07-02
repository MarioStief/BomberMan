using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public abstract class NET_CL_Entity : MonoBehaviour
{
    public struct State
    {
        public int reqId;
        public float time;
        public Rink.Pos rpos;
    }

    // position in xz-plane
    public virtual void SetPosition(Rink.Pos rpos) { }
    public abstract void SetPosition(Vector3 position);
    public abstract Vector3 GetPosition();

    // raw server responses
    public abstract Vector3 GetServerPosition();

}
