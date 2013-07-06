using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class NET_ActorState {

    public class Message
    {
        // request IDs are used to detect out-of-order packages.
        private static int reqIdCnt = 0;

        public int reqID = 0;
        public Vector3 position = Vector3.zero;
        public float vertAng = 0.0f, horzAng = 0.0f;

        public Message() { }

        public Message(Vector3 position, float vertAng, float horzAng)
        {
            this.position = position;
            this.vertAng = vertAng;
            this.horzAng = horzAng;
        }

        // reqIDs must be assigned before sending
        public void AssignID() { reqID = ++reqIdCnt; }

        public static void Serialize(BitStream stream, Message msg)
        {
            stream.Serialize(ref msg.reqID);
            stream.Serialize(ref msg.position);
            stream.Serialize(ref msg.vertAng);
            stream.Serialize(ref msg.horzAng);
        }
    }

}
