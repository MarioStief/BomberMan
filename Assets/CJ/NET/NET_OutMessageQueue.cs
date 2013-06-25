using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NET_OutMessageQueue : MonoBehaviour
{
    private Queue<NET_Message> msgs = new Queue<NET_Message>();
    public NetworkViewID viewID;

    public void Add(NET_Message msg)
    {
        msgs.Enqueue(msg);
    }

    public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        int numMsgs = msgs.Count;
        if (0 < numMsgs)
        {
            stream.Serialize(ref numMsgs);
            foreach (NET_Message msg in msgs)
            {
                int msgID = msg.GetMsgID();
                stream.Serialize(ref msgID);
                msg.Serialize(stream);
            }
            msgs.Clear();
        }
    }

    public static NET_OutMessageQueue Create(string name)
    {
        NetworkViewID viewID = Network.AllocateViewID();
        GameObject obj = new GameObject(name + " (OutMessageQueue)");
        NET_OutMessageQueue outQueue = (NET_OutMessageQueue)obj.AddComponent("NET_OutMessageQueue");
        outQueue.viewID = viewID;
        NetworkView netView = obj.AddComponent<NetworkView>();
        netView.viewID = viewID;
        netView.observed = outQueue;
        return outQueue;
    }
}