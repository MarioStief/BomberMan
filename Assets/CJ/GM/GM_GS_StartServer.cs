using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GM_GS_StartServer : GM_State {

    private GameObject obj_gameController = null;
    private NET_Server scr_netServer = null;

    private UpdateRet ret = UpdateRet.CONTINUE;
    private GM_State nextState = null;

    public override void Start()
    {
        obj_gameController = GameObject.FindGameObjectWithTag("GameController");
        scr_netServer = obj_gameController.GetComponent<NET_Server>();
    }

    public override void OnGUI()
    {
        // server controls
        GUILayout.BeginArea(new Rect(10.0f, 10.0f, 200.0f, 200.0f));
        GUILayout.BeginVertical();
        if (NetworkPeerType.Server == Network.peerType)
        {
            if (GUILayout.Button("Stop")) scr_netServer.StopServer();

            GUILayout.BeginHorizontal();
            GUILayout.Label("ip address");
            GUILayout.TextField(Network.player.ipAddress);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("port");
            GUILayout.TextField(Network.player.port.ToString());
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("external ip");
            GUILayout.TextField(Network.player.externalIP);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("external port");
            GUILayout.TextField(Network.player.externalPort.ToString());
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Start Game"))
            {
                nextState = new GM_GS_SvGame();
                ret = UpdateRet.NEXT_STATE;
            }
        }
        else
        {
            if (GUILayout.Button("Start")) scr_netServer.StartServer();

            GUILayout.Label("server disconnected");

            if (GUILayout.Button("< back"))
            {
                nextState = new GM_GS_MainMenu();
                ret = UpdateRet.NEXT_STATE;
            }
        }
        GUILayout.EndVertical();
        GUILayout.EndArea();

        // lobby
        GUILayout.BeginArea(new Rect(300.0f, 10.0f, 200.0f, 200.0f));
        GUILayout.BeginVertical();
        GUILayout.Label("Connected Clients:");

        List<string> clientNames = scr_netServer.ClientNames();
        foreach (string name in clientNames)
            GUILayout.Label(name);

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    public override UpdateRet Update()
    {
        return ret;
    }

    public override GM_State NextState()
    {
        return nextState;
    }
}
