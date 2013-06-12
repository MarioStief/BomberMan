#pragma strict

public var playerPrefab : Transform; // the player game-object

function Start () {

}

function Update () {

}

var showMenu = false;
function OnGUI () {
	if (GUI.Button(Rect(10,10,80,20), "MENU")) {
		showMenu = !showMenu;
	}
	if (showMenu) {
		GUI.Box(Rect(10,30,50,20), "Sure?");
		if (GUI.Button(Rect(60,30,50,20),"NO")) {
			showMenu = false;
		}
		if (GUI.Button(Rect(110,30,50,20),"YES")) {
			Application.LoadLevel(0);
		}
	}
	
	// List of all Players
	var cons = Network.connections;
	for (var i=0; i<cons.length; i++) {
		//Debug.Log(cons[i].externalIP.ToString());
	}
}


function OnConnectedToServer () {
    //Network.Instantiate(playerPrefab, transform.position, transform.rotation, 0);
}
