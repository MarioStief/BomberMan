using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NET_SV_Input : MonoBehaviour {

    private List<NET_Input.Message> buffer = new List<NET_Input.Message>();
    private int minReqID = 0;

    public NET_Input.Message GetInput()
    {
        NET_Input.Message msg = null;
        if (0 < buffer.Count)
        {
            msg = buffer[0];
            buffer.RemoveAt(0);
            minReqID = msg.reqId;
        }
        return msg;
    }

    private class CompareReqID : IComparer<NET_Input.Message>
    {
        public int Compare(NET_Input.Message lhp, NET_Input.Message rhp)
        {
            return lhp.reqId - rhp.reqId;
        }
    }

    public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        int numMsgs = 0;
        stream.Serialize(ref numMsgs);
        for (int i = 0; i < numMsgs; ++i)
        {
            NET_Input.Message msg = new NET_Input.Message();
            stream.Serialize(ref msg.reqId);
            stream.Serialize(ref msg.input);

            if (minReqID <= msg.reqId)
                buffer.Add(msg);
        }
        buffer.Sort(new CompareReqID());
    }

}
