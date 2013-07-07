using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class NET_ActorState {

    public class Message
    {
        public float time;
        public Vector3 position = Vector3.zero;
        public float vertAng = 0.0f, horzAng = 0.0f;

        public Message() { }

        public Message(Vector3 position, float vertAng, float horzAng)
        {
            this.position = position;
            this.vertAng = vertAng;
            this.horzAng = horzAng;
        }

        public static void Serialize(BitStream stream, Message msg)
        {
            stream.Serialize(ref msg.time);
            stream.Serialize(ref msg.position.x);
            stream.Serialize(ref msg.position.y);
            // position.z is unused and always 0
            stream.Serialize(ref msg.vertAng);
            stream.Serialize(ref msg.horzAng);
        }
    }

}
