using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NET_InMessageQueue : MonoBehaviour
{
    private Queue<NET_Message> msgs = new Queue<NET_Message>();

    public bool IsEmpty()
    {
        return 0 >= msgs.Count;
    }

    public NET_Message Pop()
    {
        return msgs.Dequeue();
    }

    public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        int numMsgs = -1;
        stream.Serialize(ref numMsgs);
        for (int i = 0; i < numMsgs; ++i)
        {
            int msgID = -1;
            stream.Serialize(ref msgID);
            NET_Message msg = NET_Message.CreateFromID(msgID);
            if (null == msg)
            {
                Debug.LogError("NET_InMessageQueue: unknown msgID " + msgID);
            }
            else
            {
                msg.Serialize(stream);
                msgs.Enqueue(msg);

                if (NET_Message.MSG_TIME != msg.GetMsgID() /* don't clutter console with frequently sent time msgs */)
                {
                    Debug.Log("NET_InMessageQueue: recv msg, type=" + NET_Message.IDToString(msg.GetMsgID()));
                }
            }
        }
    }

    public static NET_InMessageQueue Create(NetworkViewID viewID, string name)
    {
        GameObject obj = new GameObject(name + " (InMessageQueue)");
        NET_InMessageQueue inQueue = (NET_InMessageQueue)obj.AddComponent("NET_InMessageQueue");
        NetworkView netView = obj.AddComponent<NetworkView>();
        netView.viewID = viewID;
        netView.observed = inQueue;
        return inQueue;
    }
}
