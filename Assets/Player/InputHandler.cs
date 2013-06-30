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
	
	public GameObject camera;
	private Quaternion cameraRotation;
	
	private Parcel currCell;
	
	private float createTime;
	
	float verticalMovement;
	float horizontalMovement;
	
	void Awake() {
		playerHandler = GameObject.Find("Player");
	}
	
	// Use this for initialization
	void Start () {
		
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
			float distance = Vector3.Distance (GameObject.Find("Player").transform.position, position);
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
		while (Player.isDead()) {
			bool invalid = true;
			do {
				float oldVerticalMovement = verticalMovement;
				float oldHorizontalMovement = horizontalMovement;
				verticalMovement = ((float) new System.Random().Next(0, 3) - 1f) / 10;
				horizontalMovement = ((float) new System.Random().Next(0, 3) - 1f) / 10;
				
				bool noMovement = (verticalMovement == 0f && horizontalMovement == 0f);
				bool hardTurn = ((oldVerticalMovement - verticalMovement > 0.1) || (oldHorizontalMovement - horizontalMovement > 0.1)) ||
					((oldVerticalMovement - verticalMovement == 0.1) && (oldHorizontalMovement - horizontalMovement == 0.1));
				bool noChange = ((oldVerticalMovement == verticalMovement) && (oldHorizontalMovement == horizontalMovement));
				invalid = (noMovement || hardTurn || noChange);
				
				//Debug.Log ("verticalMovement: " + verticalMovement);
				//Debug.Log ("horizontalMovement: " + horizontalMovement);
			} while (invalid);
			yield return new WaitForSeconds(Random.value*5 + 5f);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
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
						if (Player.getTriggerbomb()) {
							Player.addTriggerBomb(currCell);
						}
					}
				}
			}
			
			if ((Input.GetKeyDown(KeyCode.LeftShift)) || (Input.GetKeyDown(KeyCode.RightShift))) {
				foreach (Parcel triggerBomb in Player.getTriggerBombs()) {
					triggerBomb.getExplosion().startExplosion();
					Debug.Log ("triggering " + triggerBomb.getCoordinates());
				}
				Player.getTriggerBombs().Clear();
			}

			
			if ((Time.time - createTime) > 1.0f) {
				createTime = Time.time;
				Player.increaseHP();
			}
		} else {
			moveCharacter();
		}
	}
	
	private void moveCharacter(){
		
		float verticalMovement;
		if (Player.isDead()) {
			verticalMovement = this.verticalMovement;
		} else {
			verticalMovement = Input.GetAxis("Vertical");
		}
		if ( verticalMovement != 0) {
			float m = Player.getSpeed() * verticalMovement * Time.deltaTime;
			if ( vDirection == 0) {
				
				vDirection = (int)Mathf.Sign(m);
				
				if ( vDirection == 1){
					verticalHelper -= 	Mathf.PI/(2*(n_L-1));
				} else{
					verticalHelper += 	Mathf.PI/(2*(n_L-1));
				}
			}
		
			float vAngle = verticalAngle;
			verticalAngle += m;

			if (!Player.isDead()) {
				m = determineVerticalParcelPosition( verticalMovement, m);
			}
			
			Static.sphereHandler.move(m);
			if ( m == 0) verticalAngle = vAngle;
		}
		
		float horizontalMovement;
		if (Player.isDead()) {
			horizontalMovement = this.horizontalMovement;
		} else {
			horizontalMovement = Input.GetAxis("Horizontal") * Player.getSpeed();
		}
		if ( horizontalMovement != 0){
			float m = horizontalMovement*Time.deltaTime*Player.getSpeed()*(-2);
			if ( hDirection == 0) {
				
				hDirection = (int)Mathf.Sign(m);
				
				if ( hDirection == 1){
					horizontalHelper += 	Mathf.PI/(n_B);
				} else{
					horizontalHelper -= 	Mathf.PI/(n_B);
				}
				horizontalHelper += m;
			}
		 
			float hAngle = horizontalAngle;
			horizontalAngle += m;
			
			if (!Player.isDead()) {
				m = determineHorizontalParcelPosition( horizontalMovement, m);
			}
			
			moveAlongEquator(m);
			if ( m == 0) horizontalAngle = hAngle;
			//rink.renderAll();	// 4Debug !!! Achtung: Muss im fertigen Spiel raus. Zieht locker 20 FPS!
		}
		
			Vector3 lookDirection = currCell.up.getCenterPos();

			// Spielerrotation
			int GAP = 2;
			if (verticalMovement > 0) {
				// nach oben schauen
				if (horizontalMovement < 0) {
					// nach links oben schauen
					//Debug.Log("links oben");
					lookDirection = currCell.getSurroundingCell(GAP,GAP).getCenterPos();
					currCell.getSurroundingCell(GAP,GAP).colorCell(Color.magenta);
				} else if (horizontalMovement > 0) {
					// nach rechts oben schauen
					//Debug.Log("rechts oben");
					lookDirection = currCell.getSurroundingCell(GAP,-GAP).getCenterPos();
					currCell.getSurroundingCell(GAP,-GAP).colorCell(Color.magenta);

				} else {
					// nur nach oben schauen
					//Debug.Log("oben");
					lookDirection = currCell.getSurroundingCell(GAP,0).getCenterPos();
					currCell.getSurroundingCell(GAP,0).colorCell(Color.magenta);
				}
			} else if (verticalMovement < 0) {
				// nach unten schauen
				if (horizontalMovement < 0) {
					// nach links unten schauen
					//Debug.Log("links unten");
					lookDirection = currCell.getSurroundingCell(-GAP,GAP).getCenterPos();
					currCell.getSurroundingCell(-GAP,GAP).colorCell(Color.magenta);
				} else if (horizontalMovement > 0) {
					// nach rechts unten schauen
					//Debug.Log("rechts unten");
					lookDirection = currCell.getSurroundingCell(-GAP,-GAP).getCenterPos();
					currCell.getSurroundingCell(-GAP,-GAP).colorCell(Color.magenta);
				} else {
					// nur nach unten schauen
					//Debug.Log("unten");
					lookDirection = currCell.getSurroundingCell(-GAP,0).getCenterPos();
					currCell.getSurroundingCell(-GAP,0).colorCell(Color.magenta);
				}
			} else {
				if (horizontalMovement < 0) {
					// nur nach links schauen
					//Debug.Log("links");
					lookDirection = currCell.getSurroundingCell(0,GAP).getCenterPos();
					currCell.getSurroundingCell(0,GAP).colorCell(Color.magenta);
				} else {
					// nur nach rechts schauen
					//Debug.Log("rechts");
					lookDirection = currCell.getSurroundingCell(0,-GAP).getCenterPos();
					currCell.getSurroundingCell(0,-GAP).colorCell(Color.magenta);
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
