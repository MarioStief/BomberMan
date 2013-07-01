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
	
	private GameObject playerHandler;
	
	private int n_L;				// Anzahl Längen und Breitengeraden
	private int n_B;
	
	private int lpos;				// Position der aktuellen Parzelle rink.gameArea ist [lpos][bpos]
	private int bpos;
	
	private float verticalAngle;	
	private float oldVerticalAngle;
	private float horizontalAngle;
	
	private float verticalHelper;
	private float horizontalHelper;
	
	private int vDirection;			// Bewegungsrichtung
	private int hDirection;
	
	private GameObject camera;
	private Quaternion cameraRotation;
	
	private Parcel currCell;
	
	private float createTime;
	
	void Awake() {
		//playerHandler = GameObject.Find("Player");
		playerHandler = GameObject.FindGameObjectWithTag("Player");
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
		
		Static.sphereHandler.move(0.000001f); // CK, fixed color on startup :)
		moveAlongEquator(0.000001f);
		
		createTime = Time.time;
		
		
		n_L = Static.sphereHandler.n_L;
		n_B = Static.sphereHandler.n_B;
		
		lpos = n_L/2-1;
		bpos = n_B/4;
		
		currCell = Static.rink.gameArea[lpos][bpos];
		Player.setCurrentParcel(currCell);
		
		vDirection = 0;
		hDirection = 0;
		
		verticalAngle = 0.0f;
		horizontalAngle = 0.0f;
		
		verticalHelper = 0.0f;
		horizontalHelper = 0.0f;
		
		cameraRotation = camera.transform.rotation;
		
		transform.LookAt(currCell.up.getCenterPos());
	}
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if (stream.isWriting)
		{
			// calculate my position on sphere
			/*Vector3 p = new Vector3(
				transform.position.x,
				Mathf.Cos(verticalAngle) * transform.position.y - Mathf.Sin(verticalAngle) * transform.position.z,
				Mathf.Sin(verticalAngle) * transform.position.y + Mathf.Cos(verticalAngle) * transform.position.z
			);*/
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
			transform.position = new Vector3(
				fp.x,
				Mathf.Cos(fva-verticalAngle) * fp.y - Mathf.Sin(fva-verticalAngle) * fp.z,
				Mathf.Sin(fva-verticalAngle) * fp.y + Mathf.Cos(fva-verticalAngle) * fp.z
			);
			
			Quaternion fr = Quaternion.Euler(new Vector3(0, 0, 0));
			stream.Serialize(ref fr);
			transform.rotation = fr;
		}
	
	}
	
	IEnumerator deadPlayer() {
		while (true) {
			float x = transform.position.x;
			float y = transform.position.y;
			float z = transform.position.z;
			yield return new WaitForSeconds(Random.value);
			Vector3 position = new Vector3(Random.Range(x-0.1f, x+0.1f), Random.Range(y-0.1f, y+0.1f), Random.Range(z-0.1f, z+0.1f));
			GameObject explosion = GameObject.Instantiate(Static.explosionPrefab, position, Quaternion.identity) as GameObject;
			Detonator detonator = explosion.GetComponent<Detonator>();
			detonator.setSize(Random.Range(500f, 1000f));
			detonator.setDuration(5f);
			float distance = Vector3.Distance (GameObject.Find("Player").transform.position, position);
			detonator.GetComponent<AudioSource>().volume /= 20*distance;
			detonator.GetComponent<AudioSource>().Play();
			detonator.Explode();
		}
	}
	
	void Update () {
		
		// Gegner drehen mit dem Planeten..!
		if (!networkView.isMine) {
			
			float verticalMovement = Input.GetAxis("Vertical");
			float vm = Player.getSpeed() * verticalMovement * Time.deltaTime;
			vm = determineVerticalParcelPosition( verticalMovement, vm);
			verticalAngle += vm;
			if (vm != 0) {
				transform.position = new Vector3(
					transform.position.x,
					Mathf.Cos(-vm) * transform.position.y - Mathf.Sin(-vm) * transform.position.z,
					Mathf.Sin(-vm) * transform.position.y + Mathf.Cos(-vm) * transform.position.z
				);
			}
			
			return;
		}
		
		if (!Player.isDead()) {
			
			// -----------------------------------------------------------
			// Bewegung und Bestimmung einer möglichen neuen currentParcel
			// -----------------------------------------------------------
			moveCharacter();
			currCell = Static.rink.gameArea[lpos][bpos];
			if (currCell.isExploding()) {
				Player.setDead(true);
				renderer.material.color = Color.black;
				StartCoroutine(deadPlayer());
			}
			
			
			// Falls die Zelle ein Powerup enthält -> aufsammeln
			if (currCell.hasPowerup()) {
				Player.powerupCollected(currCell.destroyPowerup(false));
			}
			
			// Leertaste -> Bombe legen
			if ( Input.GetKeyDown(KeyCode.Space)){
				if ( !currCell.hasBomb()) {
					
					if (Player.addBomb()) {
						Explosion.createExplosionOnCell(currCell, Player.getFlamePower(), Player.getDelay(), true, true);
						// Um eine Bombe eines anderen Spielers auf einer Zelle zu spawnen:
						// Explosion.createExplosionOnCell(Parcel, flamePower, true);
						// Powerup-ToDos: flameMight, flameSpeed
					}
				}
			}
			
			if ((Time.time - createTime) > 1.0f) {
				createTime = Time.time;
				Player.increaseHP();
			}
		}
	}
	
	private void moveCharacter(){
		
		float verticalMovement = Input.GetAxis("Vertical");
		float vm = Player.getSpeed() * verticalMovement * Time.deltaTime;
		if ( verticalMovement != 0) {
			
			if ( vDirection == 0) {
				
				vDirection = (int)Mathf.Sign(vm);
				
				if ( vDirection == 1){
					verticalHelper -= 	Mathf.PI/(2*(n_L-1));
				} else{
					verticalHelper += 	Mathf.PI/(2*(n_L-1));
				}
			}
		
			float vAngle = verticalAngle;
			verticalAngle += vm;

			vm = determineVerticalParcelPosition( verticalMovement, vm);
			
			Static.sphereHandler.move(vm);
			if ( vm == 0) verticalAngle = vAngle;
		}
		
		
		
		float horizontalMovement = Input.GetAxis("Horizontal") * Player.getSpeed();
		float hm = 0f;
		if ( horizontalMovement != 0){
			hm = horizontalMovement*Time.deltaTime*Player.getSpeed()*(-2);
			if ( hDirection == 0) {
				
				hDirection = (int)Mathf.Sign(hm);
				
				if ( hDirection == 1){
					horizontalHelper += 	Mathf.PI/(n_B);
				} else{
					horizontalHelper -= 	Mathf.PI/(n_B);
				}
				horizontalHelper += hm;
			}
		 
			float hAngle = horizontalAngle;
			horizontalAngle += hm;
			

			hm = determineHorizontalParcelPosition( horizontalMovement, hm);
			
			moveAlongEquator( hm);
			if ( hm == 0) horizontalAngle = hAngle;
			//rink.renderAll();	// 4Debug !!! Achtung: Muss im fertigen Spiel raus. Zieht locker 20 FPS!
		}
		
			Vector3 lookDirection = currCell.up.getCenterPos();

			// Spielerrotation
			int GAP = 2;
			if (vm > 0) {
				// nach oben schauen
				if (hm > 0) {
					// nach links oben schauen
					lookDirection = currCell.getSurroundingCell(GAP,GAP).getCenterPos();
				} else if (hm < 0) {
					// nach rechts oben schauen
					lookDirection = currCell.getSurroundingCell(GAP,-GAP).getCenterPos();
				} else {
					// nur nach oben schauen
					lookDirection = currCell.getSurroundingCell(GAP,0).getCenterPos();
				}
			} else if (vm < 0) {
				// nach unten schauen
				if (hm > 0) {
					// nach links unten schauen
					lookDirection = currCell.getSurroundingCell(-GAP,GAP).getCenterPos();
				} else if (hm < 0) {
					// nach rechts unten schauen
					lookDirection = currCell.getSurroundingCell(-GAP,-GAP).getCenterPos();
				} else {
					// nur nach unten schauen
					lookDirection = currCell.getSurroundingCell(-GAP,0).getCenterPos();
				}
			} else {
				if (hm > 0) {
					// nur nach links schauen
					lookDirection = currCell.getSurroundingCell(0,GAP).getCenterPos();
				} else {
					// nur nach rechts schauen
					lookDirection = currCell.getSurroundingCell(0,-GAP).getCenterPos();
				}
			}
		
			transform.LookAt(lookDirection);
		
	}
	
	// <summary>
	// Bestimme, ob die Spielfigur einen neuen Würfel berührt. Wenn dem so ist, ändere currParcel auf
	// die Position des neuen Würfels.
	// </summary>
	private float determineVerticalParcelPosition(float verticalMovement, float m){
		
		if ( vDirection == 1 && Mathf.Sign(verticalMovement) == 1){	// Bewegungsrichtung blieb gleich				
				//Debug.Log("#1");
			
				// Setting position of player in current cell
				float newPlayerPosition =  1-Mathf.Abs(verticalAngle-verticalHelper)/(Mathf.PI/(n_L-1)); //bs( verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Player.setXPos( newPlayerPosition);
				if (DEBUGPLAYERPOSITION)
					Debug.Log(newPlayerPosition);
			
				if (Mathf.Abs( verticalAngle - verticalHelper) > Mathf.PI/(n_L-1)){				
				
					Parcel newCell;

					if ( lpos < n_L-2){
						if ( Static.rink.gameArea[lpos+1][bpos].getType() != 0){
							return 0.0f;
						}
						newCell = Static.rink.gameArea[++lpos][bpos];
						Player.setXPos( 0);

					} else{
						if ( Static.rink.gameArea[0][bpos].getType() != 0){
							return 0.0f;
						}
						lpos = 0;
						newCell = Static.rink.gameArea[lpos][bpos];
						Player.setXPos( 0);
					}
					verticalHelper += Mathf.PI/(n_L-1);
					Player.setCurrentParcel(newCell);	
				}
			} else if (vDirection == 1 && Mathf.Sign(verticalMovement) == -1){	// Bewegungsrichtung ändert sich
				
				//Debug.Log("#2");
				float newPlayerPosition =  Mathf.Abs(verticalAngle-verticalHelper)/(Mathf.PI/(n_L-1)); //bs( verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Player.setXPos( newPlayerPosition);
				if (DEBUGPLAYERPOSITION)
					Debug.Log(newPlayerPosition);
			
				vDirection = -1;
				verticalHelper +=	Mathf.PI/((n_L-1));
					
				if (Mathf.Abs( verticalAngle - verticalHelper) > Mathf.PI/(n_L-1)){
					
					Parcel newCell;
					
						
					if ( lpos > 0){
						if ( Static.rink.gameArea[lpos-1][bpos].getType() != 0){
							return 0.0f;
						}
						newCell = Static.rink.gameArea[--lpos][bpos];
					} else{
						if ( Static.rink.gameArea[n_L-2][bpos].getType() != 0){
							return 0.0f;
						}
						lpos = n_L-2;
						newCell = Static.rink.gameArea[lpos][bpos];
					}
					verticalHelper -= Mathf.PI/(n_L-1);
					Player.setCurrentParcel(newCell);	
				}
			} else if ( vDirection == -1 && Mathf.Sign(verticalMovement) == -1){	// Bewegungsrichtung blieb gleich
				
				if (DEBUGPLAYERPOSITION)
					Debug.Log("#3");
				// Setting position of player in current cell
				float newPlayerPosition =  Mathf.Abs( verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Player.setXPos( newPlayerPosition);
				if (DEBUGPLAYERPOSITION)
					Debug.Log(newPlayerPosition);

				if (Mathf.Abs( verticalAngle - verticalHelper) > Mathf.PI/(n_L-1)){
					
					Parcel newCell;
					
						
					if ( lpos > 0){
						if ( Static.rink.gameArea[lpos-1][bpos].getType() != 0){
							return 0.0f;
						}
						newCell = Static.rink.gameArea[--lpos][bpos];
					} else{
						if ( Static.rink.gameArea[n_L-2][bpos].getType() != 0){
							return 0.0f;
						}
						lpos = n_L -2;
						newCell = Static.rink.gameArea[lpos][bpos];
					}
					verticalHelper -= Mathf.PI/(n_L-1);
					Player.setCurrentParcel(newCell);	
				}
			} else if (vDirection == -1 && Mathf.Sign(verticalMovement) == 1){	// Bewegungsrichtung ändert sich
				
				if (DEBUGPLAYERPOSITION)
					Debug.Log("#4");
				float newPlayerPosition =  1-Mathf.Abs(verticalAngle-verticalHelper)/(Mathf.PI/(n_L-1)); //bs( verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Player.setXPos( newPlayerPosition);
				if (DEBUGPLAYERPOSITION)
					Debug.Log(newPlayerPosition);
			
				vDirection = 1;
				verticalHelper -=	Mathf.PI/((n_L-1));
				
				if (Mathf.Abs( verticalAngle - verticalHelper) > Mathf.PI/(n_L-1)){
					
					Parcel newCell;
					
						
					if ( lpos < n_L-2){
						if ( Static.rink.gameArea[lpos+1][bpos].getType() != 0){
							return 0.0f;
						}
						newCell = Static.rink.gameArea[++lpos][bpos];
					} else{
						if ( Static.rink.gameArea[0][bpos].getType() != 0){
							return 0.0f;
						}
						lpos = 0;
						newCell = Static.rink.gameArea[lpos][bpos];
					}
					verticalHelper += Mathf.PI/(n_L-1);
					Player.setCurrentParcel(newCell);	
				}
			} 
		
		return m;
	}
	
	private float determineHorizontalParcelPosition(float horizontalMovement, float m){
		
		if ( hDirection == -1 && Mathf.Sign(horizontalMovement) == -1){	// Bewegungsrichtung blieb gleich				
				if (DEBUGPLAYERPOSITION)
					Debug.Log("#H1");
			
				float newPlayerPosition =  1-Mathf.Abs(horizontalAngle-horizontalHelper)/(2*Mathf.PI/n_B); //bs( verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Player.setZPos( newPlayerPosition);
				if (DEBUGPLAYERPOSITION)
					Debug.Log(newPlayerPosition);
			
			
				if (Mathf.Abs( horizontalAngle - horizontalHelper) > 2*Mathf.PI/n_B){
					
					Parcel newCell;
					
						
					if ( bpos < n_B-1){
						if ( Static.rink.gameArea[lpos][bpos+1].getType() != 0){
							return 0.0f;
						}
						newCell = Static.rink.gameArea[lpos][++bpos];
					} else{
						if ( Static.rink.gameArea[lpos][0].getType() != 0){
							return 0.0f;
						}
						bpos = 0;
						newCell = Static.rink.gameArea[lpos][bpos];
					}
					horizontalHelper += 2*Mathf.PI/n_B;
					Player.setCurrentParcel(newCell);	
				}
			} else if (hDirection == -1 && Mathf.Sign(horizontalMovement) == 1){	// Bewegungsrichtung ändert sich
				
				if (DEBUGPLAYERPOSITION)
					Debug.Log("#H2");
				
			
				hDirection = 1;
				horizontalHelper +=	2*Mathf.PI/(n_B);
			
				float newPlayerPosition =  Mathf.Abs(horizontalAngle-horizontalHelper)/(2*Mathf.PI/n_B); //bs( verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Player.setZPos( newPlayerPosition);
				if (DEBUGPLAYERPOSITION)
					Debug.Log(newPlayerPosition);
					
				if (Mathf.Abs( horizontalAngle - horizontalHelper) > 2*Mathf.PI/n_B){
				
					Parcel newCell;
					
						
					if ( bpos > 0){
						if ( Static.rink.gameArea[lpos][bpos-1].getType() != 0){
							return 0.0f;
						}
						newCell = Static.rink.gameArea[lpos][--bpos];
					} else{
						if ( Static.rink.gameArea[lpos][n_B-1].getType() != 0){
							return 0.0f;
						}
						bpos = n_B;
						newCell = Static.rink.gameArea[lpos][--bpos];
					}
					horizontalHelper -= 2*Mathf.PI/n_B;
					Player.setCurrentParcel(newCell);	
				}
			} else if ( hDirection == 1 && Mathf.Sign(horizontalMovement) == 1){	// Bewegungsrichtung blieb gleich
				
				if (DEBUGPLAYERPOSITION)
					Debug.Log("#H3");
				float newPlayerPosition =  Mathf.Abs(horizontalAngle-horizontalHelper)/(2*Mathf.PI/n_B); //bs( verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Player.setZPos( newPlayerPosition);
				if (DEBUGPLAYERPOSITION)
					Debug.Log(newPlayerPosition);
			
				if (Mathf.Abs( horizontalAngle - horizontalHelper) > 2*Mathf.PI/n_B){
					//Debug.Log("Treffer");
					Parcel newCell;
					
						
					if ( bpos > 0){
						if ( Static.rink.gameArea[lpos][bpos-1].getType() != 0){
							return 0.0f;
						}
						newCell = Static.rink.gameArea[lpos][--bpos];
					} else{
						bpos = n_B;
						newCell = Static.rink.gameArea[lpos][--bpos];
					}
					horizontalHelper -= 2*Mathf.PI/n_B;
					Player.setCurrentParcel(newCell);	
				}
			} else if (hDirection == 1 && Mathf.Sign(horizontalMovement) == -1){	// Bewegungsrichtung ändert sich
				
				if (DEBUGPLAYERPOSITION)
					Debug.Log("#H4");
				
			
				hDirection = -1;
				horizontalHelper -=	2*Mathf.PI/(n_B);
			
				float newPlayerPosition =  1-Mathf.Abs(horizontalAngle-horizontalHelper)/(2*Mathf.PI/n_B); //bs( verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Player.setZPos( newPlayerPosition);
				if (DEBUGPLAYERPOSITION)
					Debug.Log(newPlayerPosition);
			
				if (Mathf.Abs( horizontalAngle - horizontalHelper) > 2*Mathf.PI/n_B){
					
					Parcel newCell;
					
						
					if ( bpos < n_B-1){
						if ( Static.rink.gameArea[lpos][bpos+1].getType() != 0){
							return 0.0f;
						}
						newCell = Static.rink.gameArea[lpos][++bpos];
					} else{
						if ( Static.rink.gameArea[lpos][0].getType() != 0){
							return 0.0f;
						}
						bpos = 0;
						newCell = Static.rink.gameArea[lpos][bpos];
					}
					horizontalHelper += 2*Mathf.PI/n_B;
					Player.setCurrentParcel(newCell);	
				}
			} 
		
		return m;

	}
	
	private void moveAlongEquator(float movement){
	
		transform.position = new Vector3(Mathf.Cos(movement)* transform.position.x - Mathf.Sin(movement) * transform.position.y,
										Mathf.Sin(movement) * transform.position.x + Mathf.Cos(movement) * transform.position.y,
										transform.position.z);
		
		
		camera.transform.position = new Vector3(Mathf.Cos(movement)* camera.transform.position.x - Mathf.Sin(movement) * camera.transform.position.y,
										Mathf.Sin(movement) * camera.transform.position.x + Mathf.Cos(movement) * camera.transform.position.y,
										camera.transform.position.z);
		camera.transform.LookAt(Vector3.zero, Vector3.forward);
	}
	
	void OnParticleCollision(GameObject explosion) {
		Player.decreaseHP();
		if (Player.getHP() == 0) {
			renderer.material.color = Color.black;
			//moveDirection = new Vector3(0, 0, 0);
			GameObject deadPlayer = GameObject.Instantiate(Static.explosionPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity) as GameObject; 
		}
	}

}
