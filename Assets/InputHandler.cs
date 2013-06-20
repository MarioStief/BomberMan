using UnityEngine;
using System.Collections.Generic;
using AssemblyCSharp;

// <summary>
// InputHandler nimmt jeglichen relevanten Input entgegen und verarbeitet diesen
// bzw. leiten ihn an verarbeitende Klassen weiter.
// </summary>

public class InputHandler : MonoBehaviour {
	
	public GameObject sphere;			
	private SphereBuilder sphereHandler;
	
	public GameObject camera;
	private Quaternion cameraRotation;
	
	// Cell currCell = ?
	// Henning, ich brauche die Zellen wieder! xD
	
	// Use this for initialization
	void Start () {
	
		sphereHandler = sphere.GetComponent<SphereBuilder>();
		cameraRotation = camera.transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
	
		if (!Player.isDead()) {
			
			// Lese Bewegungsrichtung aus und lasse die Kugel entsprechend bewegen.
			float verticalMovement = Input.GetAxis("Vertical") * Player.getSpeed();
			sphereHandler.move(verticalMovement);
			
			float horizontalMovement = Input.GetAxis("Horizontal") * Player.getSpeed();
			if ( horizontalMovement != 0)
				moveAlongEquator( horizontalMovement/(-2)*Time.deltaTime);
			
			// Falls die Zelle ein Powerup enthält -> aufsammeln
			/*
			if (currCell.hasPowerup()) {
				Player.powerupCollected(currCell.destroyPowerup());
			}
			*/
			
			// Leertaste -> Bombe legen
			if ( Input.GetKeyDown(KeyCode.Space)){
				if (Player.addBomb()) {
					GameObject explosion = new GameObject("explosion");
					explosion.AddComponent<Explosion>();
				}
			}
			/*
			if ( Input.GetKeyDown(KeyCode.Space)){
				if ( !currCell.hasBomb()) {
					if (Player.addBomb()) {
						GameObject explosion = new GameObject("explosion");
						explosion.AddComponent<Explosion>();
					}
				}
			}
			if ((Time.time - createTime) > 1.0f) {
				createTime = Time.time;
				Player.increaseHP();
			}
			*/
		}
	}
	
	private void moveAlongEquator(float movement){
	
		transform.position = new Vector3(Mathf.Cos(movement)* transform.position.x - Mathf.Sin(movement) * transform.position.y,
										Mathf.Sin(movement) * transform.position.x + Mathf.Cos(movement) * transform.position.y,
										transform.position.z);
		
		camera.transform.position = new Vector3(Mathf.Cos(movement)* camera.transform.position.x - Mathf.Sin(movement) * camera.transform.position.y,
										Mathf.Sin(movement) * camera.transform.position.x + Mathf.Cos(movement) * camera.transform.position.y,
										camera.transform.position.z);
		
		camera.transform.rotation.SetLookRotation(Vector3.zero);
	}
	
	void OnParticleCollision(GameObject explosion) {
		Player.decreaseHP();
		if (Player.getHP() == 0) {
			renderer.material.color = Color.black;
			//moveDirection = new Vector3(0, 0, 0);
			//GameObject deadPlayer = GameObject.Instantiate(Data.deadPlayerPrefab, new Vector3(currCell.getXPos() + 0.5f, 0.3f, currCell.getZPos() + 0.5f), Quaternion.identity) as GameObject; 
		}
	}
}
