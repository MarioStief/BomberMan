using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NET_Client : MonoBehaviour {

    public GameObject pre_clInput = null; // set this in editor

    private GameObject obj_gameController = null;
    private TheGame scr_theGame = null;

    private GameObject obj_input = null;
    private NET_CL_Input scr_input = null;

    private NetworkPlayer serverPlayer;

    private string name = null;
    private int pid = 0;

    private NET_OutMessageQueue outQueue;
    private NET_InMessageQueue broadcastInQueue;
    private LinkedList<NET_Message> resendMsgs = new LinkedList<NET_Message>();

    public class Client
    {
        public string name;
        public int pid;
    }

    private List<Client> clients = new List<Client>();

    private Client ClientByPID(int pid)
    {
        foreach (Client client in clients)
            if (pid == client.pid) return client;
        return null;
    }

    public List<string> AnnotatedClientNames()
    {
        List<string> names = new List<string>();
        foreach (Client client in clients)
        {
            string name = client.name;
            if (pid == client.pid)
                name += " (me)";
            names.Add(name);
        }
        return names;
    }
	
	public void removeClient(int pid) {
		foreach (Client client in clients) {
			if (client.pid == pid) {
				clients.Remove(client);
				break;
			}
		}
	}
	public void clearClients() {
		clients.Clear();
	}

    public int GetLocalPID()
    {
        return pid;
    }

    public NET_CL_Input GetLocalInput()
    {
        return scr_input;
    }

    public int AvgPing()
    {
        return Network.GetAveragePing(serverPlayer);
    }

    public void Send(NET_Message msg)
    {
        outQueue.Add(msg);
    }

    public void StartClient(string name, string ipAddress, int port)
    {
        obj_gameController = GameObject.FindGameObjectWithTag("GameController");
        scr_theGame = obj_gameController.GetComponent<TheGame>();

        obj_input = (GameObject)GameObject.Instantiate(pre_clInput);
        scr_input = obj_input.GetComponent<NET_CL_Input>();

        this.name = name;

        //Network.Connect(ipAddress, port);
    }


	[RPC]
    public void NET_Ack0(NetworkPlayer serverPlayer, int pid, NetworkViewID broadcastViewID)
    {
        this.pid = pid;

        NetworkViewID inputViewID = Network.AllocateViewID();

        obj_input.networkView.viewID = inputViewID;
        obj_input.networkView.group = pid;

        outQueue = NET_OutMessageQueue.Create("ClOut");

        networkView.RPC("NET_Ack1", serverPlayer, name, pid, inputViewID, outQueue.viewID);

        Debug.Log("CL(" + pid + "): created ClInput with inputViewID = " + inputViewID);

        broadcastInQueue = NET_InMessageQueue.Create(broadcastViewID, "ClBroadcast");

        this.serverPlayer = serverPlayer;
    }

    [RPC]
    public void NET_ClientConnected(string name, int pid)
    {
        if (null == ClientByPID(pid))
        {
            Client client = new Client();
            client.name = name;
            client.pid = pid;
            clients.Add(client);
        }
    }

    public void Update()
    {
        LinkedListNode<NET_Message> msgIt = resendMsgs.First;
        while (null != msgIt)
        {
            LinkedListNode<NET_Message> next = msgIt.Next;
            NET_Message msg = msgIt.Value;
            msg.resend = false;

            scr_theGame.HandleMessage(msg);

            if (!msg.resend) resendMsgs.Remove(msgIt);

            msgIt = next;
        }

        while (null != broadcastInQueue && !broadcastInQueue.IsEmpty())
        {
            NET_Message msg = broadcastInQueue.Pop();
            msg.resend = false;

            scr_theGame.HandleMessage(msg);

            if (msg.resend) resendMsgs.AddLast(msg);
        }
    }
}
