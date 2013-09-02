using UnityEngine;
using System.Collections;

public class MouseLook : MonoBehaviour {
	
	private GameObject player;
	private float pinchLength;
	
	void Start() {
		player = GameObject.FindGameObjectWithTag("Player");
	}

	void Update () {

		if (Input.touchCount == 2 && Input.GetTouch(1).phase == TouchPhase.Began) {
			pinchLength = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
		}
		if (Input.touchCount == 2 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)) {
			float deltaLength = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
			camera.fieldOfView = Mathf.Clamp(camera.fieldOfView/pinchLength*deltaLength, 20, 60);
			pinchLength = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
		}
		
		if (Input.GetAxis("Mouse ScrollWheel") > 0) {
			if (camera.fieldOfView > 45)
				camera.fieldOfView -= 2;
		}
		if (Input.GetAxis("Mouse ScrollWheel") < 0) {
			if (camera.fieldOfView < 82)
				camera.fieldOfView += 2;
		}

		// Spieler auf Mauszeiger schauen lassen
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
