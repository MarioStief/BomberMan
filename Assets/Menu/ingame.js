#pragma strict

public var playerPrefab : Transform; // the player game-object

function Start () {

}

function Update () {

}

function OnGUI () {
	if (GUI.Button(Rect(10,10,80,20), "MENU")) {
		Application.LoadLevel(0);
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

// SERVER ONLY
function OnPlayerConnected (player: NetworkPlayer) {
}