using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class inGame : MonoBehaviour {
	
	void Update () {
		if (Application.loadedLevelName != "SphereCreate")
			return;
		
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Menu.showGUI = !Menu.showGUI;
			if (Menu.showGUI) {
				Static.inputHandler.lockCursor(false);
				Static.camera.GetComponent<MouseLookGame>().setRotatable(false);
			}
			else {
				Static.inputHandler.lockCursor(true);
				Static.camera.GetComponent<MouseLookGame>().setRotatable(true);
			}
		}
	}
	
	public static bool focusToChat = true;
	void OnGUI () {
		if (Application.loadedLevelName != "SphereCreate")
			return;
		
		if (Static.player.isDead() || Menu.showGUI) {
			int left = Screen.width/2 - 50;
			int i = 0;
			foreach (string p in Static.player.getWins()) {
				GUI.Label(new Rect(left, 20*i++, 100,20), p);
			}
		}
	    GUI.skin.label.alignment = TextAnchor.MiddleRight;
		GUI.Label(new Rect(Screen.width-155,5,150,20), "remaining enemies: " + (Static.player.getPlayersAlive().Count-1));
	    GUI.skin.label.alignment = TextAnchor.MiddleLeft;
		
		
		if (GUI.Button(new Rect(10,10,80,20), "MENU")) {
			Menu.showGUI = !Menu.showGUI;
		}
		
		if (Event.current.type == EventType.KeyDown && (
				Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)) {
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