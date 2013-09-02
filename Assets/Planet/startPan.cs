using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class startPan : MonoBehaviour {
	
	public Vector3 targetPos = new Vector3(0, 3.615212f, -0.5231594f);
	public Vector3 targetRot = new Vector3(81, 0, 0);
	public float targetClip = 1;
	public int targetFoV = 81;
	
	private int effect;
	private int phase = 0;
	
	void Start () {
		//if (Application.loadedLevelName != "StartMenu")
		//	return;

		effect = Random.Range(0,3);
		//targetPos = Static.sphereHandler.getStartParcel().getCenterPos() * 1.5f;
		
		if (Application.loadedLevelName == "StartMenu") {
			targetFoV = 45;
			targetPos.z = 0;
			targetRot.x = 90;
			if (effect == 1)
				effect++;
		} else {
			Vector3 tmpP = transform.position;
			Quaternion tmpR = transform.rotation;

			// Spieler hor. drehen
			targetPos = Static.rink.gameArea[Menu.spawns[Network.player][0]][Menu.spawns[Network.player][1]].getCenterPos() * 1.7f;
			targetPos.z = 0;
			float a = Vector3.Angle(targetPos, Vector3.up);
			if (targetPos.x > 0)
				a *= -1;
			transform.RotateAround(Vector3.zero, Vector3.forward, a);
			targetPos = transform.position;
			
			transform.LookAt(Vector3.zero, Vector3.forward);
			targetRot = transform.rotation.eulerAngles;
			
			transform.position = tmpP;
			transform.rotation = tmpR;
		}
		
		if (effect == 1)
			camera.nearClipPlane = 6;
	}
	
	void Update () {
		//if (Application.loadedLevelName != "StartMenu")
		//	return;
		
		if (Input.anyKeyDown || Input.GetAxis("Mouse ScrollWheel") != 0) {
			transform.position = targetPos;
			transform.rotation = Quaternion.Euler(targetRot);
			camera.nearClipPlane = targetClip;
			camera.fieldOfView = targetFoV;
			Destroy(this);
		}
			
		if (effect == 1 && camera.nearClipPlane > targetClip) { // Planet "aufbauen"
			transform.position = Vector3.Slerp(transform.position, targetPos, Time.deltaTime*4);
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRot), Time.deltaTime * 8);
			camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetFoV, Time.deltaTime*10);
			
			camera.nearClipPlane -= 0.05f;
		} else if (effect == 2) { // Lichtgeschwindigkeit
			transform.position = Vector3.Slerp(transform.position, targetPos, Time.deltaTime*4);
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRot), Time.deltaTime * 4);
			
			if (phase == 0)
				camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, 180, Time.deltaTime * 20);
			else
				camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetFoV, Time.deltaTime);
			
			if (camera.fieldOfView >= 179)
				phase = 1;
		} else { // Schwenk nach unten
			camera.nearClipPlane = targetClip;
			camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetFoV, Time.deltaTime*10);
			
			transform.position = Vector3.Slerp(transform.position, targetPos, Time.deltaTime);
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRot), Time.deltaTime * 2);
			transform.position = targetPos;
		}
	}
}
