using UnityEngine;
using System.Collections;

public class GM_SpectatorCam : MonoBehaviour {

    GameObject obj_mainCamera = null;

	void Start()
    {
        obj_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        obj_mainCamera.transform.position = new Vector3(0.0f, 30.0f, 0.0f);
        obj_mainCamera.transform.rotation = Quaternion.LookRotation(new Vector3(0.0f, -1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f));
    }

    void Update()
    {
        Vector3 trans = Vector3.zero;

        if (Input.GetKey(KeyCode.LeftArrow)) trans += new Vector3(-1.0f, 0.0f, 0.0f);
        if (Input.GetKey(KeyCode.RightArrow)) trans += new Vector3(1.0f, 0.0f, 0.0f);
        if (Input.GetKey(KeyCode.DownArrow)) trans += new Vector3(0.0f, 0.0f, -1.0f);
        if (Input.GetKey(KeyCode.UpArrow)) trans += new Vector3(0.0f, 0.0f, 1.0f);

        if (Input.GetKey(KeyCode.A)) trans += new Vector3(0.0f, 1.0f, 0.0f);
        if (Input.GetKey(KeyCode.D)) trans += new Vector3(0.0f, -1.0f, 0.0f);

        trans.Normalize();

        obj_mainCamera.transform.position += trans;
    }
}

