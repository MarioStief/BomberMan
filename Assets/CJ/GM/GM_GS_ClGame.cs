using UnityEngine;
using System.Collections;

public class GM_GS_ClGame : GM_State {

    private NET_Client scr_netClient = null;
    private GM_ClWorld world = new GM_ClWorld();

    public override void Start()
    {
        scr_netClient = GameObject.FindGameObjectWithTag("GameController").GetComponent<NET_Client>();
        world.Genesis();
    }

    public override void  HandleMessage(NET_Message msg)
    {
        world.HandleMessage(msg);
    }

    public override GM_State.UpdateRet Update()
    {
        world.Update();

        return UpdateRet.CONTINUE;
    }

    public override void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10.0f, 10.0f, 200.0f, 200.0f));
        GUILayout.BeginVertical();
        GUILayout.Label("Client Game");

        if (GUILayout.Button("Toggle Ghosts"))
            world.ToggleGhosts();

        GUILayout.EndVertical();
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(210.0f, 10.0f, 200.0f, 200.0f));
        GUILayout.BeginVertical();
        GUILayout.Label("Ping: " + scr_netClient.AvgPing());
        GUILayout.Label("IP Latency:");
        GUILayout.BeginHorizontal();
        float delta = 0.0f;
        if (GUILayout.Button("<")) delta -= 0.01f;
        GUILayout.Label("" + NET_CL_Moveable.IP_LAG);
        if (GUILayout.Button(">")) delta += 0.01f;
        NET_CL_Moveable.IP_LAG += delta;
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
	
}
