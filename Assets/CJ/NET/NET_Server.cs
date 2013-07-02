using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class NET_Server : MonoBehaviour {

    public GameObject pre_svInput = null; // set this in editor

    public const int MAX_CONNECTIONS = 10;
    public const int PORT = 25000;
    public const bool USE_NAT_PUNCHTHROUGH = true;

    public class Client
    {
        public string name;
        public int pid;
        public NetworkPlayer netPlayer;

        public NET_InMessageQueue inQueue;

        public GameObject obj_input = null;
        public NET_SV_Input scr_input = null;

        public GameObject obj_actorState = null;
        public NET_SV_ActorState scr_actorState = null;

        // game variables
        public bool isDead;
        public float time;
        public Player player = new Player();

        // scratchpad variables
        public Vector3 trans;
    }

    private List<Client> clients = new List<Client>();

    private int pidCnt = 0;

    private NET_OutMessageQueue broadcastOutQueue;

    private TheGame scr_theGame = null;

    private string MakeUniqueName(string name, int pid)
    {
        bool unique = true;
        foreach (Client client in clients)
        {
            if (name == client.name)
                unique = false;
        }
        if (!unique) name += " " + pid;
        return name;
    }

    public Client ClientByPID(int pid)
    {
        foreach (Client client in clients)
            if (pid == client.pid) return client;
        return null;
    }

    public List<string> ClientNames()
    {
        List<string> clientNames = new List<string>();
        foreach (Client client in clients)
            clientNames.Add(client.name);
        return clientNames;
    }

    public List<Client> Clients()
    {
        return clients;
    }
	
	public int removeClient(NetworkPlayer p) {
		foreach (Client client in clients) {
			if (p == client.netPlayer) {
				clients.Remove(client);
				return client.pid;
			}
		}
		return -1;
	}
	
    public void StartServer() 
    {
        //Network.InitializeServer(MAX_CONNECTIONS, PORT, USE_NAT_PUNCHTHROUGH);

        broadcastOutQueue = NET_OutMessageQueue.Create("SvBroadcast");

        GameObject obj_gameController = GameObject.FindGameObjectWithTag("GameController");
        scr_theGame = obj_gameController.GetComponent<TheGame>();
    }

    public void OnPlayerConnected(NetworkPlayer clientPlayer)
    {
        Client client = new Client();
        client.pid = ++pidCnt;
        client.netPlayer = clientPlayer;

        clients.Add(client);

        networkView.RPC("NET_Ack0", clientPlayer, 
            Network.player, 
            client.pid,
            broadcastOutQueue.viewID);
    }

    [RPC]
    public void NET_Ack1(string name, int pid, NetworkViewID inputViewID, NetworkViewID actorStateViewID, NetworkViewID inQueueID)
    {
        Client client = ClientByPID(pid);

        client.name = MakeUniqueName(name, pid);

        client.obj_input = (GameObject)GameObject.Instantiate(pre_svInput);
        client.scr_input = client.obj_input.GetComponent<NET_SV_Input>();

        client.obj_input.networkView.viewID = inputViewID;
        client.obj_input.networkView.group = pid;

        client.obj_actorState = new GameObject("SvActorState (pid=" + pid + ")");
        client.scr_actorState = client.obj_actorState.AddComponent<NET_SV_ActorState>();
        client.obj_actorState.AddComponent<NetworkView>();
        client.obj_actorState.networkView.viewID = actorStateViewID;
        client.obj_actorState.networkView.group = pid;
        client.obj_actorState.networkView.observed = client.scr_actorState;

        client.inQueue = NET_InMessageQueue.Create(inQueueID, "SvInQueue (pid = " + pid + ")");

        Debug.Log("SV: created SvInput object for pid = " + pid + ", with inputViewID = " + inputViewID);
        Debug.Log("SV: created SvActorState object for pid = " + pid + ", with actorStateViewID = " + actorStateViewID);
        
        foreach (Client peer in clients)
        {
            if (client.pid != peer.pid)
            {
                peer.obj_input.networkView.SetScope(client.netPlayer, false);
                client.obj_input.networkView.SetScope(peer.netPlayer, false);

                peer.obj_actorState.networkView.SetScope(client.netPlayer, false);
                client.obj_actorState.networkView.SetScope(peer.netPlayer, false);

                networkView.RPC("NET_ClientConnected", peer.netPlayer, client.name, client.pid);
            }

            networkView.RPC("NET_ClientConnected", client.netPlayer, peer.name, peer.pid);
        }
    }

    public void Broadcast(NET_Message msg)
    {
        broadcastOutQueue.Add(msg);
    }

    public void Update()
    {
        foreach (Client client in clients)
        {
            while (null != client.inQueue && !client.inQueue.IsEmpty())
            {
                Debug.Log("NET_Server::Update: recv msg");

                NET_Message msg = client.inQueue.Pop();
                scr_theGame.HandleMessage(msg);
            }
        }
    }
}
