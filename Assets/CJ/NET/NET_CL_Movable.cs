using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NET_CL_Moveable : NET_CL_Entity {

    public static float IP_LAG = 0.1f;

    private List<State> buffer = new List<State>();
    private int minReqID = 0;
    private float time = 0.0f;

    private Vector3 position;
    private Vector3 serverPosition = Vector3.zero;

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
        return serverPosition;
    }

    private static float Ease(float t) 
    {
        return 0.5f * (Mathf.Sin(t * Mathf.PI - 0.5f * Mathf.PI) + 1.0f);
    }

    public void Update()
    {
        time += Time.deltaTime;

        float dtime = time - IP_LAG;

        int i = buffer.Count - 1;
        for (; 0 <= i && buffer[i].time > dtime; --i) ;
        if (0 <= i)
        {
            if (i + 1 < buffer.Count)
            {
                float t = (dtime - buffer[i].time) / (buffer[i + 1].time - buffer[i].time);
                position = Vector3.Lerp(buffer[i].position, buffer[i + 1].position, t);
            }
            if(0 < i) buffer.RemoveRange(0, i - 1);
        }
    }

    private class CompareTime : IComparer<State>
    {
        public int Compare(State lhp, State rhp)
        {
            return System.Math.Sign(lhp.time - rhp.time);
        }
    }

    public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        State state = new State();

        stream.Serialize(ref state.reqId);
        stream.Serialize(ref state.time);
        stream.Serialize(ref state.position);

        buffer.Add(state);
        buffer.Sort(new CompareTime());

        serverPosition = state.position;
        time = state.time; // accept server time
    }
	
}
