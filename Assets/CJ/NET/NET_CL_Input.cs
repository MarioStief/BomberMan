using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NET_CL_Input : MonoBehaviour {

    private LinkedList<NET_Input.Message> inputBuffer = new LinkedList<NET_Input.Message>();
    private LinkedList<NET_Input.Message> sendBuffer = new LinkedList<NET_Input.Message>();
    private int reqIdCnt = 0;

    public void AddInput(int input)
    {
        if (0 < input)
        {
            NET_Input.Message msg = new NET_Input.Message();
            msg.reqId = ++reqIdCnt;
            msg.input = input;

            // buffers are sorted by reqId
            inputBuffer.AddLast(msg);
            sendBuffer.AddLast(msg);
        }
    }

    public void Ack(GameObject obj, CharacterController charCtrl, NET_CL_Entity.State state)
    {
        while (0 < inputBuffer.Count && inputBuffer.First.Value.reqId <= state.reqId)
            inputBuffer.RemoveFirst();
        // obj.transform.position = state.position;
        foreach (NET_Input.Message msg in inputBuffer)
        {
            Vector3 trans = NET_Input.DecodeTranslation(msg.input);
            charCtrl.Move(GM_World.TIMESTEP * (GM_World.ACTOR_SPEED * trans + GM_World.GRAVITY_VEC));
        }
    }

    public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        int numMsgs = sendBuffer.Count;
        stream.Serialize(ref numMsgs);
        foreach (NET_Input.Message msg in sendBuffer)
        {
            int reqId = msg.reqId;
            int input = msg.input;

            stream.Serialize(ref reqId);
            stream.Serialize(ref input);
        }
        sendBuffer.Clear();
    }
}
