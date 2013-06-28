using UnityEngine;
using System.Collections.Generic;
using AssemblyCSharp;

// <summary>
// InputHandler nimmt jeglichen relevanten Input entgegen und verarbeitet diesen
// bzw. leiten ihn an verarbeitende Klassen weiter.
// </summary>

public class InputHandler : MonoBehaviour {
	

	public GameObject sphere;
	private GameObject playerHandler;
	private SphereBuilder sphereHandler;
	
	public Rink rink;
	
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
	private static GameObject deadPlayerPrefab;
	
	private float createTime;
	
	void Awake() {
		deadPlayerPrefab = GameObject.Find("DeadPlayer");
		sphere = playerHandler = GameObject.Find("Sphere");
		sphereHandler = sphere.GetComponent<SphereBuilder>();
		playerHandler = GameObject.Find("Player");
	}
	
	// Use this for initialization
	void Start () {
		
		createTime = Time.time;
		
		
		n_L = sphereHandler.n_L;
		n_B = sphereHandler.n_B;
		
		lpos = n_L/2-1;
		bpos = n_B/4;
		
		Player.setCurrentParcel( rink.gameArea[lpos][bpos]);
		
		vDirection = 0;
		hDirection = 0;
		
		verticalAngle = 0.0f;
		horizontalAngle = 0.0f;
		
		verticalHelper = 0.0f;
		horizontalHelper = 0.0f;
		
		cameraRotation = camera.transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
	
		if (!Player.isDead()) {
			
			// -----------------------------------------------------------
			// Bewegung und Bestimmung einer möglichen neuen currentParcel
			// -----------------------------------------------------------
			moveCharacter();
			currCell = rink.gameArea[lpos][bpos];
			
			
			// Falls die Zelle ein Powerup enthält -> aufsammeln
			if (currCell.hasPowerup()) {
				Player.powerupCollected(currCell.destroyPowerup());
			}
			
			// Leertaste -> Bombe legen
			if ( Input.GetKeyDown(KeyCode.Space)){
				if ( !currCell.hasBomb()) {
					
					if (Player.addBomb()) {
						Explosion.createExplosionOnCell(currCell, Player.getFlamePower(), true);
						// Um eine Bombe eines anderen Spielers auf einer Zelle zu spawnen:
						// Explosion.createExplosionOnCell(Parcel, flamePower);
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
		float m = Player.getSpeed() * verticalMovement * Time.deltaTime;
		
		if ( verticalMovement != 0){
			
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

			m = determineVerticalParcelPosition( verticalMovement, m);
			
			sphereHandler.move(m);
			if ( m == 0) verticalAngle = vAngle;
		}
		
		
		
		float horizontalMovement = Input.GetAxis("Horizontal") * Player.getSpeed();
		if ( horizontalMovement != 0){
			m = horizontalMovement*Time.deltaTime*Player.getSpeed()*(-2);
			
			if ( hDirection == 0) {
				
				hDirection = (int)Mathf.Sign(m);
				
				if ( hDirection == 1){
					horizontalHelper += 	0;//Mathf.PI/(n_B);
				} else{
					horizontalHelper -= 	0;//Mathf.PI/(n_B);
				}
				horizontalHelper +=m;
			}
		 
			float hAngle = horizontalAngle;
			horizontalAngle += m;
			

			m = determineHorizontalParcelPosition( horizontalMovement, m);
			
			moveAlongEquator( m);
			if ( m == 0) horizontalAngle = hAngle;
			rink.renderAll();	// 4Debug !!! Achtung: Muss im fertigen Spiel raus. Zieht locker 20 FPS!

		}
		
		
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
				Debug.Log(newPlayerPosition);
			
				if (Mathf.Abs( verticalAngle - verticalHelper) > Mathf.PI/(n_L-1)){				
				
					Parcel newCell;

					if ( lpos < n_L-2){
						if ( rink.gameArea[lpos+1][bpos].getType() != 0){
							return 0.0f;
						}
						newCell = rink.gameArea[++lpos][bpos];
						Player.setXPos( 0);

					} else{
						if ( rink.gameArea[0][bpos].getType() != 0){
							return 0.0f;
						}
						lpos = 0;
						newCell = rink.gameArea[lpos][bpos];
						Player.setXPos( 0);
					}
					verticalHelper += Mathf.PI/(n_L-1);
					Player.setCurrentParcel(newCell);	
				}
			} else if (vDirection == 1 && Mathf.Sign(verticalMovement) == -1){	// Bewegungsrichtung ändert sich
				
				//Debug.Log("#2");
				float newPlayerPosition =  Mathf.Abs(verticalAngle-verticalHelper)/(Mathf.PI/(n_L-1)); //bs( verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Player.setXPos( newPlayerPosition);
				Debug.Log(newPlayerPosition);
			
				vDirection = -1;
				verticalHelper +=	Mathf.PI/((n_L-1));
					
				if (Mathf.Abs( verticalAngle - verticalHelper) > Mathf.PI/(n_L-1)){
					
					Parcel newCell;
					
						
					if ( lpos > 0){
						if ( rink.gameArea[lpos-1][bpos].getType() != 0){
							return 0.0f;
						}
						newCell = rink.gameArea[--lpos][bpos];
					} else{
						if ( rink.gameArea[n_L-2][bpos].getType() != 0){
							return 0.0f;
						}
						lpos = n_L-2;
						newCell = rink.gameArea[lpos][bpos];
					}
					verticalHelper -= Mathf.PI/(n_L-1);
					Player.setCurrentParcel(newCell);	
				}
			} else if ( vDirection == -1 && Mathf.Sign(verticalMovement) == -1){	// Bewegungsrichtung blieb gleich
				
				//Debug.Log("#3");
				// Setting position of player in current cell
				float newPlayerPosition =  Mathf.Abs( verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Player.setXPos( newPlayerPosition);
				Debug.Log(newPlayerPosition);

				if (Mathf.Abs( verticalAngle - verticalHelper) > Mathf.PI/(n_L-1)){
					
					Parcel newCell;
					
						
					if ( lpos > 0){
						if ( rink.gameArea[lpos-1][bpos].getType() != 0){
							return 0.0f;
						}
						newCell = rink.gameArea[--lpos][bpos];
					} else{
						if ( rink.gameArea[n_L-2][bpos].getType() != 0){
							return 0.0f;
						}
						lpos = n_L -2;
						newCell = rink.gameArea[lpos][bpos];
					}
					verticalHelper -= Mathf.PI/(n_L-1);
					Player.setCurrentParcel(newCell);	
				}
			} else if (vDirection == -1 && Mathf.Sign(verticalMovement) == 1){	// Bewegungsrichtung ändert sich
				
				//Debug.Log("#4");
				float newPlayerPosition =  1-Mathf.Abs(verticalAngle-verticalHelper)/(Mathf.PI/(n_L-1)); //bs( verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Player.setXPos( newPlayerPosition);
				Debug.Log(newPlayerPosition);
			
				vDirection = 1;
				verticalHelper -=	Mathf.PI/((n_L-1));
				
				if (Mathf.Abs( verticalAngle - verticalHelper) > Mathf.PI/(n_L-1)){
					
					Parcel newCell;
					
						
					if ( lpos < n_L-2){
						if ( rink.gameArea[lpos+1][bpos].getType() != 0){
							return 0.0f;
						}
						newCell = rink.gameArea[++lpos][bpos];
					} else{
						if ( rink.gameArea[0][bpos].getType() != 0){
							return 0.0f;
						}
						lpos = 0;
						newCell = rink.gameArea[lpos][bpos];
					}
					verticalHelper += Mathf.PI/(n_L-1);
					Player.setCurrentParcel(newCell);	
				}
			} 
		
		return m;
	}
	
	private float determineHorizontalParcelPosition(float horizontalMovement, float m){
		
		if ( hDirection == -1 && Mathf.Sign(horizontalMovement) == -1){	// Bewegungsrichtung blieb gleich				
				Debug.Log("#H1");
			
				float newPlayerPosition =  1-Mathf.Abs(horizontalAngle-horizontalHelper)/(2*Mathf.PI/n_B); //bs( verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Player.setZPos( newPlayerPosition);
				Debug.Log(newPlayerPosition);
			
			
				if (Mathf.Abs( horizontalAngle - horizontalHelper) > 2*Mathf.PI/n_B){
					
					Parcel newCell;
					
						
					if ( bpos < n_B-1){
						if ( rink.gameArea[lpos][bpos+1].getType() != 0){
							return 0.0f;
						}
						newCell = rink.gameArea[lpos][++bpos];
					} else{
						if ( rink.gameArea[lpos][0].getType() != 0){
							return 0.0f;
						}
						bpos = 0;
						newCell = rink.gameArea[lpos][bpos];
					}
					horizontalHelper += 2*Mathf.PI/n_B;
					Player.setCurrentParcel(newCell);	
				}
			} else if (hDirection == -1 && Mathf.Sign(horizontalMovement) == 1){	// Bewegungsrichtung ändert sich
				
				Debug.Log("#H2");
				
			
				hDirection = 1;
				horizontalHelper +=	Mathf.PI/(n_B);
			
				float newPlayerPosition =  Mathf.Abs(horizontalAngle-horizontalHelper)/(2*Mathf.PI/n_B); //bs( verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Player.setZPos( newPlayerPosition);
				Debug.Log(newPlayerPosition);
					
				if (Mathf.Abs( horizontalAngle - horizontalHelper) > 2*Mathf.PI/n_B){
				
					Parcel newCell;
					
						
					if ( bpos > 0){
						if ( rink.gameArea[lpos][bpos-1].getType() != 0){
							return 0.0f;
						}
						newCell = rink.gameArea[lpos][--bpos];
					} else{
						if ( rink.gameArea[lpos][n_B-1].getType() != 0){
							return 0.0f;
						}
						bpos = n_B;
						newCell = rink.gameArea[lpos][--bpos];
					}
					horizontalHelper -= 2*Mathf.PI/n_B;
					Player.setCurrentParcel(newCell);	
				}
			} else if ( hDirection == 1 && Mathf.Sign(horizontalMovement) == 1){	// Bewegungsrichtung blieb gleich
				
				Debug.Log("#H3");
				float newPlayerPosition =  Mathf.Abs(horizontalAngle-horizontalHelper)/(2*Mathf.PI/n_B); //bs( verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Player.setZPos( newPlayerPosition);
				Debug.Log(newPlayerPosition);
			
				if (Mathf.Abs( horizontalAngle - horizontalHelper) > 2*Mathf.PI/n_B){
					//Debug.Log("Treffer");
					Parcel newCell;
					
						
					if ( bpos > 0){
						if ( rink.gameArea[lpos][bpos-1].getType() != 0){
							return 0.0f;
						}
						newCell = rink.gameArea[lpos][--bpos];
					} else{
						bpos = n_B;
						newCell = rink.gameArea[lpos][--bpos];
					}
					horizontalHelper -= 2*Mathf.PI/n_B;
					Player.setCurrentParcel(newCell);	
				}
			} else if (hDirection == 1 && Mathf.Sign(horizontalMovement) == -1){	// Bewegungsrichtung ändert sich
				
				Debug.Log("#H4");
				
			
				hDirection = -1;
				horizontalHelper -=	Mathf.PI/(n_B);
			
				float newPlayerPosition =  1-Mathf.Abs(horizontalAngle-horizontalHelper)/(2*Mathf.PI/n_B); //bs( verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Player.setZPos( newPlayerPosition);
				Debug.Log(newPlayerPosition);
			
				if (Mathf.Abs( horizontalAngle - horizontalHelper) > 2*Mathf.PI/n_B){
					
					Parcel newCell;
					
						
					if ( bpos < n_B-1){
						if ( rink.gameArea[lpos][bpos+1].getType() != 0){
							return 0.0f;
						}
						newCell = rink.gameArea[lpos][++bpos];
					} else{
						if ( rink.gameArea[lpos][0].getType() != 0){
							return 0.0f;
						}
						bpos = 0;
						newCell = rink.gameArea[lpos][bpos];
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
		
		camera.transform.LookAt(sphere.transform);
	}
	
	void OnParticleCollision(GameObject explosion) {
		Player.decreaseHP();
		if (Player.getHP() == 0) {
			renderer.material.color = Color.black;
			//moveDirection = new Vector3(0, 0, 0);
			//GameObject deadPlayer = GameObject.Instantiate(deadPlayerPrefab, new Vector3(currCell.getXPos() + 0.5f, 0.3f, currCell.getZPos() + 0.5f), Quaternion.identity) as GameObject; 
		}
	}

}
