using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
	public class MouseLookGame : MonoBehaviour {
		
		private Vector3 position;
		private GameObject player;
		private bool rotatable = true;
		public bool birdview = true;
		private float mouseSensitivity;
		
		void Start() {
			Static.setCamera(this.camera);
			player = GameObject.FindGameObjectWithTag("Player");
			transform.localPosition = new Vector3(-0.02f, -0.02f, 0f);
			position = Vector3.zero;
			mouseSensitivity = Preferences.getMouseSensitivity();
		}
	
		void Update () {
			
			if (!birdview) {
				float h = mouseSensitivity * Input.GetAxis ("Mouse Y") / 2;
			    float v = mouseSensitivity * Input.GetAxis ("Mouse X") / 2;
				float x = transform.localPosition.x + h;
				if (x < -2f)
					x = -2f;
				else if (x > 2f)
					x = 2f;
				float y = transform.localPosition.y + v;
				if (y < -2f)
					y = -2f;
				else if (y > 2f)
					y = 2f;
				transform.localPosition = new Vector3(x, y, transform.localPosition.z);
			    //transform.Translate(v, h, 0);
				transform.LookAt(player.transform.position);
			}
			
			// Camera Movement
			// left click
			if (Input.GetButton ("Fire1")) {
				Static.inputHandler.dropBomb();
			}
			
			// right click
			if (Input.GetButton ("Fire2")) {
				Static.inputHandler.extra();
			}
			
			// middle click
			if (Input.GetButton ("Fire3")) {
				if (rotatable) {
					birdview = !birdview;
					if (birdview) {
						position = Vector3.zero;
						transform.rotation = Quaternion.Euler(new Vector3(81,0,0));
						//transform.LookAt(player.transform.position);
					}
				}
			}
			
			if (Input.GetAxis("Mouse ScrollWheel") > 0) {
				if (camera.fieldOfView > 20)
					camera.fieldOfView--;
			}
			
			if (Input.GetAxis("Mouse ScrollWheel") < 0) {
				if (camera.fieldOfView < 82)
					camera.fieldOfView++;
			}
			
			// keine abrupten Kameraübergänge
			if (birdview && transform.localPosition != position) {
				transform.localPosition = position + (transform.localPosition + position)/2; 
			}
		}
		
		public void setRotatable(bool r) {
			rotatable = r;
			if (r)
				birdview = false;
		}
		
		public bool getRotation() {
			return rotatable;
		}
	}
}
