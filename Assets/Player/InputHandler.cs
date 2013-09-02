using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using System.IO;

// <summary>
// InputHandler nimmt jeglichen relevanten Input entgegen und verarbeitet diesen
// bzw. leiten ihn an verarbeitende Klassen weiter.
// </summary>

public class InputHandler : MonoBehaviour {
	
	bool DEBUGPLAYERPOSITION = false;
	
	float angle = 0f;
	
	private int count = 0;
	
	private int n_L;				// Anzahl Längen und Breitengeraden
	private int n_B;
	
	private int lpos;				// Position der aktuellen Parzelle rink.gameArea ist [lpos][bpos]
	private int bpos;
	private int prevLpos;
	private int prevBpos;
	
	private float verticalAngle;
	private float horizontalAngle;
	
	private static float vertAngleM, vertAngle=0; // statische Kopie von verticalAngle
	
	private float verticalHelper;
	private float horizontalHelper;
	
	private int vDirection;			// Bewegungsrichtung
	private int hDirection;
	
	private GameObject cam;
	private GameObject sun;
	
	private Parcel currCell;
	
	float verticalMovement;
	float horizontalMovement;
	float deadVerticalMovement;
	float deadHorizontalMovement;
	int diffv = 0;
	int diffh = 0;
	
	bool autoMove = false;
	bool running = false;
	
	private float playerRadius = 3.5f * Mathf.Deg2Rad;
	NetworkPlayer me;
	
	void Awake() {
		Static.setInputHandler(this);
		sun = GameObject.FindGameObjectWithTag("Sun");
		cam = GameObject.FindGameObjectWithTag("MainCamera");
		
		if (Application.loadedLevelName == "StartMenu") {
			lockCursor(false);
		} else { // Ingame
			DontDestroyOnLoad(gameObject);
			lockCursor(true);
			// play bg-music
			if (Preferences.getBackgroundMusic()) {
				Static.menuHandler.playSound(Static.selectRandomMusic(), true);
				Static.menuHandler.transform.gameObject.GetComponent<AudioSource>().volume = 0.7f;
			}
		}
	}
	
	public void lockCursor(bool l) {
		if (l) {
			Screen.lockCursor = true;
			Screen.showCursor = false;
		} else {
			Screen.lockCursor = false;
			Screen.showCursor = true;
		}
	}
	
	// Use this for initialization
	void Start () {
		
		if (Network.peerType != NetworkPeerType.Disconnected && !networkView.isMine) {
			verticalAngle = vertAngle;
			return;
		}
		
		GetComponent<CapsuleCollider>().enabled = true;
		
		n_L = Static.sphereHandler.n_L;
		n_B = Static.sphereHandler.n_B;
		
		lpos = n_L/2-1;
		bpos = n_B/4;
		
		currCell = Static.rink.gameArea[lpos][bpos];
		Static.player.setCurrentParcel(currCell);
		
		vDirection = 0;
		hDirection = 0;
		
		verticalAngle = 0.0f;
		horizontalAngle = 0.0f;
		
		verticalHelper = 0.0f;
		horizontalHelper = 0.0f;
		
		transform.up = transform.position;
		
		Static.rink.renderAll();
		Static.player.resetStats();

		
		if (Application.loadedLevelName != "StartMenu") {
			networkView.RPC("coloratePlayer", RPCMode.AllBuffered, Network.player, networkView.viewID);
			me = Network.player;
			
			if (Menu.gameStarted && !Network.isServer) { // I'm too late for this round
				Static.player.setDead(true, networkView);
				networkView.RPC("removePlayer", RPCMode.AllBuffered, Network.player);
				Menu.gameStarted = false;
			}
			
			int startLpos = Menu.spawns[Network.player][0];
			int startBpos = Menu.spawns[Network.player][1];
			
			// Planet ver. drehen
			Vector3 target = Static.rink.gameArea[startLpos][startBpos].getCenterPos();
			verticalAngle = Vector3.Angle(target, new Vector3(target.x, target.y, 0)) * Mathf.Deg2Rad;
			if (target.z < 0)
				verticalAngle *= -1;
			vertAngle = verticalAngle;
			//float maxA = Mathf.PI/(n_L-1);
			float maxA = 0.01f; // Drehwinkel darf nicht größer sein!
			float done = 0;
			while (done < Mathf.Abs(verticalAngle)) {
				if (Mathf.Abs(verticalAngle) - done > maxA) {
					Static.sphereHandler.move(maxA * Mathf.Sign(verticalAngle)); //verticalAngle);
					done += maxA;
				} else {
					Static.sphereHandler.move((Mathf.Abs(verticalAngle) - done) * Mathf.Sign(verticalAngle));
					break;
				}
			}
			
			// Spieler hor. drehen
			target.z = 0;
			horizontalAngle = Vector3.Angle(target, Vector3.up);
			if (target.x > 0)
				horizontalAngle *= -1;
			transform.RotateAround(Vector3.zero, Vector3.forward, horizontalAngle);
			horizontalAngle *= Mathf.Deg2Rad;
			
			verticalHelper = (startLpos-lpos) * Mathf.PI/(n_L-1);
			//horizontalHelper = (startBpos-bpos) * 2*Mathf.PI/n_B;
			horizontalHelper = Mathf.Round(horizontalAngle / (2*Mathf.PI/n_B)) * (2*Mathf.PI/n_B);
			
			lpos = startLpos;
			bpos = startBpos;
			
			currCell = Static.rink.gameArea[lpos][bpos];
			Static.player.setCurrentParcel(currCell);
		}
	}
	
	private void moveToPosition(int newLpos, int newBpos) {
		const float SPEED = 1.5f;
		prevLpos = lpos;
		prevBpos = bpos;

		diffv = newLpos - lpos;
		diffh = newBpos - bpos;

		// kürzesten Weg bestimmen
		if (19 - Mathf.Abs(diffv) < Mathf.Abs(diffv))
			diffv = (diffv < 0 ? 19 - Mathf.Abs(diffv) : Mathf.Abs(diffv) - 19);
		if (30 - Mathf.Abs(diffh) < Mathf.Abs(diffh))
			diffh = (diffh < 0 ? 30 - Mathf.Abs(diffh) : Mathf.Abs(diffh) - 30);
		
		transform.localScale = Vector3.zero;
		//Static.rink.gameArea[newLpos][newBpos].hightlightColor(true);
		
		//GetComponent<CapsuleCollider>().enabled = false;
		autoMove = true;
		Static.camera.GetComponent<MouseLookGame>().setRotatable(false);
		
		if (diffh != 0) {
			horizontalMovement = diffh < 0 ? SPEED : -SPEED;
			horizontalMovement *= Static.player.getSpeed();
		}
		if (diffv != 0)
			verticalMovement = diffv < 0 ? -SPEED : SPEED;
		
		diffv = Mathf.Abs(diffv);
		diffh = Mathf.Abs(diffh);

	}

	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if (stream.isWriting) {
			Vector3 p = transform.position;
			stream.Serialize(ref p);
			
			float sva = vertAngle;
			if (info.networkView.isMine)
				sva = 0;
			stream.Serialize(ref sva);
			
			float va = verticalAngle;
			stream.Serialize(ref va);
			
			Quaternion r = transform.rotation;
			stream.Serialize(ref r);
			
			bool ir = running;
			stream.Serialize(ref ir);
		}
		else if (info.networkView.viewID == networkView.viewID) {
			Vector3 fp = Vector3.zero;
			stream.Serialize(ref fp);
			
			float sva = 0f; // der aktuelle Winkel des Servers
			stream.Serialize(ref sva);
			
			float fva = 0f;
			stream.Serialize(ref fva);
			verticalAngle = fva;
			
			Quaternion fr = Quaternion.Euler(new Vector3(0, 0, 0));
			stream.Serialize(ref fr);
			transform.rotation = fr;
			
			transform.position = fp; // Spieler ist auf Äquator.. gar nicht wahr!
			Vector3 axis = Vector3.Cross(transform.position, Vector3.forward).normalized;
			
			if (sva != 0) // anderer Client über Server..
				transform.RotateAround(Vector3.zero, axis, (sva + -vertAngle) * Mathf.Rad2Deg);
			else
				transform.RotateAround(Vector3.zero, axis, (-vertAngle + verticalAngle) * Mathf.Rad2Deg);
			
			// Animate Player
			bool fir = false;
			stream.Serialize(ref fir);
			running = fir;
			if (running)
				GetComponentInChildren<Animation>().CrossFade("runforward");
			else
				GetComponentInChildren<Animation>().CrossFade("idle");
		}
	
	}
	
	IEnumerator deadPlayer() {
		float createTime = Time.time;
		float elapsedTime = 0.0f;
		GetComponentInChildren<Animation>().CrossFade("die");
		while (elapsedTime < 10f) {
			float multiplicator = elapsedTime + 10f; // 10 <= multiplicator <= 20
			float x = transform.position.x;
			float y = transform.position.y;
			float z = transform.position.z;
			yield return new WaitForSeconds(Random.value);
			Vector3 position = new Vector3(Random.Range(x-0.1f, x+0.1f), Random.Range(y-0.1f, y+0.1f), Random.Range(z-0.1f, z+0.1f));
			GameObject explosion = GameObject.Instantiate(Static.explosionPrefab, position, Quaternion.identity) as GameObject;
			Detonator detonator = explosion.GetComponent<Detonator>();
			detonator.setSize(Random.Range(50f * multiplicator, 100f * multiplicator));
			detonator.setDuration(5f);
			float distance = Vector3.Distance (GameObject.FindGameObjectWithTag("Player").transform.position, position);
			detonator.GetComponent<AudioSource>().volume /= distance * multiplicator;
			detonator.GetComponent<AudioSource>().Play();
			detonator.Explode();
			//float scale = 1f - ((multiplicator - 10f) / 10f); // Range: 1 - 0
			//Debug.Log ("---> " + scale);
			//transform.localScale *= scale;
			//transform.position -= 0.7f * transform.position.normalized * Time.deltaTime;
			elapsedTime = Time.time - createTime;
		}
		transform.localScale = Vector3.zero;
		GetComponent<CapsuleCollider>().enabled = false;
		while (Static.player.isDead()) {
			Debug.Log("old Vertical: " + deadVerticalMovement + " old Horizontal: " + deadHorizontalMovement);
			float v;
			float h;
			do {
				v = deadVerticalMovement;
				h = deadHorizontalMovement;
				switch (Random.Range(0, 2)) {
				case 0:
					if (v != 0f) {
						v = 0f;
					} else {
						v = (Random.Range(0, 2) == 0 ? 0.1f : -0.1f);
					}
					break;
				case 1:
					if (h != 0f) {
						h = 0f;
					} else {
						h = (Random.Range(0, 2) == 0 ? 0.1f : -0.1f);
					}
					break;
				}
			} while (v == 0f && h == 0f);
			deadVerticalMovement = v;
			deadHorizontalMovement = h;
			Debug.Log("new Vertical: " + deadVerticalMovement + " new Horizontal: " + deadHorizontalMovement);
			yield return new WaitForSeconds(Random.value*5 + 5f);
		}
	}
	
	void Update () {
		
		if (autoMove) { // drehe Kamera auf Spawnposition
			if (prevLpos != lpos) {
				diffv--;
				prevLpos = lpos;
			}
			
			if (prevBpos != bpos) {
				diffh--;
				prevBpos = bpos;
			}
			
			if (diffv == 0)
				verticalMovement = 0f;
			if (diffh == 0)
				horizontalMovement = 0f;
			
			if (diffv == 0 && diffh == 0) {
				autoMove = false;
				Static.camera.GetComponent<MouseLookGame>().setRotatable(true);
				transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
				GetComponentInChildren<Animation>().CrossFade("land");
			}
		}
		
		// Gegner drehen mit dem Planeten..!
		if (Network.peerType != NetworkPeerType.Disconnected && !networkView.isMine) {
			
			if (vertAngleM != 0) { // an Wänden hängen bleiben..
				Vector3 axis = Vector3.Cross(Vector3.forward, transform.position);
				transform.RotateAround(Vector3.zero, axis, vertAngleM * Mathf.Rad2Deg);
			}
			
			return;
		}
		
		// Menu
		if (Application.loadedLevelName == "StartMenu") {
			moveCharacter();
			
			// drehe Menüpunkte
			GameObject.Find("Menu").transform.RotateAround(Vector3.zero, Vector3.right, -vertAngleM*Mathf.Rad2Deg);
			vertAngleM = 0;
			
			// nöchsten Screen anzeigen
			string s = "";
			if (Input.GetMouseButtonDown(0)) {
				RaycastHit hit;
				Ray ray = cam.camera.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out hit)) {
					if (hit.collider.gameObject.name == "create" || hit.collider.gameObject.name == "cServer")
						s = "server";
					else if (hit.collider.gameObject.name == "join" || hit.collider.gameObject.name == "jServer")
						s = "join";
					else if (hit.collider.gameObject.name == "help")
						s = "help";
					else if (hit.collider.gameObject.name == "exit")
						Application.Quit();
				}
			}
			if (currCell != Static.rink.gameArea[lpos][bpos] || s != "") {
				if (currCell.getBpos() > bpos)
					s = "server";
				else if (currCell.getBpos() < bpos)
					s = "join";
				else if (currCell.getLpos() < lpos)
					s = "help";
				else if (currCell.getLpos() > lpos)
					Application.Quit();
				if (s != "") {
					GameObject.Find("Menu").GetComponent<Menu>().setScreen(s);
					Application.LoadLevel("Menu");
					GameObject.Find("Menu").transform.RotateAround(Vector3.zero, Vector3.right, verticalAngle*Mathf.Rad2Deg);
				}
			}
			return;
		}

		
		if (Static.rink != null && !Static.player.isDead()) {
			
			// -----------------------------------------------------------
			// Bewegung und Bestimmung einer möglichen neuen currentParcel
			// -----------------------------------------------------------
			moveCharacter();
			currCell = Static.rink.gameArea[lpos][bpos];
			if (DEBUGPLAYERPOSITION)
				currCell.colorCell(Color.cyan);
			//Debug.Log("[" + lpos + "][" + bpos + "]");
			
			if (currCell.hasContactMine()) {
				networkView.RPC("startEvent", RPCMode.All, currCell.getLpos(), currCell.getBpos(), 3);
			}
				
			if (currCell.isExploding()) {
				Static.player.setDead(true, networkView);
				//renderer.material.color = Color.black;
				StartCoroutine(deadPlayer());
				networkView.RPC("removePlayer", RPCMode.OthersBuffered, Network.player);
				Static.player.imOut(Network.player);
			}
			
			
			// Falls die Zelle ein Powerup enthält -> aufsammeln
			if (currCell.hasPowerup()) {
				networkView.RPC("startEvent", RPCMode.Others, currCell.getLpos(), currCell.getBpos(), 0);
				Static.player.powerupCollected(currCell.destroyPowerup(true, false));
			}
			
			// Leertaste -> Bombe legen
#if UNITY_IPHONE
			if (Input.GetButtonUp("Fire1") && Input.touchCount < 2 && !Menu.showGUI)
				dropBomb();
#else
			if (Input.GetButton("Fire1") && !Menu.showGUI)
				dropBomb();
#endif

			if (Input.GetButtonDown("Fire2") || Input.touchCount == 2 && Input.GetTouch(1).phase == TouchPhase.Ended)
				if (!Menu.showGUI)
					extra();
			
			if (Input.GetKeyDown(KeyCode.Print) || Input.GetKeyDown(KeyCode.P))
	            StartCoroutine(ScreenshotEncode());

		} else if (Static.player.isDead()) {
			// vor Scham im Boden versinken lassen ;)
			transform.position -= 0.023f * transform.position.normalized * Time.deltaTime;
			moveCharacter();
		}
	}
			
	IEnumerator ScreenshotEncode()
    {
        // wait for graphics to render
        yield return new WaitForEndOfFrame();
 
        // create a texture to pass to encoding
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
 
        // put buffer into texture
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();
 
        // split the process up--ReadPixels() and the GetPixels() call inside of the encoder are both pretty heavy
        yield return 0;
 
        byte[] bytes = texture.EncodeToPNG();
 
        // save our test image (could also upload to WWW)
        File.WriteAllBytes(Application.dataPath + "/../../Screenshot-" + count + ".png", bytes);
        count++;
 
        // Added by Karl. - Tell unity to delete the texture, by default it seems to keep hold of it and memory crashes will occur after too many screenshots.
        DestroyObject( texture );
 
        Debug.Log( Application.dataPath + "/../Screenshot-" + count + ".png" );
    }
	
	public void dropBomb() {
		if (!currCell.hasExplosion() && !Static.player.isDead() && !autoMove) {
			int extra = Static.player.addBomb();
			if (extra > -1) {
				GameObject ex = Network.Instantiate(Resources.Load("Prefabs/Bombe"), currCell.getCenterPos(), Quaternion.identity, 0) as GameObject;
				ex.networkView.RPC("createExplosionOnCell", RPCMode.All, currCell.getLpos(), currCell.getBpos(), 
			    	               Static.player.getFlamePower(), Static.player.getDelay(), Static.player.getSuperbomb(), extra, true, true);
				if (extra == 1)
					Static.player.addTriggerBomb(ex, currCell);
				Static.menuHandler.playSound(Static.bombDropSoundEffect, false);
			}
		}
	}
	
	public void extra() {
		if (!currCell.hasExplosion() && !Static.player.isDead() && !autoMove) {
			if (Static.player.addContactMine()) {
				GameObject ex = Network.Instantiate(Resources.Load("Prefabs/Bombe"), currCell.getCenterPos(), Quaternion.identity, 0) as GameObject;
				ex.networkView.RPC("createExplosionOnCell", RPCMode.All, currCell.getLpos(), currCell.getBpos(), 
			     	              Static.player.getFlamePower(), Static.player.getDelay(), Static.player.getSuperbomb(), 2, true, true);
				Static.menuHandler.playSound(Static.contactMineDropSoundEffect, false);
			}
		}
		Static.player.releaseTriggerBombs();
	}
	
	[RPC]
	public void startEvent(int lpos, int bpos, int mode) {
		Parcel cell = Static.rink.gameArea[lpos][bpos];
		switch (mode) {
			case 0: // destroy powerup
				cell.destroyPowerup(false, false);
				break;
			case 1: // destroy powerup
				cell.destroyPowerup(false, true);
				break;
			case 3: // explding contact-mine
				cell.getExplosion().startExplosion(false);
				break;
		}
	}
	[RPC]
	public void addPowerup(int lpos, int bpos, int type) {
		Parcel cell = Static.rink.gameArea[lpos][bpos];
		cell.addPowerup(new Powerup((PowerupType) type));
	}
	
	[RPC]
	public void removePlayer(NetworkPlayer p) {
		if (me == p) {
			transform.localScale = Vector3.zero;
			Static.player.imOut(p);
		}
	}
	
	[RPC]
	public void coloratePlayer(NetworkPlayer p, NetworkViewID id) {
		if (networkView.viewID == id) {
			// colorate the player
			Texture2D illuminColor = Instantiate(Resources.Load("Textures/Player/astrod00d_selfillum") as Texture2D) as Texture2D;
			Color[] color = illuminColor.GetPixels();
			
			Color pColor = Menu.getPlayerColor(p);
			for (int i = 0; i < color.Length; i++)
				if (color[i] != color[0])
					color[i] = pColor;
			
			illuminColor.SetPixels(color);
			illuminColor.Apply();
			
			renderer.material.SetTexture("_SelfIllumin", illuminColor);
			
			me = p;
		}
	}
	
	private void moveCharacter() {
		
		float vm = 0, m = 0;
		vertAngleM = 0;
		
		float vM = Input.GetAxis("Vertical");
		float hM = Input.GetAxis("Horizontal");
		
		if (Mathf.Abs(Input.acceleration.x) > 0.1f)
			hM = Mathf.Clamp(Input.acceleration.x*2f, -1f, 1f);
		if (Mathf.Abs(Input.acceleration.y) > 0.1f)
			vM = Mathf.Clamp(Input.acceleration.y*2f, -1f, 1f);
		
		if (vM != 0 || hM != 0 || autoMove || Static.player.isDead()) {
		
			
			if (!autoMove) {
				verticalMovement = vM;
				horizontalMovement = hM;
			}
			
			if (Static.camera != null && !Static.camera.GetComponent<MouseLookGame>().birdview) {
			
				/* Directions:
				 * 0 hoch:			verticalMovement =  1, horizontalMovement =  0
				 * 1 hoch rechts:	verticalMovement =  1, horizontalMovement =  1
				 * 2 rechts:		verticalMovement =  0, horizontalMovement =  1
				 * 3 runter rechts:	verticalMovement = -1, horizontalMovement =  1
				 * 4 runter:		verticalMovement = -1, horizontalMovement =  0
				 * 5 runter links:	verticalMovement = -1, horizontalMovement = -1
				 * 6 links:			verticalMovement =  0, horizontalMovement = -1
				 * 7 hoch links:	verticalMovement =  1, horizontalMovement = -1
				 */
				
				// Oktant, in den sich der Spieler bewegt
				int moveDirection = 0;
				float treshold = 0f;
				//Debug.Log ("h: " + horizontalMovement + " v: " + verticalMovement);
				
				if (horizontalMovement > treshold) {
					if (verticalMovement > treshold)
						moveDirection = 1;
					else if (verticalMovement < -treshold)
						moveDirection = 3;
					else
						moveDirection = 2;
				} else if (horizontalMovement < -treshold) {
					if (verticalMovement > treshold)
						moveDirection = 7;
					else if (verticalMovement < -treshold)
						moveDirection = 5;
					else
						moveDirection = 6;
				} else {
					if (verticalMovement > treshold)
						moveDirection = 0;
					else if (verticalMovement < -treshold)
						moveDirection = 4;
				}
				
				// Oktantenausrichtung durch die Kamerarotation
				int rotationOctant;
				float x = Static.camera.transform.localPosition.x;
				float y = Static.camera.transform.localPosition.y;
				
				if (y > 0f) { // obere Hälfte
					if (x/y < -2f) rotationOctant = 6; // links
					else if (x/y < -0.5) rotationOctant = 7; // hoch links
					else if (x/y < 0.5) rotationOctant = 0; // hoch
					else if (x/y < 2) rotationOctant = 1; // hoch rechts
					else rotationOctant = 2; // rechts
				} else { // untere Hälfte
					if (x/y < -2f) rotationOctant = 2; // rechts
					else if (x/y < -0.5) rotationOctant = 3; // runter rechts
					else if (x/y < 0.5) rotationOctant = 4; // runter
					else if (x/y < 2) rotationOctant = 5; // runter links
					else rotationOctant = 6; // links
				}
				
				// So drehen, dass der der Kamera gegenüberliegende Oktant dort ist,
				// wo der Spieler nach in Pfeilrichtung "hoch" läuft
				moveDirection = (moveDirection + rotationOctant + 4) % 8;
				
				switch (moveDirection) {
					case 0:
						//Debug.Log("-> hoch");
						verticalMovement =  1f;
						horizontalMovement =  0f;
						break;
					case 1:
						//Debug.Log("-> hoch rechts");
						verticalMovement =  1f;
						horizontalMovement =  1f;
						break;
					case 2:
						//Debug.Log("-> rechts");
						verticalMovement =  0f;
						horizontalMovement =  1f;
						break;
					case 3:
						//Debug.Log("-> runter rechts");
						verticalMovement = -1f;
						horizontalMovement =  1f;
						break;
					case 4:
						//Debug.Log("-> runter");
						verticalMovement = -1f;
						horizontalMovement =  0f;
						break;
					case 5:
						//Debug.Log("-> runter links");
						verticalMovement = -1f;
						horizontalMovement = -1f;
						break;
					case 6:
						//Debug.Log("-> links");
						verticalMovement =  0f;
						horizontalMovement = -1f;
						break;
					case 7:
						//Debug.Log("-> hoch links");
						verticalMovement =  1f;
						horizontalMovement = -1f;
						break;
				}
				if (vM != 0) {
					verticalMovement *= Mathf.Abs(vM);
					horizontalMovement *= Mathf.Abs(vM);
				} else if (hM != 0) {
					verticalMovement *= Mathf.Abs(hM);
					horizontalMovement *= Mathf.Abs(hM);
				}
			}
			
			
			if (!autoMove)
				horizontalMovement *= Static.player.getSpeed();
			
			if (Static.player.isDead() && !autoMove) {
				verticalMovement = deadVerticalMovement;
				horizontalMovement = deadHorizontalMovement;
			}
			
			if (verticalMovement != 0) {
				vm = Static.player.getSpeed() * verticalMovement * Time.deltaTime;
				if (vDirection == 0) {
					
					vDirection = (int)Mathf.Sign(vm);
					
					if (vDirection == 1) {
						verticalHelper -= 	Mathf.PI/(2*(n_L-1));
					} else {
						verticalHelper += 	Mathf.PI/(2*(n_L-1));
					}
				}
			
				if (!Static.player.isDead()) {
					vm = determineVerticalParcelPosition(verticalMovement, vm);
				}
				verticalAngle += vm;
				//verticalAngle = verticalAngle % (Mathf.PI*2);
				vertAngle = verticalAngle;
				vertAngleM = vm;
				
				Static.sphereHandler.move(vm);
			}

			if (horizontalMovement != 0) {
				m = horizontalMovement*Time.deltaTime*Static.player.getSpeed()*(-2);
				if (hDirection == 0) {
					
					hDirection = (int)Mathf.Sign(m);
					
					if (hDirection == 1) {
						horizontalHelper += 	Mathf.PI/(n_B);
					} else {
						horizontalHelper -= 	Mathf.PI/(n_B);
					}
					horizontalHelper += m;
				}
			 
				if (!Static.player.isDead()) {
					m = determineHorizontalParcelPosition(horizontalMovement, m);
				}
				horizontalAngle += m;
				//horizontalAngle = horizontalAngle % (Mathf.PI*2);
				
				moveAlongEquator(m);
			}
			
			// Spielerrotation
			float newAngle = angle;
			if (verticalMovement > 0) {
				// nach hoch schauen
				if (horizontalMovement < 0) {
					// nach links hoch schauen
					newAngle = 315f;
				} else if (horizontalMovement > 0) {
					// nach rechts hoch schauen
					newAngle = 45f;
				} else {
					// nur nach hoch schauen
					newAngle = 0f;
				}
			} else if (verticalMovement < 0) {
				// nach runter schauen
				if (horizontalMovement < 0) {
					// nach links runter schauen
					newAngle = 225f;
				} else if (horizontalMovement > 0) {
					// nach rechts runter schauen
					newAngle = 135f;
				} else {
					// nur nach runter schauen
					newAngle = 180f;
				}
			} else {
				if (horizontalMovement < 0) {
					// nur nach links schauen
					newAngle = 270f;
				} else if (horizontalMovement > 0) {
					// nur nach rechts schauen
					newAngle = 90f;
				}
			}
			
			if (Mathf.Abs(angle - newAngle) > 180f)
				angle = (angle + newAngle) / 2 + 180f;
			else
				angle = (angle + newAngle) / 2;
			
			if (angle != newAngle || Application.loadedLevelName != "StartMenu") {
				transform.up = transform.position;
				transform.Rotate(0f, angle, 0f, Space.Self);
			}
		}
		
		// Animate Player
		if (!Static.player.isDead()) {
			running = Mathf.Abs(m) > 0.002 || Mathf.Abs(vm) > 0.002;
			if (running)
				GetComponentInChildren<Animation>().CrossFade("runforward");
			else
				GetComponentInChildren<Animation>().CrossFade("idle");
		}
	}
	
	// <summary>
	// Bestimme, ob die Spielfigur einen neuen Würfel berührt. Wenn dem so ist, ändere currParcel auf
	// die Position des neuen Würfels.
	// </summary>
	private float determineVerticalParcelPosition(float verticalMovement, float m) {
		
		if (vDirection == 1 && Mathf.Sign(verticalMovement) == 1) {	// Bewegungsrichtung blieb gleich				
				if (DEBUGPLAYERPOSITION)
					Debug.Log("#V1");
			
				// Setting position of player in current cell
				float newPlayerPosition =  1-Mathf.Abs(verticalAngle-verticalHelper)/(Mathf.PI/(n_L-1)); //bs(verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Static.player.setXPos(newPlayerPosition);
				if (DEBUGPLAYERPOSITION)
					Debug.Log(newPlayerPosition);
			
				if (Mathf.Abs(verticalAngle - verticalHelper + playerRadius) > Mathf.PI/(n_L-1)) {
				
					Parcel newCell = Static.rink.gameArea[lpos < n_L-2 ? lpos+1 : 0][bpos];
					if (newCell.getType() != 0 || newCell.hasBomb()) {
						if (!autoMove) return 0.0f;
					}
					if (Mathf.Abs(verticalAngle - verticalHelper) > Mathf.PI/(n_L-1)) {
						//Static.player.setXPos(0);
						verticalHelper += Mathf.PI/(n_L-1);
						lpos = newCell.getLpos();
						Static.player.setCurrentParcel(newCell);
					}
				}
			} else if (vDirection == 1 && Mathf.Sign(verticalMovement) == -1) {	// Bewegungsrichtung ändert sich
				
				if (DEBUGPLAYERPOSITION)
					Debug.Log("#V2");
				float newPlayerPosition = Mathf.Abs(verticalAngle-verticalHelper)/(Mathf.PI/(n_L-1)); //bs(verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Static.player.setXPos(newPlayerPosition);
				if (DEBUGPLAYERPOSITION)
					Debug.Log(newPlayerPosition);
			
				vDirection = -1;
				verticalHelper += Mathf.PI/((n_L-1));
					
				if (Mathf.Abs(verticalAngle - verticalHelper + playerRadius) > Mathf.PI/(n_L-1)) {
					
					Parcel newCell = Static.rink.gameArea[lpos > 0 ? lpos-1 : n_L-2][bpos];
					if (newCell.getType() != 0 || newCell.hasBomb()) {
						if (!autoMove) return 0.0f;
					}
					if (Mathf.Abs(verticalAngle - verticalHelper) > Mathf.PI/(n_L-1)) {
						verticalHelper -= Mathf.PI/(n_L-1);
						Static.player.setCurrentParcel(newCell);
						lpos = newCell.getLpos();
					}
				}
			} else if (vDirection == -1 && Mathf.Sign(verticalMovement) == -1) {	// Bewegungsrichtung blieb gleich
				
				if (DEBUGPLAYERPOSITION)
					Debug.Log("#V3");
				// Setting position of player in current cell
				float newPlayerPosition =  Mathf.Abs(verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Static.player.setXPos(newPlayerPosition);
				if (DEBUGPLAYERPOSITION)
					Debug.Log(newPlayerPosition);

				if (Mathf.Abs(verticalAngle - verticalHelper - playerRadius) > Mathf.PI/(n_L-1)) {
					
					Parcel newCell = Static.rink.gameArea[lpos > 0 ? lpos-1 : n_L-2][bpos];
					if (newCell.getType() != 0 || newCell.hasBomb()) {
						if (!autoMove) return 0.0f;
					}
					if (Mathf.Abs(verticalAngle - verticalHelper) > Mathf.PI/(n_L-1)) {
						verticalHelper -= Mathf.PI/(n_L-1);
						Static.player.setCurrentParcel(newCell);
						lpos = newCell.getLpos();
					}
				}
			} else if (vDirection == -1 && Mathf.Sign(verticalMovement) == 1) {	// Bewegungsrichtung ändert sich
				
				if (DEBUGPLAYERPOSITION)
					Debug.Log("#V4");
				float newPlayerPosition =  1-Mathf.Abs(verticalAngle-verticalHelper)/(Mathf.PI/(n_L-1)); //bs(verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Static.player.setXPos(newPlayerPosition);
				if (DEBUGPLAYERPOSITION)
					Debug.Log(newPlayerPosition);
			
				vDirection = 1;
				verticalHelper -=	Mathf.PI/((n_L-1));
				
				if (Mathf.Abs(verticalAngle - verticalHelper + playerRadius) > Mathf.PI/(n_L-1)) {
					
					Parcel newCell = Static.rink.gameArea[lpos < n_L-2 ? lpos+1 : 0][bpos];
					if (newCell.getType() != 0 || newCell.hasBomb()) {
						if (!autoMove) return 0.0f;
					}
					if (Mathf.Abs(verticalAngle - verticalHelper) > Mathf.PI/(n_L-1)) {
						verticalHelper += Mathf.PI/(n_L-1);
						Static.player.setCurrentParcel(newCell);
						lpos = newCell.getLpos();
					}
				}
			}
		
		return m;
	}
	
	private float determineHorizontalParcelPosition(float horizontalMovement, float m) {
		
		if (hDirection == -1 && Mathf.Sign(horizontalMovement) == -1) {	// Bewegungsrichtung blieb gleich				
				if (DEBUGPLAYERPOSITION)
					Debug.Log("#H1");
			
				float newPlayerPosition =  1-Mathf.Abs(horizontalAngle-horizontalHelper)/(2*Mathf.PI/n_B); //bs(verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Static.player.setZPos(newPlayerPosition);
				if (DEBUGPLAYERPOSITION)
					Debug.Log(newPlayerPosition);
			
			
				if (Mathf.Abs(horizontalAngle - horizontalHelper + playerRadius) > 2*Mathf.PI/n_B) {
					
					Parcel newCell;
					
					if (bpos < n_B-1) {
						if (Static.rink.gameArea[lpos][bpos+1].getType() != 0 || Static.rink.gameArea[lpos][bpos+1].hasBomb()) {
							if (!autoMove) return 0.0f;
						}
						newCell = Static.rink.gameArea[lpos][bpos+1];
					} else {
						if (Static.rink.gameArea[lpos][0].getType() != 0 || Static.rink.gameArea[lpos][0].hasBomb()) {
							if (!autoMove) return 0.0f;
						}
						newCell = Static.rink.gameArea[lpos][0];
					}
					if (Mathf.Abs(horizontalAngle - horizontalHelper) > 2*Mathf.PI/n_B) {
						horizontalHelper += 2*Mathf.PI/n_B;
						Static.player.setCurrentParcel(newCell);
						bpos = newCell.getBpos();
					}
				}
			} else if (hDirection == -1 && Mathf.Sign(horizontalMovement) == 1) {	// Bewegungsrichtung ändert sich
				
				if (DEBUGPLAYERPOSITION)
					Debug.Log("#H2");
				
			
				hDirection = 1;
				horizontalHelper +=	2*Mathf.PI/(n_B);
			
				float newPlayerPosition =  Mathf.Abs(horizontalAngle-horizontalHelper)/(2*Mathf.PI/n_B); //bs(verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Static.player.setZPos(newPlayerPosition);
				if (DEBUGPLAYERPOSITION)
					Debug.Log(newPlayerPosition);
					
				if (Mathf.Abs(horizontalAngle - horizontalHelper - playerRadius) > 2*Mathf.PI/n_B) {
				
					Parcel newCell;
					
					if (bpos > 0) {
						if (Static.rink.gameArea[lpos][bpos-1].getType() != 0 || Static.rink.gameArea[lpos][bpos-1].hasBomb()) {
							if (!autoMove) return 0.0f;
						}
						newCell = Static.rink.gameArea[lpos][bpos-1];
					} else {
						if (Static.rink.gameArea[lpos][n_B-1].getType() != 0 || Static.rink.gameArea[lpos][n_B-1].hasBomb()) {
							if (!autoMove) return 0.0f;
						}
						newCell = Static.rink.gameArea[lpos][n_B-1];
					}
					if (Mathf.Abs(horizontalAngle - horizontalHelper) > 2*Mathf.PI/n_B) {
						horizontalHelper -= 2*Mathf.PI/n_B;
						Static.player.setCurrentParcel(newCell);
						bpos = newCell.getBpos();
					}
				}
			} else if (hDirection == 1 && Mathf.Sign(horizontalMovement) == 1) {	// Bewegungsrichtung blieb gleich
				
				if (DEBUGPLAYERPOSITION)
					Debug.Log("#H3");
			
				float newPlayerPosition =  Mathf.Abs(horizontalAngle-horizontalHelper)/(2*Mathf.PI/n_B); //bs(verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Static.player.setZPos(newPlayerPosition);
				if (DEBUGPLAYERPOSITION)
					Debug.Log(newPlayerPosition);
			
				if (Mathf.Abs(horizontalAngle - horizontalHelper - playerRadius) > 2*Mathf.PI/n_B) {
				
					Parcel newCell;
						
					if (bpos > 0) {
						if (Static.rink.gameArea[lpos][bpos-1].getType() != 0 || Static.rink.gameArea[lpos][bpos-1].hasBomb()) {
							if (!autoMove) return 0.0f;
						}
						newCell = Static.rink.gameArea[lpos][bpos-1];
					} else {
						if (Static.rink.gameArea[lpos][n_B-1].getType() != 0 || Static.rink.gameArea[lpos][n_B-1].hasBomb()) {
							if (!autoMove) return 0.0f;
						}
						newCell = Static.rink.gameArea[lpos][n_B-1];
					}
					if (Mathf.Abs(horizontalAngle - horizontalHelper) > 2*Mathf.PI/n_B) {
						horizontalHelper -= 2*Mathf.PI/n_B;
						Static.player.setCurrentParcel(newCell);
						bpos = newCell.getBpos();
					}
				}
			} else if (hDirection == 1 && Mathf.Sign(horizontalMovement) == -1) {	// Bewegungsrichtung ändert sich
				
				if (DEBUGPLAYERPOSITION)
					Debug.Log("#H4");
				
				hDirection = -1;
				horizontalHelper -=	2*Mathf.PI/(n_B);
			
				float newPlayerPosition =  1-Mathf.Abs(horizontalAngle-horizontalHelper)/(2*Mathf.PI/n_B); //bs(verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Static.player.setZPos(newPlayerPosition);
				if (DEBUGPLAYERPOSITION)
					Debug.Log(newPlayerPosition);
			
				if (Mathf.Abs(horizontalAngle - horizontalHelper + playerRadius) > 2*Mathf.PI/n_B) {
					
					Parcel newCell;
					
					if (bpos < n_B-1) {
						if (Static.rink.gameArea[lpos][bpos+1].getType() != 0 || Static.rink.gameArea[lpos][bpos+1].hasBomb()) {
							if (!autoMove) return 0.0f;
						}
						newCell = Static.rink.gameArea[lpos][bpos+1];
					} else {
						if (Static.rink.gameArea[lpos][0].getType() != 0 || Static.rink.gameArea[lpos][0].hasBomb()) {
							if (!autoMove) return 0.0f;
						}
						newCell = Static.rink.gameArea[lpos][0];
					}
					if (Mathf.Abs(horizontalAngle - horizontalHelper) > 2*Mathf.PI/n_B) {
						horizontalHelper += 2*Mathf.PI/n_B;
						Static.player.setCurrentParcel(newCell);
						bpos = newCell.getBpos();
					}
				}
			} 
		
		return m;

	}
	
	private void moveAlongEquator(float movement) {
		
		movement = movement * Mathf.Rad2Deg;
		
		// Spieler drehen
		transform.RotateAround(Vector3.zero, Vector3.forward, movement);
		
		// Kamera drehen
		cam.transform.RotateAround(Vector3.zero, Vector3.forward, movement);
		Vector3 up = Vector3.forward;
		if (Application.loadedLevelName != "StartMenu" && !Static.camera.GetComponent<MouseLookGame>().birdview)
			up = transform.position;
		cam.transform.LookAt(transform.position, up);
		
		// Licht mitdrehen..
		sun.transform.RotateAround(Vector3.zero, Vector3.forward, movement);
		sun.transform.eulerAngles = new Vector3(0,90,0);
	}
}
