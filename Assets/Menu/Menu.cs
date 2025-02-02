using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class Menu : MonoBehaviour {

	public Texture bgHelp; // Picture of Help-Menu
	
	public GameObject colorPicker;
	
	private string screen = "start";

	public string serverName = "Galaxy 42";
	private int port = 25000;
	public int maxPlayers = 4;

	private string nickname = "Player ";
	private static Color playerColor;
	private int expDetail;
	private int chestDensity;
	private int mouseSensitivity;
	private int roundsToWin;
	private string chat = "";
	
	private static Dictionary<NetworkPlayer,string> playerList = new Dictionary<NetworkPlayer, string>();
	private static Dictionary<NetworkPlayer,Color> playerColorList = new Dictionary<NetworkPlayer, Color>();
	public static Dictionary<NetworkPlayer,int[]> spawns = new Dictionary<NetworkPlayer, int[]>();
	
	public static int rSeed;
	public static bool showGUI = true;
	public static bool gameStarted = false;
	
	public static int scrnWidth = 0;
	public static int scrnHeight = 800;
	
	public static Menu instance = null;
	public void Awake() {
	    if (instance == null) {
			DontDestroyOnLoad(transform.gameObject);
	        instance = this;
	    } else {
	        Destroy(transform.gameObject);
	    }
	}
	
	public void Start () {
		
		nickname += Mathf.Floor(Random.value*1000);
		
		// load settings
		Preferences.load();
		expDetail = Preferences.getExplosionDetail(); // 0 (off) - 3 (max)
		chestDensity = Preferences.getChestDensity(); // 5 - 50
		mouseSensitivity = Preferences.getMouseSensitivity(); // 0f - 1f
		roundsToWin = Preferences.getRoundsToWin();

		nickname = PlayerPrefs.GetString("Player Name", nickname);
		serverName = PlayerPrefs.GetString("Server Name", serverName);
		maxPlayers = PlayerPrefs.GetInt("Server MaxPlayers", maxPlayers);
		playerColor.r = PlayerPrefs.GetFloat("PlayerRed", 0);
		playerColor.g = PlayerPrefs.GetFloat("PlayerGreen", 1);
		playerColor.b = PlayerPrefs.GetFloat("PlayerBlue", 0);
		playerColor.a = 1f;
		
		Static.setMenu(this);
		// play bg-music
		playSound(Static.selectRandomMusic(), true);
		gameObject.GetComponent<AudioSource>().volume = 0.7f;
		//gameObject.GetComponent<AudioSource>().volume = Preferences.getVolume();
		Static.player.updateMenuStats();
		
#if UNITY_IPHONE
		//scrnWidth = 640;
		scrnHeight = 360;
#endif
	    scrnWidth = (int)((float)Screen.width/(float)Screen.height * (float)scrnHeight);
	}
	
	public void playSound(AudioClip clip, bool loop) {
		if (loop) { // background music
			bool first = true;
			// clean sound devices
			foreach (AudioSource audioIterator in GetComponents<AudioSource>()) {
				if (first) {
					if (Preferences.getBackgroundMusic())
						audioIterator.enabled = true;
					else
						audioIterator.enabled = false;
					first = false;
				}
				else
					Destroy(audioIterator);
			}
		}
		/*	
		AudioSource audioSource;
		if (GetComponent<AudioSource>() != null) {
			audioSource = GetComponent<AudioSource>();
			audioSource.enabled = true;
			Debug.Log ("found");
		}
		else {
			audioSource = gameObject.AddComponent<AudioSource>();
			Debug.Log ("not found. dreating new");
		}
		*/
		
		AudioSource audioSource = GetComponent<AudioSource>();

		if (audioSource.isPlaying && !loop) {
			// neue Soundsource dazu
			foreach (AudioSource audioIterator in GetComponents<AudioSource>()) {
				if (!audioIterator.isPlaying) // entferne nicht mehr laufende
					Destroy(audioIterator);
			}
			audioSource = gameObject.AddComponent<AudioSource>();
		}
		audioSource.clip = clip;
		audioSource.loop = loop;
		audioSource.Play();
		//audioSource.volume = Preferences.getVolume();
	}

	
	void Update() {
		if ((Input.GetKeyDown(KeyCode.Plus)) || (Input.GetKeyDown(KeyCode.KeypadPlus))) {
			AudioSource audioSource = gameObject.GetComponent<AudioSource>();
			if (audioSource.volume < 1f) {
				audioSource.volume += 0.1f;
				//Preferences.setVolume(audioSource.volume);
			}
			Debug.Log("Audio volume set to " + audioSource.volume);
		}
		
		if ((Input.GetKeyDown(KeyCode.Minus)) || (Input.GetKeyDown(KeyCode.KeypadMinus))) {
			AudioSource audioSource = gameObject.GetComponent<AudioSource>();
			if (audioSource.volume > 0f) {
				audioSource.volume -= 0.1f;
				//Preferences.setVolume(audioSource.volume);
			}
			Debug.Log("Audio volume set to " + audioSource.volume);
		}
	}

	public void OnGUI () {
		
		if (!showGUI) {
			screen = "overlay";
			return;
		}
		
		GUI.matrix = Matrix4x4.Scale(new Vector3((float)Screen.width/scrnWidth, (float)Screen.height/scrnHeight, 1f));

		switch (screen) {
			case "join":
				joinScreen();
				break;
			case "waitingForStart":
				waitingForStartScreen();
				break;
				
			case "server":
				serverScreen();
				break;
				
			case "help":
				helpScreen();
				break;
			
			case "kicked":
				kickedScreen();
				return;
				
			case "start":
				if (Application.loadedLevelName != "StartMenu")
					Application.LoadLevel("StartMenu");
				return;
			
			default:
				startScreen();
				return;
		}
		if (Application.loadedLevelName == "Menu")
			backButton();
	}
	
	public void setScreen(string s) {
		switch (s) {
			case "join":
				// refresh server-list
		    	MasterServer.ClearHostList();
		        MasterServer.RequestHostList("BomberManUniTrier");
			break;

			case "server":
				// start up the server
			    var useNat = !Network.HavePublicAddress();
			    var err = Network.InitializeServer(maxPlayers-1, port, useNat);
			    while (err == NetworkConnectionError.CreateSocketOrThreadFailure) {
			    	port++;
				    err = Network.InitializeServer(maxPlayers-1, port, useNat);
			    }
			    regServerName = serverName;
			    MasterServer.RegisterHost("BomberManUniTrier", serverName, "a comment!");
				InvokeRepeating("refreshServerName", 5f, 3f);
				
				chat = "Welcome to Bomberman Galaxy.";
				// and add myself to the list of players
				networkView.RPC("newPlayer", RPCMode.AllBuffered, Network.player, nickname, playerColor.r, playerColor.g, playerColor.b);
			break;
			
			case "disconnect":
				Network.Disconnect();
				foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
					Destroy (p);
				Application.LoadLevel("StartMenu");
				s = "start";
			break;


		}
		screen = s;
	}
	public void returnToMenu() {
		setScreen("disconnect");
	}
	
	void OnLevelWasLoaded() {
		bool state = Application.loadedLevelName == "StartMenu";
		for (int i=1; i<transform.childCount; i++)
			transform.GetChild(i).gameObject.SetActive(state);
	}
	
	
	// CLIENT SIDE ONLY
	void OnConnectedToServer() {
		chat = "Welcome to Bomberman Galaxy.";
		// tell the others my nick and color
		networkView.RPC("newPlayer", RPCMode.OthersBuffered, Network.player, nickname, playerColor.r, playerColor.g, playerColor.b);
		playerList.Add(Network.player, nickname+" (me)");
		playerColorList.Add(Network.player, playerColor);
		networkView.RPC("incomingChatMessage", RPCMode.Others, nickname + " joined");
		screen = "waitingForStart";
	}
	void OnDisconnectedFromServer(NetworkDisconnection info) {
        if (Network.isServer) {
			MasterServer.UnregisterHost();
			screen = "start";
		} else {
            if (info == NetworkDisconnection.LostConnection) {
                Debug.Log("Lost connection to the server");
			} else {
                Debug.Log("Successfully diconnected from the server");
			}
			if (screen == "disconnected")
				screen = "start";
			else
				screen = "kicked";
		}
		Application.LoadLevel("Menu");
		showGUI = true;
		playerList.Clear();
		playerColorList.Clear();
		spawns.Clear();
		// delete all Players
		Static.inputHandler.lockCursor(false);
		Static.player.resetPlayers();
		foreach (var p in GameObject.FindGameObjectsWithTag("Player")) {
			Destroy(p);
		}
    }
	
	
	// SERVER SIDE ONLY
	void OnPlayerDisconnected(NetworkPlayer p) {
		Network.RemoveRPCs(p);
		Network.DestroyPlayerObjects(p);
		if (playerList.ContainsKey(p)) {
			networkView.RPC("removePlayer", RPCMode.OthersBuffered, p);
			networkView.RPC("incomingChatMessage", RPCMode.All, playerList[p] + " leaved");
			playerList.Remove(p);
			playerColorList.Remove(p);
			spawns.Remove(p);
			Static.player.imOut(p);
		}
	}
	void OnPlayerConnected(NetworkPlayer p) {
		int L_pos = Random.Range(1, Static.sphereHandler.n_L-2);
		int B_pos = Random.Range(1, Static.sphereHandler.n_B-1);
		networkView.RPC("tellSpawnPoint", RPCMode.AllBuffered, L_pos, B_pos, p);
				
		if (gameStarted) {
			networkView.RPC("tooLate", p);
		}
	}
	
	
	void joinScreen() {
		var width = 150;
		GUI.BeginGroup(new Rect(scrnWidth/2-width/2,10,width+75,500));
		
		// REFRESH BUTTON
	    if (GUI.Button(new Rect(0,10,width,20),"Refresh Serverlist")) {
		    MasterServer.updateRate = 3;
	    	MasterServer.ClearHostList();
	        MasterServer.RequestHostList("BomberManUniTrier");
	    }
				
	    // NICK
		GUI.Label(new Rect(0,50,width,20), "Nickname:");
		nickname = GUI.TextField(new Rect(0,70,width,20), nickname, 30);
	
		// COLOR
		Color c = GUI.color;
		GUI.color = playerColor;
		if (GUI.Button (new Rect(0,92, width,20), "Illumination Color")) {
			colorPicker.SetActive(!colorPicker.activeSelf);
		}
		GUI.color = c;
		
		if (!colorPicker.activeSelf) {
				
			// GRAPHIC-DETAILS
			GUI.Label(new Rect(0,120,width,20), "Bomb Shatter Detail:");
			GUI.Label(new Rect(0,145,50,20), "Min");
		    GUI.skin.label.alignment = TextAnchor.MiddleRight;
			GUI.Label(new Rect(width-40,145,40,20), "Max");
			expDetail = (int)GUI.HorizontalSlider (new Rect (0, 140, width, 20), (float)expDetail, 0.0f, 3.0f);
			string detailLevel = "";
			switch (expDetail) {
			case 0:
				GUI.skin.label.normal.textColor = Color.grey;
				detailLevel = "off";
				break;
			case 1:
				GUI.skin.label.normal.textColor = Color.green;
				detailLevel = "low";
				break;
			case 2:
				GUI.skin.label.normal.textColor = Color.yellow;
				detailLevel = "moderate";
				break;
			case 3:
				GUI.skin.label.normal.textColor = Color.red;
				detailLevel = "high";
				break;
			}
		    GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			GUI.Label(new Rect(width/2-30,145,60,20), detailLevel);
			GUI.skin.label.normal.textColor = Color.white;
		    GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			
			GUI.Label(new Rect(0,170,width,20), "Mouse Sensitivity:");
			GUI.Label(new Rect(0,195,50,20), "Min");
		    GUI.skin.label.alignment = TextAnchor.MiddleRight;
			GUI.Label(new Rect(width-40,195,50,20), "Max");
		    GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			mouseSensitivity = (int) GUI.HorizontalSlider (new Rect (0, 190, width, 20), mouseSensitivity, 1, 10);
			
			Preferences.setExplosionDetail(expDetail);
			Preferences.setMouseSensitivity(mouseSensitivity);
			
			bool pe = Preferences.getBackgroundMusic();
			if (pe != GUI.Toggle(new Rect(0,230,width,20), pe, "Background Music")) {
				Preferences.setBackgroundMusic(!pe);
			}
			
			// SERVER LIST
			HostData[] servers = MasterServer.PollHostList();
			int i = 1;
			foreach (HostData srv in servers) {
				var name = srv.gameName + " " + srv.connectedPlayers + "/" + srv.playerLimit;
				GUI.Label(new Rect(0,260+25*i,width,20), name);
				/*var hostInfo : String = "[";
				for (var host in srv.ip)
					hostInfo = hostInfo + host + ":" + srv.port + " ";
				hostInfo = hostInfo + "]";
				GUILayout.Label(hostInfo);
				//GUILayout.Label(srv.comment);*/
				if (GUI.Button(new Rect(width,260+25*i,75,20), "Connect")) {
					if (nickname.Length > 0) {
						PlayerPrefs.SetString("Player Name", nickname);
						PlayerPrefs.SetFloat("PlayerRed", playerColor.r);
						PlayerPrefs.SetFloat("PlayerGreen", playerColor.g);
						PlayerPrefs.SetFloat("PlayerBlue", playerColor.b);
						
						Network.Connect(srv);
					}
				}
				i++;
			}
			// NO SERVERS
			if (i == 1) {
				GUI.Box(new Rect(0,270,width,24), "no servers found");
			}
		}
		
		GUI.EndGroup();
	}

	void waitingForStartScreen() {
		// display a chat and the list of currently conntected players
		GUI.BeginGroup(new Rect(10, 100, 200, 400));
		GUILayout.Label("Connected Players:");
		Color c = GUI.contentColor;
		foreach (var p in playerList) {
			GUI.contentColor = playerColorList[p.Key];
            GUILayout.Label(p.Value);
		}
		GUI.contentColor = c;
		GUI.EndGroup();
		
		GUI.Box(new Rect(scrnWidth/2-100, 50, 200, 24), "Waiting for server to start game");
		chatArea();
	}


	private bool showMaxPlayers = false;
	void serverScreen() {
		int width = 150;
		GUI.BeginGroup(new Rect(scrnWidth/2-200, 50, 200, 900));
		// NICK
		GUI.Label(new Rect(25,0,width,20), "Nickname:");
		nickname = GUI.TextField(new Rect(25,20,width,20), nickname, 30);
		
		// COLOR
		Color c = GUI.color;
		GUI.color = playerColor;
		if (GUI.Button (new Rect(25,40, width,20), "Illumination Color")) {
			colorPicker.SetActive(!colorPicker.activeSelf);
		}
		GUI.color = c;


		if (!colorPicker.activeSelf) {
		
			// GRAPHIC-DETAILS
			GUI.Label(new Rect(25,70,width,20), "Bomb Shatter Detail:");
			GUI.Label(new Rect(25,95,50,20), "Min");
		    GUI.skin.label.alignment = TextAnchor.MiddleRight;
			GUI.Label(new Rect(width-15,95,50,20), "Max");
			expDetail = (int)GUI.HorizontalSlider (new Rect (25, 90, width, 20), (float)expDetail, 0.0f, 3.0f);
			string detailLevel = "";
			switch (expDetail) {
			case 0:
				GUI.skin.label.normal.textColor = Color.grey;
				detailLevel = "off";
				break;
			case 1:
				GUI.skin.label.normal.textColor = Color.green;
				detailLevel = "low";
				break;
			case 2:
				GUI.skin.label.normal.textColor = Color.yellow;
				detailLevel = "moderate";
				break;
			case 3:
				GUI.skin.label.normal.textColor = Color.red;
				detailLevel = "high";
				break;
			}
		    GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			GUI.Label(new Rect(width/2,95,60,20), detailLevel);
			GUI.skin.label.normal.textColor = Color.white;
		    GUI.skin.label.alignment = TextAnchor.MiddleLeft;

			GUI.Label(new Rect(25,120,width,20), "Mouse Sensitivity:");
			GUI.Label(new Rect(25,145,50,20), "Min");
		    GUI.skin.label.alignment = TextAnchor.MiddleRight;
			GUI.Label(new Rect(width-15,145,50,20), "Max");
		    GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			mouseSensitivity = (int) GUI.HorizontalSlider (new Rect (25, 140, width, 20), mouseSensitivity, 1, 10);
			
			bool pe = Preferences.getBackgroundMusic();
			if (pe != GUI.Toggle(new Rect(25,180,width,20), pe, "Background Music")) {
				Preferences.setBackgroundMusic(!pe);
			}
		}
		GUI.EndGroup();
		
		
		GUI.BeginGroup(new Rect(scrnWidth/2,50,200,900));
		
		// SERVER NAME
		GUI.Label(new Rect(25,0,width,20), "Server Name:");
		serverName = GUI.TextField(new Rect(25,20,width,20), serverName, 30);
		
		// MAX PLAYERS
		if (GUI.Button(new Rect(25, 40, width, 20), "Max. Players: "+maxPlayers)) {
			showMaxPlayers = !showMaxPlayers;
	    }
		
		if (!colorPicker.activeSelf) {
		    if (showMaxPlayers) {
				
				for (int i=1; i<5; i++) {
					int p = (int)Mathf.Pow(2,i);
					if (Network.connections.Length+1 > p)
						continue;
					if (GUI.Button(new Rect(25, 40+(20*i), width, 20), p.ToString())) {
						showMaxPlayers = false;
						maxPlayers = p;
						Network.maxConnections = maxPlayers-1;
					}
				}
		    } else {
				// SOME SETTINGS
				bool pe = Preferences.getNegativePowerups();
				if (pe != GUI.Toggle(new Rect(25,70,width,20), pe, "Powerdowns")) {
					Preferences.setNegative(!pe);
				}
				pe = Preferences.getDestroyablePowerups();
				if (pe != GUI.Toggle(new Rect(25,90,width,20), pe, "destroyable Powerups")) {
					Preferences.setDestroyablePowerups(!pe);
				}
				bool pee = Preferences.getExplodingPowerups();
				if (pe && pee != GUI.Toggle(new Rect(25,110,width,20), pee, "exploding Powerups")) {
					Preferences.setExplodingPowerups(!pee);
				}
			}

			GUI.Label(new Rect(25,140,width,20), "Chest Density:");
			GUI.Label(new Rect(25,165,50,20), "Min");
		    GUI.skin.label.alignment = TextAnchor.MiddleRight;
			GUI.Label(new Rect(width-15,165,50,20), "Max");
		    GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			chestDensity = (int)GUI.HorizontalSlider (new Rect (25, 160, width, 20), (float)chestDensity, 1.0f, 5.0f);

			GUI.Label(new Rect(25,190,width,20), "Rounds To Win: " + (roundsToWin == 0 ? "endless" : roundsToWin.ToString()));
			GUI.Label(new Rect(25,215,50,20), "Min");
		    GUI.skin.label.alignment = TextAnchor.MiddleRight;
			GUI.Label(new Rect(width-15,215,50,20), "Max");
		    GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			roundsToWin = (int) GUI.HorizontalSlider (new Rect (25, 210, width, 20), roundsToWin, 0, 10);
			
			Preferences.setExplosionDetail(expDetail);
			Preferences.setChestDensity(chestDensity);
			Preferences.setMouseSensitivity(mouseSensitivity);
			Preferences.setRoundsToWin(roundsToWin);
			
		}
		GUI.EndGroup();
		
	    // START GAME
		if (!colorPicker.activeSelf && GUI.Button(new Rect(scrnWidth/2-50,300,100,30),"Start Game")) {
			// save settings
			PlayerPrefs.SetInt("Server MaxPlayers", maxPlayers);
			PlayerPrefs.SetString("Server Name", serverName);
			PlayerPrefs.SetString("Player Name", nickname);
			PlayerPrefs.SetFloat("PlayerRed", playerColor.r);
			PlayerPrefs.SetFloat("PlayerGreen", playerColor.g);
			PlayerPrefs.SetFloat("PlayerBlue", playerColor.b);
			
			showGUI = false;
			gameStarted = true;
			
			CancelInvoke("refreshServerName");

			// spawn-points
			int L_pos = Random.Range(1, Static.sphereHandler.n_L-2);
			int B_pos = Random.Range(1, Static.sphereHandler.n_B-1);
			networkView.RPC("tellSpawnPoint", RPCMode.AllBuffered, L_pos, B_pos, Network.player);
			networkView.RPC("startGame",RPCMode.AllBuffered, Mathf.FloorToInt(Random.value*100000), Preferences.getChestDensity());
		}
		
		// CONNECTED PLAYERS
		GUI.BeginGroup(new Rect(10,50,250,400));
		GUI.Label(new Rect(0,0,200,20), "Connected Players:");
		if (playerList.Count == 0)
			GUI.Label(new Rect(10,20,190,20), "no player connected");
			
		int j = 0;
		Dictionary<NetworkPlayer,string> tpl = playerList;
		foreach (var p in tpl) {
			
			GUI.color = playerColorList[p.Key];
			GUI.Label(new Rect(20,j*24+20,200,24), p.Value);
 
			GUI.color = Color.red;
			if (p.Key != Network.player && GUI.Button(new Rect(0,j*24+21,18,15), "x")) {
				Network.CloseConnection(p.Key, true);
				networkView.RPC("removePlayer", RPCMode.OthersBuffered, p.Key);
				networkView.RPC("incomingChatMessage", RPCMode.All, p.Value + " was kicked");
				playerList.Remove(p.Key);
				playerColorList.Remove(p.Key);
				spawns.Remove(p.Key);
			}
			GUI.color = Color.white;
			j++;
		}
		GUI.EndGroup();
		
		// CHAT
		chatArea();
	}
	
	void kickedScreen() {
		//guiText.fontSize = 24;
		GUIStyle s = new GUIStyle();
	    s.fontSize = 50;
		s.alignment = TextAnchor.MiddleCenter;
		s.normal.textColor = Color.white;
		GUI.Label(new Rect(scrnWidth/2-150, 100, 300, 100), "Server disconnected!", s);
		
		if (GUI.Button(new Rect(scrnWidth/2-50,270,100,30), "continue")) {
			screen = "start";
		}
	}

	void startScreen() {
		var width = 150;
		var height = 30;
		var x = scrnWidth/2 - width/2;
		var y = scrnHeight/2 - 2*height;
		
		if (Network.peerType != NetworkPeerType.Disconnected) {
			// we are ingame!
			if (GUI.Button(new Rect(x,y,width,height), "Disconnect")) {
				setScreen("disconnect");
			}
		} else {
			if (GUI.Button(new Rect(x,y,width,height), "Join Server")) {
				setScreen("join");
			}
			if (GUI.Button(new Rect(x,y+height,width,height), "Start new Server")) {
				setScreen("server");
			}
		}
		if (GUI.Button(new Rect(x,y+2*height,width,height), "How to Play")) {
			// show help menu
			setScreen("help");
		}
		if (GUI.Button(new Rect(x,y+3*height,width,height), "Exit")) {
			// exit game
			Application.Quit();
		}
	}
	
	void helpScreen() {
		GUI.DrawTexture(new Rect(0,0,scrnWidth,scrnHeight), bgHelp, ScaleMode.StretchToFill, true, 10.0f);
	}
	
	void backButton() {
		if (GUI.Button(new Rect(10,scrnHeight-40,80,30), "Back") || Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape) {
			if (colorPicker.activeSelf) {
				colorPicker.SetActive(!colorPicker.activeSelf);
			} else {
				if (screen == "server") { // cancel the server
					Network.Disconnect();
					MasterServer.UnregisterHost();
					CancelInvoke("refreshServerName");
				} else if (screen == "waitingForStart") {
					Network.Disconnect();
				}
				screen = "start";
			}
		}
	}

	
	// check if serverName changed
	private string regServerName, regNick;
	void refreshServerName() {
		if (serverName != regServerName) {
		    MasterServer.RegisterHost("BomberManUniTrier", serverName, "a comment!");
		    regServerName = serverName;
		}
		if (nickname != regNick && nickname.Trim().Length > 1) {
			playerList[Network.player] = nickname + " (me)";
			regNick = nickname;
			networkView.RPC("newPlayer", RPCMode.AllBuffered, Network.player, nickname, playerColor.r, playerColor.g, playerColor.b);
		}
	}
	
	private string chatMsg = "";
	public void chatArea() {
		int x = scrnWidth - 230;
		
	    GUI.skin.label.alignment = TextAnchor.LowerLeft;
		GUI.Label(new Rect(x,30,150,scrnHeight-70), chat);
	    GUI.skin.label.alignment = TextAnchor.MiddleLeft;

		GUI.SetNextControlName("chat");
		chatMsg = GUI.TextField(new Rect(x,scrnHeight-40,150,20), chatMsg);
		if (((GUI.Button(new Rect(x+160,scrnHeight-40,50,20), "Send")
				|| Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter))
				&& chatMsg.Trim().Length > 0 && nickname.Trim().Length > 0) {
			networkView.RPC("incomingChatMessage", RPCMode.All, nickname + ": " + chatMsg);
			chatMsg = "";
			inGame.focusToChat = true;
		}
	}
	
	public static void setPlayerColor(Color color) {
        playerColor = color;
		if (playerColorList.ContainsKey(Network.player))
			playerColorList[Network.player] = color;
	}
	public static Color getPlayerColor(NetworkPlayer p) {
		if (playerList.ContainsKey(p))
	        return playerColorList[p];
		return Color.white;
	}
	public static string getPlayerNick(NetworkPlayer p) {
		if (playerList.ContainsKey(p))
			return playerList[p];
		return "";
	}
	public static bool isInGame() {
		return !showGUI;
	}
	
	
	[RPC]
	public void incomingChatMessage(string text) {
		chat += "\n" + text;
		// max x lines
		if (chat.Replace("\n","").Length + 100 <= chat.Length) {
			chat = chat.Substring(chat.IndexOf('\n'));
		}
		if (!showGUI)
			Invoke("removeChatLine", 10);
	}
	public void removeChatLine() {
		if (chat.Length > 1 && chat.IndexOf('\n',1) > 0) {
			chat = chat.Substring(chat.IndexOf('\n',1));
		} else {
			chat = "";
		}
	}

	[RPC]
	public void newPlayer(NetworkPlayer p, string nick, float r, float g, float b) {
		if (playerList.ContainsKey(p))
			playerList[p] = nick;
		else
			playerList.Add(p, nick + (Network.player == p ? " (me)" : ""));
		
		if (playerColorList.ContainsKey(p))
			playerColorList[p] = new Color(r,g,b);
		else
			playerColorList.Add(p, new Color(r,g,b));
	}
	[RPC]
	public void removePlayer(NetworkPlayer p) {
		playerList.Remove(p);
		playerColorList.Remove(p);
		spawns.Remove(p);
		Static.player.imOut(p);
	}
	[RPC]
	public void tooLate() {
		gameStarted = true;
	}

	[RPC]
	public void startGame(int seed, int chestDensity) {
		Static.player.resetStats();
		foreach (var p in GameObject.FindGameObjectsWithTag("Player"))
			Destroy(p);
		Static.player.setPlayers(new List<NetworkPlayer>(spawns.Keys));
		
		rSeed = seed;
		Random.seed = seed;
		Preferences.setChestDensity(chestDensity);
		Application.LoadLevel("SphereCreate");
		showGUI = false;
		chat = "";
		incomingChatMessage("Game started. Have Fun!");
	}
	
	// SERVER only
	public void startRound() {
		List<NetworkPlayer> playerAlive = Static.player.getPlayersAlive();
		if (playerAlive.Count == 1) {
			NetworkPlayer winner = playerAlive[0];
			networkView.RPC("incomingChatMessage",RPCMode.All,Menu.getPlayerNick(winner) + " has won this round!");
			
			bool gameOver = Static.player.getWinAmount(winner) == Preferences.getRoundsToWin() - 1;
			if (gameOver) {
				networkView.RPC("incomingChatMessage",RPCMode.All, getPlayerNick(winner) + " has won this match!");
				networkView.RPC("incomingChatMessage",RPCMode.All, "Game over. You'll return to menu in 15 seconds.");
			}
			networkView.RPC("winner", RPCMode.AllBuffered, winner, gameOver, false);
		} else {
			networkView.RPC("incomingChatMessage", RPCMode.All, "Round draw!");
			networkView.RPC("winner", RPCMode.AllBuffered, Network.player, false, true);
		}
	}
	private void startNext() {
		networkView.RPC("startGame", RPCMode.All, Mathf.FloorToInt(Random.value*100000), Preferences.getChestDensity());
	}
	
	[RPC]
	public void winner(NetworkPlayer p, bool gameOver, bool draw) {
		if (!draw)
			Static.player.setWinner(p);
		if (gameOver)
			Invoke("returnToMenu", 15);
		else {
			inGame.startCounter(5);
			Invoke("startNext", 5);
		}
	}
	
	[RPC]
	public void tellSpawnPoint(int l, int b, NetworkPlayer p) {
		if (spawns.ContainsKey(p))
			spawns.Remove(p);
		spawns.Add(p, new int[] {l,b});
	}

}
