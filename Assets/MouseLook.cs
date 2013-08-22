using UnityEngine;
using System.Collections;

public class MouseLook : MonoBehaviour {
	
	Vector3 position;
	float z = 0f;
	
	void Start() {
		position = Vector3.zero;
		//step = transform.parent.position.normalized/10;
		//z = new Vector3(0f, 0f, 0.05f);
	}

	void Update () {
		
		// Camera Movement
		// left click
		if (Input.GetMouseButtonUp(0)) {
		}
		// right click
		if (Input.GetMouseButtonUp(1)) {
		}
		// middle click
		if (Input.GetMouseButtonUp(2)) {
			z = 0f;
		}
		if (Input.GetAxis("Mouse ScrollWheel") > 0) {
			//Debug.Log("Mouse ScrollWheel > 0");
			if (z > -1.7f)
				z -= 0.1f;
		}
		if (Input.GetAxis("Mouse ScrollWheel") < 0) {
			//Debug.Log("Mouse ScrollWheel < 0");
			if (z < 0.7f)
				z += 0.05f;
		}
		transform.localPosition = new Vector3(0f, 0f, z);
		//transform.LookAt(GameObject.FindGameObjectWithTag("Player").transform.position);
	}
}
