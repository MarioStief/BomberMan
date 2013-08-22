using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

// <summary>
// InputHandler nimmt jeglichen relevanten Input entgegen und verarbeitet diesen
// bzw. leiten ihn an verarbeitende Klassen weiter.
// </summary>

public class InputHandler : MonoBehaviour {
	
	bool DEBUGPLAYERPOSITION = false;
	
	int angle = 0;
	
	private int n_L;				// Anzahl Längen und Breitengeraden
	private int n_B;
	
	private int lpos;				// Position der aktuellen Parzelle rink.gameArea ist [lpos][bpos]
	private int bpos;
	
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
	
	private float createTime;
	
	float verticalMovement;
	float horizontalMovement;
	
	bool running = false;
	
	private float playerRadius = 3.5f * Mathf.Deg2Rad;
	
	void Awake() {
		Static.setInputHandler(this);
		sun = GameObject.FindGameObjectWithTag("Sun");
		cam = GameObject.FindGameObjectWithTag("MainCamera");
		
		if (Application.loadedLevelName != "StartMenu")
			DontDestroyOnLoad(gameObject);
	}

	
	// Use this for initialization
	void Start () {
		
		// colorate the player
		Texture2D illuminColor = Instantiate(Resources.Load("Textures/Player/astrod00d_selfillum") as Texture2D) as Texture2D;
		Color[] color = illuminColor.GetPixels();
		
		Color pColor = Menu.getPlayerColor(networkView.owner);
		for (int i = 0; i < color.Length; i++)
			if (color[i] != color[0])
				color[i] = pColor;
		
		illuminColor.SetPixels(color);
		illuminColor.Apply();
		
		renderer.material.SetTexture("_SelfIllumin", illuminColor);
		
		if (Network.peerType != NetworkPeerType.Disconnected && !networkView.isMine) {
			verticalAngle = vertAngle;
			return;
		}
		
		createTime = Time.time;
		
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
		
		Static.rink.renderAll();
		Static.player.resetStats();
	}

	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if (stream.isWriting) {
			Vector3 p = transform.position;
			stream.Serialize(ref p);
			
			float va = verticalAngle;
			stream.Serialize(ref va);
			
			Quaternion r = transform.rotation;
			stream.Serialize(ref r);
			
			bool ir = running;
			stream.Serialize(ref ir);
		}
		else {
			Vector3 fp = Vector3.zero;
			stream.Serialize(ref fp);
			
			float fva = 0f;
			stream.Serialize(ref fva);
			
			Quaternion fr = Quaternion.Euler(new Vector3(0, 0, 0));
			stream.Serialize(ref fr);
			transform.rotation = fr;
			
			transform.position = fp; // Spieler ist auf Äquator
			Vector3 axis = Vector3.Cross(Vector3.forward, transform.position).normalized;
			transform.RotateAround(Vector3.zero, axis, (verticalAngle * Mathf.Rad2Deg) + (-fva * Mathf.Rad2Deg));
			
			// Animate Player
			bool fir = false;
			stream.Serialize(ref fir);
			if (fir)
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
			Debug.Log("old Vertical: " + verticalMovement + " old Horizontal: " + horizontalMovement);
			float v;
			float h;
			do {
				v = verticalMovement;
				h = horizontalMovement;
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
			verticalMovement = v;
			horizontalMovement = h;
			Debug.Log("new Vertical: " + verticalMovement + " new Horizontal: " + horizontalMovement);
			yield return new WaitForSeconds(Random.value*5 + 5f);
		}
	}
	
	void Update () {
		
		// Gegner drehen mit dem Planeten..!
		if (Network.peerType != NetworkPeerType.Disconnected && !networkView.isMine && Static.rink != null) {
			
			if (vertAngleM != 0) { // an Wänden hängen bleiben..
				float vm;
				if (Static.player.isDead()) {
					vm = vertAngleM;
					vertAngleM = 0;
				} else {
					vm = Static.player.getSpeed() * Input.GetAxis("Vertical") * Time.deltaTime;
					vm = determineVerticalParcelPosition(Input.GetAxis("Vertical"), vm);
				}
				verticalAngle += vm;
				//verticalAngle = verticalAngle % (Mathf.PI*2);

				Vector3 axis = Vector3.Cross(Vector3.forward, transform.position);
				transform.RotateAround(Vector3.zero, axis, vm * Mathf.Rad2Deg);
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
					Debug.Log("HIT: "+hit.collider.gameObject.name);
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
		
		if ((Input.GetKeyDown(KeyCode.Plus)) || (Input.GetKeyDown(KeyCode.KeypadPlus))) {
			AudioSource audioSource = Static.menuHandler.gameObject.GetComponent<AudioSource>();
			if (audioSource.volume < 1f) {
				Static.menuHandler.gameObject.GetComponent<AudioSource>().volume += 0.1f;
				Preferences.setVolume(Static.menuHandler.gameObject.GetComponent<AudioSource>().volume);
			}
			Debug.Log("Audio volume set to " + audioSource.volume);
		}
		
		if ((Input.GetKeyDown(KeyCode.Minus)) || (Input.GetKeyDown(KeyCode.KeypadMinus))) {
			AudioSource audioSource = Static.menuHandler.gameObject.GetComponent<AudioSource>();
			if (audioSource.volume > 0f) {
				Static.menuHandler.gameObject.GetComponent<AudioSource>().volume -= 0.1f;
				Preferences.setVolume(Static.menuHandler.gameObject.GetComponent<AudioSource>().volume);
			}
			Debug.Log("Audio volume set to " + audioSource.volume);
		}
		
		if (Static.rink != null && !Static.player.isDead()) {
			
			// -----------------------------------------------------------
			// Bewegung und Bestimmung einer möglichen neuen currentParcel
			// -----------------------------------------------------------
			moveCharacter();
			currCell = Static.rink.gameArea[lpos][bpos];
			//currCell.colorCell(Color.cyan);
			//Debug.Log("[" + lpos + "][" + bpos + "]");
			
			if (currCell.hasContactMine()) {
				networkView.RPC("startEvent", RPCMode.All, currCell.getLpos(), currCell.getBpos(), 3);
			}
				
			if (currCell.isExploding()) {
				Static.player.setDead(true, networkView);
				//renderer.material.color = Color.black;
				StartCoroutine(deadPlayer());
				networkView.RPC("removePlayer", RPCMode.OthersBuffered, Network.player);
			}
			
			
			// Falls die Zelle ein Powerup enthält -> aufsammeln
			if (currCell.hasPowerup()) {
				networkView.RPC("startEvent", RPCMode.Others, currCell.getLpos(), currCell.getBpos(), 0);
				Static.player.powerupCollected(currCell.destroyPowerup(true, false));
			}
			
			// Leertaste -> Bombe legen
			if (Input.GetKeyDown(KeyCode.Space)) {
				if (!currCell.hasExplosion()) {
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
			
			if ((Input.GetKeyDown(KeyCode.LeftShift)) || (Input.GetKeyDown(KeyCode.RightShift))) {
				if (!currCell.hasExplosion()) {
					if (Static.player.addContactMine()) {
						GameObject ex = Network.Instantiate(Resources.Load("Prefabs/Bombe"), currCell.getCenterPos(), Quaternion.identity, 0) as GameObject;
						ex.networkView.RPC("createExplosionOnCell", RPCMode.All, currCell.getLpos(), currCell.getBpos(), 
					     	              Static.player.getFlamePower(), Static.player.getDelay(), Static.player.getSuperbomb(), 2, true, true);
						Static.menuHandler.playSound(Static.contactMineDropSoundEffect, false);
					}
				}
				Static.player.releaseTriggerBombs();
			}
			
		} else {
			// vor Scham im Boden versinken lassen ;)
			transform.position -= 0.023f * transform.position.normalized * Time.deltaTime;
			moveCharacter();
		}
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
		if (networkView.owner == p) {
			transform.localScale = Vector3.zero;
		}
	}
	
	private void moveCharacter() {
		
		float verticalMovement, vm=0, m=0;
		if (Static.player.isDead()) {
			verticalMovement = this.verticalMovement;
		} else {
			verticalMovement = Input.GetAxis("Vertical");
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
		
		float horizontalMovement;
		if (Static.player.isDead()) {
			horizontalMovement = this.horizontalMovement;
		} else {
			horizontalMovement = Input.GetAxis("Horizontal") * Static.player.getSpeed();
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
		int newAngle = angle;
		if (verticalMovement > 0) {
			// nach oben schauen
			if (horizontalMovement < 0) {
				// nach links oben schauen
				newAngle = 315;
			} else if (horizontalMovement > 0) {
				// nach rechts oben schauen
				newAngle = 45;
			} else {
				// nur nach oben schauen
				newAngle = 0;
			}
		} else if (verticalMovement < 0) {
			// nach unten schauen
			if (horizontalMovement < 0) {
				// nach links unten schauen
				newAngle = 225;
			} else if (horizontalMovement > 0) {
				// nach rechts unten schauen
				newAngle = 135;
			} else {
				// nur nach unten schauen
				newAngle = 180;
			}
		} else {
			if (horizontalMovement < 0) {
				// nur nach links schauen
				newAngle = 270;
			} else if (horizontalMovement > 0) {
				// nur nach rechts schauen
				newAngle = 90;
			}
		}
		
		if (Mathf.Abs(angle - newAngle) > 180)
			angle = (angle + newAngle) / 2 + 180;
		else
			angle = (angle + newAngle) / 2;
		
		transform.up = transform.position;
		transform.Rotate(0f, angle, 0f, Space.Self);
		
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
						return 0.0f;
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
						return 0.0f;
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
						return 0.0f;
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
						return 0.0f;
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
							return 0.0f;
						}
						newCell = Static.rink.gameArea[lpos][bpos+1];
					} else {
						if (Static.rink.gameArea[lpos][0].getType() != 0 || Static.rink.gameArea[lpos][0].hasBomb()) {
							return 0.0f;
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
							return 0.0f;
						}
						newCell = Static.rink.gameArea[lpos][bpos-1];
					} else {
						if (Static.rink.gameArea[lpos][n_B-1].getType() != 0 || Static.rink.gameArea[lpos][n_B-1].hasBomb()) {
							return 0.0f;
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
							return 0.0f;
						}
						newCell = Static.rink.gameArea[lpos][bpos-1];
					} else {
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
							return 0.0f;
						}
						newCell = Static.rink.gameArea[lpos][bpos+1];
					} else {
						if (Static.rink.gameArea[lpos][0].getType() != 0 || Static.rink.gameArea[lpos][0].hasBomb()) {
							return 0.0f;
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
		cam.transform.LookAt(Vector3.zero, Vector3.forward);
		
		// Licht mitdrehen..
		sun.transform.RotateAround(Vector3.zero, Vector3.forward, movement);
		sun.transform.eulerAngles = new Vector3(0,90,0);
	}
}
