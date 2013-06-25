using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GM_GS_Connect : GM_State {

    private GameObject obj_gameController = null;
    private NET_Client scr_netClient = null;

    private string name = "Sir Bombs-A-Lot";
    private string ipAddress = "localhost";
    private string port = "" + NET_Server.PORT;

    private UpdateRet ret = UpdateRet.CONTINUE;
    private GM_State nextState = null;

    public override void Start()
    {
        obj_gameController = GameObject.FindGameObjectWithTag("GameController");
        scr_netClient = obj_gameController.GetComponent<NET_Client>();
    }

    public override void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10.0f, 10.0f, 200.0f, 200.0f));
        GUILayout.BeginVertical();

        if (NetworkPeerType.Server == Network.peerType)
        {
            // don't draw anything
        }
        else if (NetworkPeerType.Client == Network.peerType)
        {
            if (GUILayout.Button("Disconnect")) scr_netClient.StopClient();
        }
        else
        {
            bool connect = GUILayout.Button("Connect Remote");

            GUILayout.BeginHorizontal();
            GUILayout.Label("username");
            name = GUILayout.TextField(name);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("ip address");
            ipAddress = GUILayout.TextField(ipAddress);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("port");
            port = GUILayout.TextField(port);
            GUILayout.EndHorizontal();

            if (GUILayout.Button("< back"))
            {
                nextState = new GM_GS_MainMenu();
                ret = UpdateRet.NEXT_STATE;
            }

            if (connect) scr_netClient.StartClient(name, ipAddress, int.Parse(port));
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();

        // lobby
        GUILayout.BeginArea(new Rect(300.0f, 10.0f, 200.0f, 200.0f));
        GUILayout.BeginVertical();
        GUILayout.Label("Connected Clients:");

        foreach (string name in scr_netClient.AnnotatedClientNames())
            GUILayout.Label(name);


        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    public override UpdateRet Update()
    {
        return ret;
    }

    public override void HandleMessage(NET_Message msg)
    {
        if (UpdateRet.NEXT_STATE == ret)
        {
            // resend messages to the next state
            msg.resend = true;
            return;
        }

        if (NET_Message.MSG_STARTGAME == msg.GetMsgID())
        {
            nextState = new GM_GS_ClGame();
            ret = UpdateRet.NEXT_STATE;
        }
    }

    public override GM_State NextState()
    {
        return nextState;
    }
}
