using UnityEngine;
using System.Collections;

public class MouseLook : MonoBehaviour {
	
	Vector3 position;
	float z = 0f;
	GameObject player;
	
	void Start() {
		//position = transform.position;
		//step = transform.parent.position.normalized/10;
		//z = new Vector3(0f, 0f, 0.05f);
	}

	void Update () {
		
		// Camera Movement
		// left click
		if (Input.GetMouseButton(0)) {
			/*
			Vector3 a = player.transform.rotation.eulerAngles;
			a.x = -45;
			player.transform.rotation = Quaternion.Euler(a);
			*/
		}
		
		// right click
		if (Input.GetMouseButtonUp(1)) {
		}
		// middle click
		if (Input.GetMouseButtonUp(2)) {
			z = 0f;
		}
		
		if (Input.GetAxis("Mouse ScrollWheel") > 0) {
			if (camera.fieldOfView > 45)
				camera.fieldOfView -= 1;
		}
		if (Input.GetAxis("Mouse ScrollWheel") < 0) {
			if (camera.fieldOfView < 82)
				camera.fieldOfView += 1;
		}
		
		if (player == null) {
			player = GameObject.FindWithTag("Player");
			player.transform.up = transform.position;
		}
		
		Plane playerPlane = new Plane(Vector3.up, player.transform.position);
	    Ray ray = camera.ScreenPointToRay(Input.mousePosition);
	    float hitdist = 0;
	    if (playerPlane.Raycast(ray, out hitdist)) {
	        Vector3 targetPoint = ray.GetPoint(hitdist);
			Quaternion targetRotation = Quaternion.LookRotation(targetPoint - player.transform.position);
			
	        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, Time.deltaTime * 2);
	    }
	
	}
}
