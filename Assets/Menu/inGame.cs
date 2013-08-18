using UnityEngine;
using System.Collections;

public class inGame : MonoBehaviour {
	
	void Update () {
		if (Application.loadedLevel == 0)
			return;
		
		if (Input.GetKeyDown(KeyCode.Escape))
			Menu.showGUI = !Menu.showGUI;
	}
	
	public static bool focusToChat = true;
	void OnGUI () {
		if (Application.loadedLevel == 0)
			return;
		
		if (GUI.Button(new Rect(10,10,80,20), "MENU")) {
			Menu.showGUI = !Menu.showGUI;
		}
		
		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return) {
			if (focusToChat) {
				GUI.FocusControl("chat");
			} else {
				GUI.SetNextControlName("game");
            	GUI.Label(new Rect(-100, -100, 1, 1), "");
            	GUI.FocusControl("game");
			}
			focusToChat = !focusToChat;
		}
		
		Menu.instance.chatArea();
	}
}
