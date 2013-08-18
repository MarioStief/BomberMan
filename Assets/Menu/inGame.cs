using UnityEngine;
using System.Collections;

public class inGame : MonoBehaviour {
	
	void Update () {
		if (Application.loadedLevel == 0)
			return;
		
		if (Input.GetKeyDown(KeyCode.Escape))
			Menu.showGUI = !Menu.showGUI;
	}
	
	private bool focusToChat = true;
	void OnGUI () {
		if (Application.loadedLevel == 0)
			return;
		
		if (GUI.Button(new Rect(10,10,80,20), "MENU")) {
			Menu.showGUI = !Menu.showGUI;
		}
		
		if (Input.GetKeyDown(KeyCode.Return)) {
			if (focusToChat)
				GUI.FocusControl("chat");
			focusToChat = !focusToChat;
		}
		
		Menu.instance.chatArea();
	}
}
