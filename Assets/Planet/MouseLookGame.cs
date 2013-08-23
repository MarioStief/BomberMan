using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
	public class MouseLookGame : MonoBehaviour {
		
		private Vector3 position;
		private float z = 0f;
		private GameObject player;
		private bool rotationOn = true;
		
		void Start() {
			Static.setCamera(this.camera);
			player = GameObject.FindGameObjectWithTag("Player");
			transform.localPosition = new Vector3(-0.02f, -0.02f, 0f);
		}
	
		void Update () {
			
			// Camera Movement
			// left click
			if (Input.GetButton ("Fire1")) {
				float h = Input.GetAxis ("Mouse Y") / 2;
			    float v = Input.GetAxis ("Mouse X") / 2;
			    transform.Translate(v, h, 0);
				transform.LookAt(player.transform.position);
			}
			
			// right click
			if (Input.GetButton ("Fire2")) {
				transform.localPosition = Vector3.zero;
				transform.Rotate(Vector3.zero);
				transform.LookAt(player.transform.position);
			}
			
			/*
			// middle click
			if (Input.GetButton ("Fire3")) {
				z = 0f;
			}
			*/
			
			if (Input.GetAxis("Mouse ScrollWheel") > 0) {
				if (camera.fieldOfView > 45)
					camera.fieldOfView -= 1;
			}
			
			if (Input.GetAxis("Mouse ScrollWheel") < 0) {
				if (camera.fieldOfView < 82)
					camera.fieldOfView += 1;
			}
		}
		
		public void setRotation(bool r) {
			rotationOn = r;
		}
		
		public bool getRotation() {
			return rotationOn;
		}
	}
}
