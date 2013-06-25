using UnityEngine;
using System.Collections;

public class GM_GS_MainMenu : GM_State {

    private UpdateRet ret = UpdateRet.CONTINUE;
    private GM_State nextState = null;

    public override void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10.0f, 10.0f, 200.0f, 200.0f));
        GUILayout.BeginVertical();

        if (GUILayout.Button("Start Server"))
        {
            nextState = new GM_GS_StartServer();
            ret = UpdateRet.NEXT_STATE;
        }
        if (GUILayout.Button("Connect to Server"))
        {
            nextState = new GM_GS_Connect();
            ret = UpdateRet.NEXT_STATE;
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    public override GM_State.UpdateRet Update()
    {
        return ret;
    }

    public override GM_State NextState()
    {
        return nextState;
    }
}
