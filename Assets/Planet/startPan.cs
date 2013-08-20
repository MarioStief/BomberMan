using UnityEngine;
using System.Collections;

public class startPan : MonoBehaviour {
	
	public Vector3 targetPos = new Vector3(0, 3.615212f, -0.5231594f);
	public Vector3 targetRot = new Vector3(81, 0, 0);
	public float targetClip = 1;
	public int targetFoV = 81;
	
	private int effect;
	private int phase = 0;
	
	void Start () {
		effect = Random.Range(0,3);
		
		if (effect == 1)
			camera.nearClipPlane = 6;
	}
	
	void Update () {
		if (Input.anyKeyDown) {
			transform.position = targetPos;
			transform.rotation = Quaternion.Euler(targetRot);
			camera.nearClipPlane = targetClip;
			Destroy(this);
		}
			
		if (effect == 1 && camera.nearClipPlane > targetClip) { // Planet "aufbauen"
			transform.position = Vector3.Slerp(transform.position, targetPos, Time.deltaTime*4);
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRot), Time.deltaTime * 8);
			
			camera.nearClipPlane -= 0.05f;
		} else if (effect == 2) { // Lichtgeschwindigkeit
			transform.position = Vector3.Slerp(transform.position, targetPos, Time.deltaTime*4);
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRot), Time.deltaTime * 8);
			
			if (phase == 0)
				camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, 180, Time.deltaTime * 12);
			else
				camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetFoV, Time.deltaTime);
			
			if (camera.fieldOfView >= 179)
				phase = 1;
		} else { // Schwenk nach unten
			camera.nearClipPlane = targetClip;
			
			transform.position = Vector3.Slerp(transform.position, targetPos, Time.deltaTime);
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRot), Time.deltaTime * 2);
			transform.position = targetPos;
		}

	}
}
