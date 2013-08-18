using UnityEngine;
using System.Collections;

public class startPan : MonoBehaviour {
	
	public Vector3 targetPos = new Vector3(0, 3.615212f, -0.5231594f);
	public Vector3 targetRot = new Vector3(81, 0, 0);
	public float targetClip = 1;
	
	private bool effect;
	
	// Use this for initialization
	void Start () {
		effect = Random.Range(0,2) == 0;
		
		if (effect)
			camera.nearClipPlane = 6;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.anyKeyDown) {
			transform.position = targetPos;
			transform.rotation = Quaternion.Euler(targetRot);
			camera.nearClipPlane = targetClip;
			Destroy(this);
		}
			
		if (effect && camera.nearClipPlane > targetClip) {
			transform.position = targetPos;
			transform.rotation = Quaternion.Euler(targetRot);
			
			camera.nearClipPlane -= 0.05f;
		} else {
			camera.nearClipPlane = targetClip;
			
			transform.position = Vector3.Slerp(transform.position, targetPos, Time.deltaTime);
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRot), Time.deltaTime * 2);
			transform.position = targetPos;
		}

	}
}
