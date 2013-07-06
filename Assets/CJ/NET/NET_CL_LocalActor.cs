using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NET_CL_LocalActor : NET_CL_Entity {

    private NET_Client scr_netClient = null;
    private NET_CL_Input scr_localInput = null;

    private Vector3 position;
    private Vector3 serverPosition = Vector3.zero;
    private int maxReqID = 0;

    private CharacterController charCtrl = null;

    private float time = 0.0f;
    private float accu = 0.0f;

    public override void SetPosition(Vector3 position)
    {
        this.position = position;
    }

    public override Vector3 GetPosition()
    {
        return position;
    }

    public override float GetVerticalAngle()
    {
        return 0.0f;
    }

    public override float GetHorizontalAngle()
    {
        return 0.0f;
    }

    public Vector3 GetServerPosition()
    {
        return serverPosition;
    }

    public void Start()
    {
        GameObject obj_gameController = (GameObject)GameObject.FindGameObjectWithTag("GameController");
        scr_netClient = obj_gameController.GetComponent<NET_Client>();
        scr_localInput = scr_netClient.GetLocalInput();

        charCtrl = gameObject.GetComponent<CharacterController>();
    }

    public void Update()
    {
        accu += Time.deltaTime;

        while (GM_World.TIMESTEP <= accu)
        {
            accu -= GM_World.TIMESTEP;
            time += GM_World.TIMESTEP;

            int input = NET_Input.EncodeLocalInput();
            scr_localInput.AddInput(input);

            Vector3 trans = NET_Input.DecodeTranslation(input);
            charCtrl.Move(GM_World.TIMESTEP * (GM_World.ACTOR_SPEED * trans + GM_World.GRAVITY_VEC));
        }
        position = gameObject.transform.position;

        if (Input.GetButtonDown("Fire1"))
        {
            NET_MSG_PlantBomb plantBombMsg = new NET_MSG_PlantBomb();
            plantBombMsg.pid = scr_netClient.GetLocalPID();
            plantBombMsg.time = time;
            scr_netClient.Send(plantBombMsg);
        }
    }

    public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        State state = new State();

        stream.Serialize(ref state.reqId);
        stream.Serialize(ref state.time);
        //stream.Serialize(ref state.position);

        time = state.time;

        if (maxReqID <= state.reqId)
        {
            maxReqID = state.reqId;
            scr_localInput.Ack(gameObject, charCtrl, state);
            position = gameObject.transform.position;
        }

        // serverPosition = state.position;
    }

}
