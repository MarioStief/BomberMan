using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class inGame : MonoBehaviour {
	
	public static bool focusToChat = true;
	private static int counter = -1;
	private bool showScore = false;
	
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
		if (Input.GetKeyDown(KeyCode.Tab))
			showScore = true;
		if (Input.GetKeyUp(KeyCode.Tab))
			showScore = false;
	}
	
	void OnGUI () {
		if (Application.loadedLevelName != "SphereCreate")
			return;

#if UNITY_IPHONE
		GUI.matrix = Matrix4x4.Scale(new Vector3((float)Screen.width/Menu.scrnWidth, (float)Screen.height/Menu.scrnHeight, 1f));
#else
		GUI.matrix = Matrix4x4.Scale(new Vector3((float)Screen.width/Menu.scrnWidth, (float)Screen.height/Menu.scrnHeight, 1f));
#endif
			
		// Scoreboard
		if (Static.player.isDead() || Menu.showGUI || showScore) {
			int left = Menu.scrnWidth/2 - 200;
			int i = 0;
			foreach (string p in Static.player.getWins()) {
				GUI.Label(new Rect(left, 20*i++, 400,20), p);
			}
		}
		// remaining enemies (top right)
	    GUI.skin.label.alignment = TextAnchor.MiddleRight;
		GUI.Label(new Rect(Menu.scrnWidth-155,5,150,20), "remaining enemies: " + (Static.player.getPlayersAlive().Count-1));
	    GUI.skin.label.alignment = TextAnchor.MiddleLeft;
		
		// collected powerups
		Texture2D[] icons = Static.player.getIcons();
		string[] text = Static.player.getIconText();
		GUI.BeginGroup(new Rect(10,Menu.scrnHeight-icons.Length*32-20,200,600));
		for (int i=0; i<icons.Length; i++) {
			if (icons[i] != null) {
				GUI.DrawTexture(new Rect(0,i*32,32,32), icons[i]);
				GUI.Label(new Rect(35,i*32,100,32), text[i]);
			}
		}
		GUI.EndGroup();
		
		// Next Round Countdown
		if (counter > 0) {
	    	GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			int fs = GUI.skin.label.fontSize;
			GUI.skin.label.fontSize = 40;
			GUI.Label(new Rect(Menu.scrnWidth/2-200,Menu.scrnHeight/2-50,400,100), "Round starts in " + counter);
			GUI.skin.label.fontSize = fs;
		}
	    GUI.skin.label.alignment = TextAnchor.MiddleLeft;
		
		// Menu-Button
		if (GUI.Button(new Rect(10,10,80,20), "MENU")) {
			Menu.showGUI = !Menu.showGUI;
		}
		// Chat
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
		
		if (Event.current.keyCode == KeyCode.Tab || Event.current.character == '\t')
   			Event.current.Use();
	}
	
	public static void startCounter(int c) {
		counter = c;
		Static.menuHandler.GetComponent<inGame>().Invoke("decCounter",1);
	}
	private void decCounter() {
		counter--;
		if (counter > -1)
			Invoke("decCounter", 1);
	}

}