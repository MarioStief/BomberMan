using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
	public class MouseLookGame : MonoBehaviour {
		
		private GameObject player;
		private bool rotatable = true;
		public bool birdview = true;
		private int mouseSensitivity;
		
		void Start() {
			Static.setCamera(this.camera);
			player = GameObject.FindGameObjectWithTag("Player");
			mouseSensitivity = Preferences.getMouseSensitivity();
		}
	
		void Update () {
			
			if (Input.GetAxis("Mouse ScrollWheel") > 0) {
				if (camera.fieldOfView > 20)
					camera.fieldOfView -= 2;
			}
			
			if (Input.GetAxis("Mouse ScrollWheel") < 0) {
				if (camera.fieldOfView < 60)
					camera.fieldOfView += 2;
			}

			if (!rotatable)
				return;
			
			if (!birdview) {
		    	float v = 2 * mouseSensitivity * Input.GetAxis ("Mouse X");
				transform.RotateAround(player.transform.position, player.transform.position, v);
				
				float h = 2 * mouseSensitivity * Input.GetAxis ("Mouse Y");
				float newA = Vector3.Angle(Vector3.Cross(player.transform.up, transform.right*-1), transform.forward) + h;
				if (newA > 0 && newA <= 60)
					transform.RotateAround(player.transform.position, transform.right, h);
			} else
				if (transform.localPosition != Vector3.zero)
					transform.localPosition /= 2;
			
			// middle click
			if (Input.GetButtonDown("Fire3")) {
				birdview = !birdview;
				if (birdview) {
					transform.position = player.transform.position * 1.7f;
					transform.LookAt(player.transform.position, Vector3.forward);
				} else {
					transform.RotateAround(player.transform.position, transform.right, -45);
				}
			}
		}
		
		public void setRotatable(bool r) {
			rotatable = r;
		}
		
		public bool getRotation() {
			return rotatable;
		}
	}
}
