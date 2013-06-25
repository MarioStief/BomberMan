using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GM_GS_SvGame : GM_State {

    private GameObject obj_gameController = null;
    private NET_Server scr_netServer = null;

    private GM_SvWorld world = new GM_SvWorld();

    public override void Start()
    {
        obj_gameController = (GameObject)GameObject.FindGameObjectWithTag("GameController");
        scr_netServer = obj_gameController.GetComponent<NET_Server>();

        NET_MSG_StartGame startGameMsg = new NET_MSG_StartGame();
        scr_netServer.Broadcast(startGameMsg);

        world.Genesis(scr_netServer);
    }

    public override void HandleMessage(NET_Message msg)
    {
        world.HandleMessage(msg);
    }

    public override UpdateRet Update()
    {
        world.Update();

        return UpdateRet.CONTINUE;
    }
}
