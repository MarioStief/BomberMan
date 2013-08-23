using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
	public class MouseLookGame : MonoBehaviour {
		
		private Vector3 position;
		private GameObject player;
		private bool rotatable = true;
		public bool birdview = true;
		
		void Start() {
			Static.setCamera(this.camera);
			player = GameObject.FindGameObjectWithTag("Player");
			transform.localPosition = new Vector3(-0.02f, -0.02f, 0f);
			position = Vector3.zero;
		}
	
		void Update () {
			
			if (!birdview) {
				float h = Input.GetAxis ("Mouse Y") / 2;
			    float v = Input.GetAxis ("Mouse X") / 2;
			    transform.Translate(v, h, 0);
				transform.LookAt(player.transform.position);
			}
			
			// Camera Movement
			// left click
			if (Input.GetButton ("Fire1")) {
				Static.inputHandler.dropBomb();
			}
			
			// right click
			if (Input.GetButton ("Fire2")) {
				Static.inputHandler.dropContactMine();
			}
			
			// middle click
			if (Input.GetButton ("Fire3")) {
				birdview = !birdview;
				if (birdview) {
					position = Vector3.zero;
					transform.Rotate(Vector3.zero);
					//transform.LookAt(player.transform.position);
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
			if (transform.localPosition != position) {
				transform.localPosition = position + (transform.localPosition + position)/2; 
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
