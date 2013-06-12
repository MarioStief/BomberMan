#pragma strict


var bgHelp : Texture; // Picture of Help-Menu

private var screen : String;

var serverName : String = "Galaxy 42";
private var port   : int = 25000;
var maxPlayers  : int = 4;

private var nickname : String = "Player 42";
private var chat : String = "";

private var players = new Array();

function Start () {
	// load settings
	if (PlayerPrefs.GetString("Player Name"))
		nickname = PlayerPrefs.GetString("Player Name");
	if (PlayerPrefs.GetString("Server Name"))
		serverName = PlayerPrefs.GetString("Server Name");
	if (PlayerPrefs.GetInt("Server MaxPlayers"))
		maxPlayers = PlayerPrefs.GetInt("Server MaxPlayers");
}

function Update () {
}

function OnGUI () {
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
			
		default:
			startScreen();
			return;
	}
	backButton();
}

function OnConnectedToServer() {
	networkView.RPC("newPlayer", RPCMode.All, nickname);
	networkView.RPC("incommingChatMessage", RPCMode.All, nickname + " joined");
	screen = "waitingForStart";
}

function joinScreen() {
	var width = 150;
	GUI.BeginGroup(Rect(Screen.width/2-width/2,10,width+75,500));
	
	// REFRESH BUTTON
    if (GUI.Button(Rect(0,10,width,20),"Refresh Serverlist")) {
	    MasterServer.updateRate = 3;
    	MasterServer.ClearHostList();
        MasterServer.RequestHostList("BomberManUniTrier");
    }

    // NICK
	GUI.Label(Rect(0,50,width,20), "Nickname:");
	nickname = GUI.TextField(Rect(0,70,width,20), nickname, 30);

	// SERVER LIST
	var servers : HostData[] = MasterServer.PollHostList();
	var i = 1;
	for (var srv in servers) {
		var name = srv.gameName + " " + srv.connectedPlayers + "/" + srv.playerLimit;
		GUI.Label(Rect(0,100+25*i,width,20), name);
		/*var hostInfo : String = "[";
		for (var host in srv.ip)
			hostInfo = hostInfo + host + ":" + srv.port + " ";
		hostInfo = hostInfo + "]";
		GUILayout.Label(hostInfo);
		//GUILayout.Label(srv.comment);*/
		if (GUI.Button(Rect(width,100+25*i,75,20), "Connect")) {
			if (nickname.Length > 0) {
				try {
					PlayerPrefs.SetString("Player Name", nickname);
					Network.Connect(srv);
				} catch(err) {
					Debug.Log("Server unreachable!");
				}
			}
		}
		i++;
	}
	
	// NO SERVERS
	if (i == 1) {
		GUI.Box(Rect(0,200,width,24), "no servers found");
	}
	
	GUI.EndGroup();
}

private var chatMsg : String = "";
function waitingForStartScreen() {
	// display a chat and the list of currently conntected players
	// ..
	GUI.Box(Rect(Screen.width/2-100, 50, 200, 24), "Waiting for server to start game");
	chatArea(Screen.width-210, 100);
}

@RPC
function incommingChatMessage(text : String, info: NetworkMessageInfo) {
	// max 18 lines
	if (chat.Replace("\n","").Length + 18 <= chat.Length) {
		var lines = new Array();
		lines = chat.Split("\n"[0]);
		lines.shift();
		chat = lines.join("\n");
	}
	chat += "\n" + text;
	Debug.Log(text + " from: "+info.sender);
}

@RPC
function startGame() {
	Application.LoadLevel(1);
}

@RPC
function newPlayer(nick : String, info: NetworkMessageInfo) {
	Debug.Log("new player (" + nick + ") connected");
	
	//players.Push(nick);
}

private var showMaxPlayers = false;
function serverScreen() {

	GUI.BeginGroup (Rect(Screen.width/2-100, Screen.height/2-250, 200, 500));
	// NICK
	GUI.Label(Rect(25,10,150,20), "Nickname:");
	nickname = GUI.TextField(Rect(25,30,150,20), nickname, 30);
	
	// SERVER NAME
	GUI.Label(Rect(25,110,150,20), "Server Name:");
	serverName = GUI.TextField(Rect(25,130,150,20), serverName, 30);
	
	// MAX PLAYERS
	//GUI.Label(Rect(10,100,150,20), "Max. Players:");
	if (GUI.Button(Rect(25, 170, 150, 20), "Max. Players: "+maxPlayers)) {
		showMaxPlayers = !showMaxPlayers;
    }
    if (showMaxPlayers) {
		for (var i=1; i<5; i++) {
			if (GUI.Button(Rect(25, 170+(20*i), 150, 20), Mathf.Pow(2,i).ToString())) {
				showMaxPlayers = false;
				maxPlayers = Mathf.Pow(2,i);
				Network.maxConnections = maxPlayers-1;
			}
		}
    }
    
    // START SERVER
	if (GUI.Button(Rect(50,300,100,30),"Start Game")) {
		// save settings
		PlayerPrefs.SetInt("Server MaxPlayers", maxPlayers);
		PlayerPrefs.SetString("Server Name", serverName);
		PlayerPrefs.SetString("Player Name", nickname);
		
		networkView.RPC("startGame", RPCMode.AllBuffered);
	    //Application.LoadLevel(1);
	}
	GUI.EndGroup();
	
	// CONNECTED PLAYERS
	// ...
	
	// CHAT
	chatArea(Screen.width-210, 10);
}

function startScreen() {
	var width = 150;
	var height = 30;
	var x = Screen.width/2 - width/2;
	var y = Screen.height/2 - 2*height;
	
	if (GUI.Button(Rect(x,y,width,height), "Join Server")) {
		// show join menu
		screen = "join";
		// refresh server-list
    	MasterServer.ClearHostList();
        MasterServer.RequestHostList("BomberManUniTrier");
	}
	if (GUI.Button(Rect(x,y+height,width,height), "Start new Server")) {
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
	}
	if (GUI.Button(Rect(x,y+2*height,width,height), "How to Play")) {
		// show help menu
		screen = "help";
	}
	if (GUI.Button(Rect(x,y+3*height,width,height), "Exit")) {
		// exit game
		Application.Quit();
	}
}

function helpScreen() {
	GUI.DrawTexture(Rect(0,0,Screen.width,Screen.height), bgHelp, ScaleMode.StretchToFill, true, 10.0f);
}

function backButton() {
	if (GUI.Button(Rect(10,Screen.height-40,80,30), "Back")) {
		if (screen == "server") { // cancel the server
			Network.Disconnect();
			MasterServer.UnregisterHost();
			CancelInvoke("refreshServerName");
			Debug.Log("UNREGISTER!");
		} else if (screen == "waitingForStart") {
			Debug.Log("waitingForStart");
			networkView.RPC("incommingChatMessage", RPCMode.Others, nickname + " leaved");
			Network.Disconnect();
		}
		screen = "";
	}
}
function chatArea(x:int, y:int) {
	chatMsg = GUI.TextField(Rect(x,y+300,150,20), chatMsg);
	if ((GUI.Button(Rect(x+160,y+300,50,20), "Send")
			|| Event.current.keyCode == KeyCode.Return)
			&& chatMsg.Length > 0) {
		networkView.RPC("incommingChatMessage", RPCMode.All, nickname + ": " + chatMsg);
		chatMsg = "";
	}
	GUI.Label(Rect(x,y,150,300), chat);
}

// check, if serverName has changed
private var regServerName : String = serverName;
function refreshServerName() {
	if (serverName != regServerName) {
	    MasterServer.RegisterHost("BomberManUniTrier", serverName, "a comment!");
	    regServerName = serverName;
	}
}
