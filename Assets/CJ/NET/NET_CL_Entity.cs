using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public abstract class NET_CL_Entity : MonoBehaviour
{
    public struct State
    {
        // uses actorState.reqID
        public int reqId; // REMOVEME
        public float time;
        public NET_ActorState.Message actorState;
    }

    public abstract void SetPosition(Vector3 position);
    public abstract Vector3 GetPosition();
    public abstract float GetVerticalAngle();
    public abstract float GetHorizontalAngle();
}
