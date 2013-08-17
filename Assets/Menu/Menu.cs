using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Menu : MonoBehaviour {

	public Texture bgHelp; // Picture of Help-Menu
	
	public GameObject colorPicker;
	
	private string screen = "start";

	public string serverName = "Galaxy 42";
	private int port = 25000;
	public int maxPlayers = 4;

	private string nickname = "Player ";
	private static Color playerColor;
	private int expDetail = 5; // 1-10
	private string chat = "";
	
	private Dictionary<NetworkPlayer,string> playerList = new Dictionary<NetworkPlayer, string>();
	public static bool showGUI = true;
	
	private static bool created = false;
	public void Awake() {
	    if (!created) {
			DontDestroyOnLoad(transform.gameObject);
	        created = true;
	    } else {
	        Destroy(transform.gameObject);
	    } 
	}
	
	public void Start () {
		nickname += Mathf.Floor(Random.value*1000);
		
		// load settings
		nickname = PlayerPrefs.GetString("Player Name", nickname);
		serverName = PlayerPrefs.GetString("Server Name", serverName);
		maxPlayers = PlayerPrefs.GetInt("Server MaxPlayers", maxPlayers);
		playerColor.r = PlayerPrefs.GetFloat("PlayerRed", 0);
		playerColor.g = PlayerPrefs.GetFloat("PlayerGreen", 1);
		playerColor.b = PlayerPrefs.GetFloat("PlayerBlue", 0);
		playerColor.a = 1f;
	}


	public void OnGUI () {
		
		if (!showGUI) {
			screen = "start";
			return;
		}
		
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
				
			default:
				startScreen();
				return;
		}
		backButton();
	}
	
	
	// CLIENT SIDE ONLY
	void OnConnectedToServer() {
		
		chat = "Welcome to Bomberman Galaxy.";
		// tell the others my nick
		networkView.RPC("newPlayer", RPCMode.OthersBuffered, nickname, Network.player);
		playerList.Add(Network.player, nickname+" (me)");
		networkView.RPC("incommingChatMessage", RPCMode.All, nickname + " joined");
		screen = "waitingForStart";
	}
	void OnDisconnectedFromServer(NetworkDisconnection info) {
        if (Network.isServer) {
            Debug.Log("Local server connection disconnected");
		} else {
            if (info == NetworkDisconnection.LostConnection) {
                Debug.Log("Lost connection to the server");
			} else {
                Debug.Log("Successfully diconnected from the server");
			}
			Application.LoadLevel(0);
			screen = "kicked";
			showGUI = true;
		}
		playerList = new Dictionary<NetworkPlayer,string>();
    }
	
	
	// SERVER SIDE ONLY
	void OnPlayerDisconnected(NetworkPlayer p) {
		Network.RemoveRPCs(p);
		//Network.DestroyPlayerObjects(p);
		networkView.RPC("removePlayer", RPCMode.OthersBuffered, p);
		networkView.RPC("incommingChatMessage", RPCMode.All, playerList[p] + " leaved");
		playerList.Remove(p);
	}
	
	
	void joinScreen() {
		var width = 150;
		GUI.BeginGroup(new Rect(Screen.width/2-width/2,10,width+75,500));
		
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
		
		// GRAPHIC-DETAILS
		if (!colorPicker.activeSelf) {
				
			GUI.Label(new Rect(0,120,width,20), "Effect Details:");
			GUI.Label(new Rect(0,145,50,20), "Min");
		    GUI.skin.label.alignment = TextAnchor.MiddleRight;
			GUI.Label(new Rect(width-40,145,40,20), "Max");
		    GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			expDetail = (int)GUI.HorizontalSlider (new Rect (0, 140, width, 20), (float)expDetail, 1.0f, 10.0f);
			Preferences.setExplosionDetail(11-expDetail);
	
			// SERVER LIST
			HostData[] servers = MasterServer.PollHostList();
			int i = 1;
			foreach (HostData srv in servers) {
				var name = srv.gameName + " " + srv.connectedPlayers + "/" + srv.playerLimit;
				GUI.Label(new Rect(0,170+25*i,width,20), name);
				/*var hostInfo : String = "[";
				for (var host in srv.ip)
					hostInfo = hostInfo + host + ":" + srv.port + " ";
				hostInfo = hostInfo + "]";
				GUILayout.Label(hostInfo);
				//GUILayout.Label(srv.comment);*/
				if (GUI.Button(new Rect(width,170+25*i,75,20), "Connect")) {
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
				GUI.Box(new Rect(0,200,width,24), "no servers found");
			}
		}
		
		GUI.EndGroup();
	}

	void waitingForStartScreen() {
		// display a chat and the list of currently conntected players
		GUI.BeginGroup(new Rect(10, 100, 200, 400));
		GUILayout.Label("Connected Players:");
		foreach (var p in playerList)
            GUILayout.Label(p.Value);
		GUI.EndGroup();
		
		GUI.Box(new Rect(Screen.width/2-100, 50, 200, 24), "Waiting for server to start game");
		chatArea(-1);
	}


	private bool showMaxPlayers = false;
	void serverScreen() {
		int width = 150;
		GUI.BeginGroup (new Rect(Screen.width/2-100, 50, 200, 500));
		// NICK
		GUI.Label(new Rect(25,10,width,20), "Nickname:");
		nickname = GUI.TextField(new Rect(25,30,width,20), nickname, 30);
		
		// COLOR
		Color c = GUI.color;
		GUI.color = playerColor;
		if (GUI.Button (new Rect(25,52, width,20), "Illumination Color")) {
			colorPicker.SetActive(!colorPicker.activeSelf);
		}
		GUI.color = c;


		if (!colorPicker.activeSelf) {
		
			// GRAPHIC-DETAILS
			GUI.Label(new Rect(25,80,width,20), "Effect-Details:");
			GUI.Label(new Rect(25,105,50,20), "Min");
		    GUI.skin.label.alignment = TextAnchor.MiddleRight;
			GUI.Label(new Rect(width-15,105,50,20), "Max");
		    GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			expDetail = (int)GUI.HorizontalSlider (new Rect (25, 100, width, 20), (float)expDetail, 1.0f, 10.0f);
			Preferences.setExplosionDetail(11-expDetail);
		
			// SERVER NAME
			GUI.Label(new Rect(25,150,width,20), "Server Name:");
			serverName = GUI.TextField(new Rect(25,170,width,20), serverName, 30);
			
			// MAX PLAYERS
			if (GUI.Button(new Rect(25, 190, width, 20), "Max. Players: "+maxPlayers)) {
				showMaxPlayers = !showMaxPlayers;
		    }
		    if (showMaxPlayers) {
				
				for (int i=1; i<5; i++) {
					int p = (int)Mathf.Pow(2,i);
					if (Network.connections.Length+1 > p)
						continue;
					if (GUI.Button(new Rect(25, 190+(20*i), width, 20), p.ToString())) {
						showMaxPlayers = false;
						maxPlayers = p;
						Network.maxConnections = maxPlayers-1;
					}
				}
		    } else {
				// SOME SETTINGS
				bool pe = Preferences.getNegativePowerups();
				if (pe != GUI.Toggle(new Rect(25,220,width,20), pe, "negative Powerups")) {
					Preferences.setNegative(!pe);
				}
				pe = Preferences.getDestroyablePowerups();
				if (pe != GUI.Toggle(new Rect(25,240,width,20), pe, "destroyable Powerups")) {
					Preferences.setDestroyablePowerups(!pe);
				}
				bool pee = Preferences.getExplodingPowerups();
				if (pe && pee != GUI.Toggle(new Rect(25,260,width,20), pee, "exploding Powerups")) {
					Preferences.setExplodingPowerups(!pee);
				}
			}
			
		    // START GAME
			if (GUI.Button(new Rect(50,340,100,30),"Start Game")) {
				// save settings
				PlayerPrefs.SetInt("Server MaxPlayers", maxPlayers);
				PlayerPrefs.SetString("Server Name", serverName);
				PlayerPrefs.SetString("Player Name", nickname);
				PlayerPrefs.SetFloat("PlayerRed", playerColor.r);
				PlayerPrefs.SetFloat("PlayerGreen", playerColor.g);
				PlayerPrefs.SetFloat("PlayerBlue", playerColor.b);
				
				showGUI = false;
				
				networkView.RPC("startGame",RPCMode.AllBuffered, (int)Random.value*100000);
			}
		}
		GUI.EndGroup();
		
		// CONNECTED PLAYERS
		GUI.BeginGroup(new Rect(10,50,250,400));
		GUI.Label(new Rect(0,0,200,20), "Connected Players:");
		if (playerList.Count == 0)
			GUI.Label(new Rect(10,20,190,20), "no player connected");
			
		int j = 0;
		Dictionary<NetworkPlayer,string> tpl = playerList;
		foreach (var p in tpl) {
			GUI.Label(new Rect(20,j*24+20,200,24), p.Value);
			GUI.color = new Color(255,0,0);
			if (GUI.Button(new Rect(0,j*24+21,18,15), "x")) {
				Network.CloseConnection(p.Key, true);
				networkView.RPC("removePlayer", RPCMode.OthersBuffered, p.Key);
				networkView.RPC("incommingChatMessage", RPCMode.All, p.Value + " was kicked");
				playerList.Remove(p.Key);
			}
			GUI.color = Color.white;
			j++;
		}
		GUI.EndGroup();
		
		// CHAT
		chatArea(-1);
	}
	
	void kickedScreen() {
		//guiText.fontSize = 24;
		GUIStyle s = new GUIStyle();
	    s.fontSize = 50;
		s.alignment = TextAnchor.MiddleCenter;
		GUI.Label(new Rect(Screen.width/2-150, 100, 300, 100), "Server disconnected!", s);
		playerList = new Dictionary<NetworkPlayer,string>();
		
		if (GUI.Button(new Rect(Screen.width/2-50,270,100,30), "continue")) {
			screen = "start";
		}
	}

	void startScreen() {
		var width = 150;
		var height = 30;
		var x = Screen.width/2 - width/2;
		var y = Screen.height/2 - 2*height;
		
		if (Network.peerType != NetworkPeerType.Disconnected) {
			// we are ingame!
			if (GUI.Button(new Rect(x,y,width,height), "Disconnect")) {
				Network.Disconnect();
				if (Network.isServer) {
					MasterServer.UnregisterHost();
					CancelInvoke("refreshServerName");
				}
				Application.LoadLevel(0);
			}
		} else {
			if (GUI.Button(new Rect(x,y,width,height), "Join Server")) {
				// show join menu
				screen = "join";
				// refresh server-list
		    	MasterServer.ClearHostList();
		        MasterServer.RequestHostList("BomberManUniTrier");
			}
			if (GUI.Button(new Rect(x,y+height,width,height), "Start new Server")) {
				// show new-server menu
				screen = "server";
				// and start up the server
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
				//playerList.Add(nickname);
			}
		}
		if (GUI.Button(new Rect(x,y+2*height,width,height), "How to Play")) {
			// show help menu
			screen = "help";
		}
		if (GUI.Button(new Rect(x,y+3*height,width,height), "Exit")) {
			// exit game
			Application.Quit();
		}
	}
	
	void helpScreen() {
		GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height), bgHelp, ScaleMode.StretchToFill, true, 10.0f);
	}
	
	void backButton() {
		if (GUI.Button(new Rect(10,Screen.height-40,80,30), "Back")) {
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

	
	// check if serverName changed
	private string regServerName;
	void refreshServerName() {
		if (serverName != regServerName) {
		    MasterServer.RegisterHost("BomberManUniTrier", serverName, "a comment!");
		    regServerName = serverName;
		}
	}
	
	private string chatMsg = "";
	public void chatArea(int time2show) {
		int x = Screen.width - 230;
		
	    GUI.skin.label.alignment = TextAnchor.LowerLeft;
		GUI.Label(new Rect(x,30,150,Screen.height-70), chat);
	    GUI.skin.label.alignment = TextAnchor.MiddleLeft;

		chatMsg = GUI.TextField(new Rect(x,Screen.height-40,150,20), chatMsg);
		if ((GUI.Button(new Rect(x+160,Screen.height-40,50,20), "Send")
				|| Event.current.keyCode == KeyCode.Return)
				&& chatMsg.Length > 0) {
			networkView.RPC("incommingChatMessage", RPCMode.All, nickname + ": " + chatMsg);
			chatMsg = "";
		}
	}
	
	public static void setPlayerColor(Color color) {
        playerColor = color;
	}
	public static Color getPlayerColor() {
        return playerColor;
	}
	public static bool isInGame() {
		return !showGUI;
	}
	
	
	[RPC]
	public void incommingChatMessage(string text, NetworkMessageInfo info) {
		// max x lines
		int lines = (int) (Screen.height / GUI.skin.label.lineHeight);
		chat += "\n" + text;
		if (chat.Replace("\n","").Length + lines <= chat.Length) {
			chat = chat.Substring(chat.IndexOf('\n'));
		}
	}

	[RPC]
	public void newPlayer(string nick, NetworkPlayer p) {
		playerList.Add(p, nick);
	}
	[RPC]
	public void removePlayer(NetworkPlayer p) {
		playerList.Remove(p);
	}	
	
	[RPC]
	void startGame(int seed) {
		Random.seed = seed;
		Application.LoadLevel(1);
		showGUI = false;
	}
	
}
