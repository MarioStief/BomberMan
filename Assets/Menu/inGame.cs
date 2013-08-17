using UnityEngine;
using System.Collections;

public class inGame : MonoBehaviour {
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape) && Application.loadedLevel != 0) //MenuState.instance.Update() == GM_State.UpdateRet.NEXT_STATE
			Menu.showGUI = !Menu.showGUI;
	}
	
	bool showSure = false;
	void OnGUI () {
		//if (MenuState.instance.Update() == GM_State.UpdateRet.CONTINUE) // we are not ingame!
		if (Application.loadedLevel == 0)
			return;
		
		if (GUI.Button(new Rect(10,10,80,20), "MENU")) {
			//showSure = !showSure;
			Menu.showGUI = !Menu.showGUI;
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
