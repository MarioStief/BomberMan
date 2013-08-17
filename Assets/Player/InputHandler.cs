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
	
	Vector3 lookDirection = new Vector3(0, 0, 0);
	int angle = 0;
	
	private GameObject playerHandler;
	
	private int n_L;				// Anzahl Längen und Breitengeraden
	private int n_B;
	
	private int lpos;				// Position der aktuellen Parzelle rink.gameArea ist [lpos][bpos]
	private int bpos;
	
	private float verticalAngle;	
	private float oldVerticalAngle;
	private float horizontalAngle;
	
	private static float vertAngle; // statische Kopie von verticalAngle
	
	private float verticalHelper;
	private float horizontalHelper;
	
	private int vDirection;			// Bewegungsrichtung
	private int hDirection;
	
	private GameObject camera;
	
	private Parcel currCell;
	
	private float createTime;
	
	float verticalMovement;
	float horizontalMovement;
	
	void Awake() {
		//playerHandler = GameObject.Find("Player");
		playerHandler = GameObject.FindGameObjectWithTag("Player");
		Static.setInputHandler(this);
	}
	
	void OnNetworkInstantiate(NetworkMessageInfo info) {
		if (info.sender != Network.player) {
		}
		Debug.Log("New object instantiated by " + info.sender);
	}
	
	// Use this for initialization
	void Start () {
		
		if (!networkView.isMine) {
			return;
		}
		
		camera = GameObject.FindGameObjectWithTag("MainCamera");
		
		// set my color
		renderer.material.color = Menu.getPlayerColor();
		
		Static.sphereHandler.move(0.000001f); // CK, fixed color on startup :)
		moveAlongEquator(0.000001f);
		
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
		
		//transform.position += new Vector3(0.21f, 0.21f, 0.21f);
	}
	
	public void playSound(AudioClip clip) {
		AudioSource audioSource = gameObject.GetComponent<AudioSource>();
		audioSource.clip = clip;
		//audioSource.GetComponent<AudioSource>().volume *= 2;
		audioSource.Play();
	}
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if (stream.isWriting)
		{
			Vector3 p = transform.position;
			stream.Serialize(ref p);
			
			float va = verticalAngle;
			stream.Serialize(ref va);
			
			Quaternion r = transform.rotation;
			stream.Serialize(ref r);
		}
		else
		{
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
		}
	
	}
	
	IEnumerator deadPlayer() {
		float createTime = Time.time;
		float elapsedTime = 0.0f;
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
			float scale = 1f - ((multiplicator - 10f) / 10f); // Range: 1 - 0
			//Debug.Log ("---> " + scale);
			playerHandler.transform.localScale *= scale;
			elapsedTime = Time.time - createTime;
		}
		playerHandler.transform.localScale = Vector3.zero;
		playerHandler.GetComponent<CapsuleCollider>().enabled = false;
		while (Static.player.isDead()) {
			do {			
				switch (Random.Range(0, 2)) {
				case 0:
					if (verticalMovement != 0f) {
						verticalMovement = 0f;
					} else {
						verticalMovement = (Random.Range(0, 2) == 0 ? 0.1f : -0.1f);
					}
					break;
				case 1:
					if (horizontalMovement != 0f) {
						horizontalMovement = 0f;
					} else {
						horizontalMovement = (Random.Range(0, 2) == 0 ? 0.1f : -0.1f);
					}
					break;
				}
			} while (verticalMovement == 0f && horizontalMovement == 0f);
			yield return new WaitForSeconds(Random.value*5 + 5f);
		}
	}
	
	void Update () {
		
		// Gegner drehen mit dem Planeten..!
		if (!networkView.isMine && Static.rink != null) {
			
			if (vertAngle != 0) { // an Wänden hängen bleiben..
				float verticalMovement = Input.GetAxis("Vertical");
				float vm = Static.player.getSpeed() * verticalMovement * Time.deltaTime;
				vm = determineVerticalParcelPosition(verticalMovement, vm);
				verticalAngle += vm;
				//verticalAngle = verticalAngle % (Mathf.PI*2);
				
				Vector3 axis = Vector3.Cross(Vector3.forward, transform.position);
				transform.RotateAround(Vector3.zero, axis, vm * Mathf.Rad2Deg);
			}
			
			return;
		}
		
		if (Static.rink != null && !Static.player.isDead()) {
			
			// -----------------------------------------------------------
			// Bewegung und Bestimmung einer möglichen neuen currentParcel
			// -----------------------------------------------------------
			moveCharacter();
			currCell = Static.rink.gameArea[lpos][bpos];
			//currCell.colorCell(Color.cyan);
			
			if (currCell.hasContactMine()) {
				currCell.getExplosion().startExplosion();
			}
				
			if (currCell.isExploding()) {
				Static.player.setDead(true);
				renderer.material.color = Color.black;
				StartCoroutine(deadPlayer());
				networkView.RPC("removePlayer", RPCMode.OthersBuffered, Network.player);
			}
			
			
			// Falls die Zelle ein Powerup enthält -> aufsammeln
			if (currCell.hasPowerup()) {
				networkView.RPC("destroyPowerup", RPCMode.Others, currCell.getLpos(), currCell.getBpos(), false);
				Static.player.powerupCollected(currCell.destroyPowerup(false));
			}
			
			// Leertaste -> Bombe legen
			if (Input.GetKeyDown(KeyCode.Space)) {
				if (!currCell.hasBomb() && !currCell.hasContactMine()) {
					//Static.player.addBomb();
					
					int extra = Static.player.addBomb();
					if (extra > -1) {
						GameObject ex = Network.Instantiate(Resources.Load("Prefabs/Bombe"), currCell.getCenterPos(), Quaternion.identity, 0) as GameObject;
						ex.networkView.RPC("createExplosionOnCell", RPCMode.All, currCell.getLpos(), currCell.getBpos(), 
					    	               Static.player.getFlamePower(), Static.player.getDelay(), Static.player.getSuperbomb(), extra, true, true);
						playSound(Static.bombLayedSoundEffect);
					}
				}
			}
			
			if ((Input.GetKeyDown(KeyCode.LeftShift)) || (Input.GetKeyDown(KeyCode.RightShift))) {
				if (!currCell.hasBomb() && !currCell.hasContactMine()) {
					if (Static.player.addContactMine()) {
						GameObject ex = Network.Instantiate(Resources.Load("Prefabs/Bombe"), currCell.getCenterPos(), Quaternion.identity, 0) as GameObject;
						ex.networkView.RPC("createExplosionOnCell", RPCMode.All, currCell.getLpos(), currCell.getBpos(), 
					     	              Static.player.getFlamePower(), Static.player.getDelay(), Static.player.getSuperbomb(), 2, true, true);
					}
				}
				Static.player.releaseTriggerBombs();
			}

			
			if ((Time.time - createTime) > 1.0f) {
				createTime = Time.time;
				Static.player.increaseHP();
			}
		} else {
			moveCharacter();
		}
	}
	
	[RPC]
	public void destroyPowerup(int lpos, int bpos, bool shatter) {
		Parcel cell = Static.rink.gameArea[lpos][bpos];
		cell.destroyPowerup(shatter);
	}
	[RPC]
	public void removePlayer(NetworkPlayer p) {
		if (networkView.owner == p) {
			transform.localScale = Vector3.zero;
		}
	}
	
	private void moveCharacter() {
		
		float verticalMovement;
		if (Static.player.isDead()) {
			verticalMovement = this.verticalMovement;
		} else {
			verticalMovement = Input.GetAxis("Vertical");
		}
		if (verticalMovement != 0) {
			float m = Static.player.getSpeed() * verticalMovement * Time.deltaTime;
			if (vDirection == 0) {
				
				vDirection = (int)Mathf.Sign(m);
				
				if (vDirection == 1) {
					verticalHelper -= 	Mathf.PI/(2*(n_L-1));
				} else {
					verticalHelper += 	Mathf.PI/(2*(n_L-1));
				}
			}
		
			if (!Static.player.isDead()) {
				m = determineVerticalParcelPosition(verticalMovement, m);
			}
			verticalAngle += m;
			//verticalAngle = verticalAngle % (Mathf.PI*2);
			
			Static.sphereHandler.move(m);
			vertAngle = m;
		}
		
		float horizontalMovement;
		if (Static.player.isDead()) {
			horizontalMovement = this.horizontalMovement;
		} else {
			horizontalMovement = Input.GetAxis("Horizontal") * Static.player.getSpeed();
		}
		if (horizontalMovement != 0) {
			float m = horizontalMovement*Time.deltaTime*Static.player.getSpeed()*(-2);
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
	}
	
	// <summary>
	// Bestimme, ob die Spielfigur einen neuen Würfel berührt. Wenn dem so ist, ändere currParcel auf
	// die Position des neuen Würfels.
	// </summary>
	private float determineVerticalParcelPosition(float verticalMovement, float m) {
		
		if (vDirection == 1 && Mathf.Sign(verticalMovement) == 1) {	// Bewegungsrichtung blieb gleich				
				//Debug.Log("#1");
			
				// Setting position of player in current cell
				float newPlayerPosition =  1-Mathf.Abs(verticalAngle-verticalHelper)/(Mathf.PI/(n_L-1)); //bs(verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Static.player.setXPos(newPlayerPosition);
				if (DEBUGPLAYERPOSITION)
					Debug.Log(newPlayerPosition);
			
				if (Mathf.Abs(verticalAngle - verticalHelper) > Mathf.PI/(n_L-1)) {				
				
					Parcel newCell;

					if (lpos < n_L-2) {
						if (Static.rink.gameArea[lpos+1][bpos].getType() != 0 || Static.rink.gameArea[lpos+1][bpos].hasBomb()) {
							return 0.0f;
						}
						newCell = Static.rink.gameArea[++lpos][bpos];
						Static.player.setXPos(0);

					} else {
						if (Static.rink.gameArea[0][bpos].getType() != 0 || Static.rink.gameArea[0][bpos].hasBomb()) {
							return 0.0f;
						}
						lpos = 0;
						newCell = Static.rink.gameArea[lpos][bpos];
						Static.player.setXPos(0);
					}
					verticalHelper += Mathf.PI/(n_L-1);
					Static.player.setCurrentParcel(newCell);	
				}
			} else if (vDirection == 1 && Mathf.Sign(verticalMovement) == -1) {	// Bewegungsrichtung ändert sich
				
				//Debug.Log("#2");
				float newPlayerPosition = Mathf.Abs(verticalAngle-verticalHelper)/(Mathf.PI/(n_L-1)); //bs(verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Static.player.setXPos(newPlayerPosition);
				if (DEBUGPLAYERPOSITION)
					Debug.Log(newPlayerPosition);
			
				vDirection = -1;
				verticalHelper += Mathf.PI/((n_L-1));
					
				if (Mathf.Abs(verticalAngle - verticalHelper) > Mathf.PI/(n_L-1)) {
					
					Parcel newCell;
					
						
					if (lpos > 0) {
						if (Static.rink.gameArea[lpos-1][bpos].getType() != 0 || Static.rink.gameArea[lpos-1][bpos].hasBomb()) {
							return 0.0f;
						}
						newCell = Static.rink.gameArea[--lpos][bpos];
					} else {
						if (Static.rink.gameArea[n_L-2][bpos].getType() != 0 || Static.rink.gameArea[n_L-2][bpos].hasBomb()) {
							return 0.0f;
						}
						lpos = n_L-2;
						newCell = Static.rink.gameArea[lpos][bpos];
					}
					verticalHelper -= Mathf.PI/(n_L-1);
					Static.player.setCurrentParcel(newCell);	
				}
			} else if (vDirection == -1 && Mathf.Sign(verticalMovement) == -1) {	// Bewegungsrichtung blieb gleich
				
				if (DEBUGPLAYERPOSITION)
					Debug.Log("#3");
				// Setting position of player in current cell
				float newPlayerPosition =  Mathf.Abs(verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Static.player.setXPos(newPlayerPosition);
				if (DEBUGPLAYERPOSITION)
					Debug.Log(newPlayerPosition);

				if (Mathf.Abs(verticalAngle - verticalHelper) > Mathf.PI/(n_L-1)) {
					
					Parcel newCell;
					
						
					if (lpos > 0) {
						if (Static.rink.gameArea[lpos-1][bpos].getType() != 0 || Static.rink.gameArea[lpos-1][bpos].hasBomb()) {
							return 0.0f;
						}
						newCell = Static.rink.gameArea[--lpos][bpos];
					} else {
						if (Static.rink.gameArea[n_L-2][bpos].getType() != 0 || Static.rink.gameArea[n_L-2][bpos].hasBomb()) {
							return 0.0f;
						}
						lpos = n_L -2;
						newCell = Static.rink.gameArea[lpos][bpos];
					}
					verticalHelper -= Mathf.PI/(n_L-1);
					Static.player.setCurrentParcel(newCell);	
				}
			} else if (vDirection == -1 && Mathf.Sign(verticalMovement) == 1) {	// Bewegungsrichtung ändert sich
				
				if (DEBUGPLAYERPOSITION)
					Debug.Log("#4");
				float newPlayerPosition =  1-Mathf.Abs(verticalAngle-verticalHelper)/(Mathf.PI/(n_L-1)); //bs(verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Static.player.setXPos(newPlayerPosition);
				if (DEBUGPLAYERPOSITION)
					Debug.Log(newPlayerPosition);
			
				vDirection = 1;
				verticalHelper -=	Mathf.PI/((n_L-1));
				
				if (Mathf.Abs(verticalAngle - verticalHelper) > Mathf.PI/(n_L-1)) {
					
					Parcel newCell;
					
						
					if (lpos < n_L-2) {
						if (Static.rink.gameArea[lpos+1][bpos].getType() != 0 || Static.rink.gameArea[lpos+1][bpos].hasBomb()) {
							return 0.0f;
						}
						newCell = Static.rink.gameArea[++lpos][bpos];
					} else {
						if (Static.rink.gameArea[0][bpos].getType() != 0 || Static.rink.gameArea[0][bpos].hasBomb()) {
							return 0.0f;
						}
						lpos = 0;
						newCell = Static.rink.gameArea[lpos][bpos];
					}
					verticalHelper += Mathf.PI/(n_L-1);
					Static.player.setCurrentParcel(newCell);	
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
			
			
				if (Mathf.Abs(horizontalAngle - horizontalHelper) > 2*Mathf.PI/n_B) {
					
					Parcel newCell;
					
						
					if (bpos < n_B-1) {
						if (Static.rink.gameArea[lpos][bpos+1].getType() != 0 || Static.rink.gameArea[lpos][bpos+1].hasBomb()) {
							return 0.0f;
						}
						newCell = Static.rink.gameArea[lpos][++bpos];
					} else {
						if (Static.rink.gameArea[lpos][0].getType() != 0 || Static.rink.gameArea[lpos][0].hasBomb()) {
							return 0.0f;
						}
						bpos = 0;
						newCell = Static.rink.gameArea[lpos][bpos];
					}
					horizontalHelper += 2*Mathf.PI/n_B;
					Static.player.setCurrentParcel(newCell);	
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
					
				if (Mathf.Abs(horizontalAngle - horizontalHelper) > 2*Mathf.PI/n_B) {
				
					Parcel newCell;
					
						
					if (bpos > 0) {
						if (Static.rink.gameArea[lpos][bpos-1].getType() != 0 || Static.rink.gameArea[lpos][bpos-1].hasBomb()) {
							return 0.0f;
						}
						newCell = Static.rink.gameArea[lpos][--bpos];
					} else {
						if (Static.rink.gameArea[lpos][n_B-1].getType() != 0 || Static.rink.gameArea[lpos][n_B-1].hasBomb()) {
							return 0.0f;
						}
						bpos = n_B;
						newCell = Static.rink.gameArea[lpos][--bpos];
					}
					horizontalHelper -= 2*Mathf.PI/n_B;
					Static.player.setCurrentParcel(newCell);	
				}
			} else if (hDirection == 1 && Mathf.Sign(horizontalMovement) == 1) {	// Bewegungsrichtung blieb gleich
				
				if (DEBUGPLAYERPOSITION)
					Debug.Log("#H3");
				float newPlayerPosition =  Mathf.Abs(horizontalAngle-horizontalHelper)/(2*Mathf.PI/n_B); //bs(verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Static.player.setZPos(newPlayerPosition);
				if (DEBUGPLAYERPOSITION)
					Debug.Log(newPlayerPosition);
			
				if (Mathf.Abs(horizontalAngle - horizontalHelper) > 2*Mathf.PI/n_B) {
					//Debug.Log("Treffer");
					Parcel newCell;
					
						
					if (bpos > 0) {
						if (Static.rink.gameArea[lpos][bpos-1].getType() != 0 || Static.rink.gameArea[lpos][bpos-1].hasBomb()) {
							return 0.0f;
						}
						newCell = Static.rink.gameArea[lpos][--bpos];
					} else {
						bpos = n_B;
						newCell = Static.rink.gameArea[lpos][--bpos];
					}
					horizontalHelper -= 2*Mathf.PI/n_B;
					Static.player.setCurrentParcel(newCell);	
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
			
				if (Mathf.Abs(horizontalAngle - horizontalHelper) > 2*Mathf.PI/n_B) {
					
					Parcel newCell;
					
						
					if (bpos < n_B-1) {
						if (Static.rink.gameArea[lpos][bpos+1].getType() != 0 || Static.rink.gameArea[lpos][bpos+1].hasBomb()) {
							return 0.0f;
						}
						newCell = Static.rink.gameArea[lpos][++bpos];
					} else {
						if (Static.rink.gameArea[lpos][0].getType() != 0 || Static.rink.gameArea[lpos][0].hasBomb()) {
							return 0.0f;
						}
						bpos = 0;
						newCell = Static.rink.gameArea[lpos][bpos];
					}
					horizontalHelper += 2*Mathf.PI/n_B;
					Static.player.setCurrentParcel(newCell);	
				}
			} 
		
		return m;

	}
	
	private void moveAlongEquator(float movement) {
		
		transform.position = new Vector3(Mathf.Cos(movement)* transform.position.x - Mathf.Sin(movement) * transform.position.y,
										Mathf.Sin(movement) * transform.position.x + Mathf.Cos(movement) * transform.position.y,
										transform.position.z);
		
		
		camera.transform.position = new Vector3(Mathf.Cos(movement)* camera.transform.position.x - Mathf.Sin(movement) * camera.transform.position.y,
										Mathf.Sin(movement) * camera.transform.position.x + Mathf.Cos(movement) * camera.transform.position.y,
										camera.transform.position.z);
		camera.transform.LookAt(Vector3.zero, Vector3.forward);
		
		// Licht mitdrehen..
		GameObject.FindGameObjectWithTag("Sun").transform.RotateAround(Vector3.zero, Vector3.forward, movement*Mathf.Rad2Deg);
		GameObject.FindGameObjectWithTag("Sun").transform.eulerAngles = new Vector3(0,90,0);
	}
	
	/*
	void OnParticleCollision(GameObject explosion) {
		Static.player.decreaseHP();
		if (Static.player.getHP() == 0) {
			renderer.material.color = Color.black;
			//moveDirection = new Vector3(0, 0, 0);
			GameObject deadPlayer = GameObject.Instantiate(Static.explosionPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity) as GameObject; 
		}
	}
	*/

}
