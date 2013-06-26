using UnityEngine;
using System.Collections;

public class inGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	bool showSure = false;
	void OnGUI () {
		if (Menu.showGUI) // we are not ingame!
			return;
		
		if (GUI.Button(new Rect(10,10,80,20), "MENU")) {
			showSure = !showSure;
		}
		if (showSure) {
			GUI.Box(new Rect(10,32,100,20), "Disconnect?");
			if (GUI.Button(new Rect(63,55,47,20),"NO")) {
				showSure = false;
			}
			if (GUI.Button(new Rect(10,55,50,20),"YES")) {
				//Application.LoadLevel(0);
				Menu.showGUI = true;
				showSure = false;
			}
		}
		
		// List of all Players
		for (int i=0; i < Network.connections.Length; i++) {
			//Debug.Log(cons[i].externalIP.ToString());
		}
	}
}
