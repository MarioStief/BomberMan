#pragma strict


var bgHelp : Texture; // Picture of Help-Menu

private var screen : String;

var serverName : String = "Galaxy 42";
private var port   : int = 25000;
var maxPlayers  : int = 4;

private var nickname : String = "";
private var chat : String = "";

function Start () {
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
	networkView.RPC("newPlayer", RPCMode.Server, nickname);
}

function joinScreen() {
	var width = 150;
	//GUI.Label(Rect(x, Screen.height/2-50, width, 20), "Server-IP:");
	//server = GUI.TextField(Rect(x, Screen.height/2-30,width,20), server);
	//GUI.Label(Rect(x, Screen.height/2-50, width, 20), "Server-IP:");
	//port = GUI.TextField(Rect(x, Screen.height/2-30,width,20), port);
	GUI.BeginGroup(Rect(Screen.width/2-width/2,10,width+75,500));
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
					Network.Connect(srv);
				} catch(err) {
					Debug.Log("Server unreachable!");
				}
				screen = "waitingForStart";
			}
		}
		i++;
	}
	GUI.EndGroup();
	//GUILayout.TextField(Network.player.externalIP);
}

private var chatMsg : String = "";
function waitingForStartScreen() {
	// display a chat and the list of currently conntected players
	// ..
	chatArea(Screen.width/2-200, 20);
}

@RPC
function incommingChatMessage(text : String, info: NetworkMessageInfo) {
	chat += "\n" + text;
	Debug.Log(text + " from: "+info.sender);
}

@RPC
function startGame() {
	Application.LoadLevel(1);
}

@RPC
function newPlayer(player : String) {
	Debug.Log("new player "+player+" connected");
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
	    Application.LoadLevel(1);
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
			MasterServer.UnregisterHost();
			CancelInvoke("refreshServerName");
		}
		screen = "";
	}
}
function chatArea(x:int, y:int) {
	chatMsg = GUI.TextField(Rect(x,y+300,150,20), chatMsg);
	if ((GUI.Button(Rect(x+160,y+300,50,20), "Send")
			|| Event.current.keyCode == KeyCode.Return)
			&& chatMsg.Length > 0) {
		networkView.RPC("incommingChatMessage", RPCMode.All, chatMsg);
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
