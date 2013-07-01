using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class Explosion : MonoBehaviour
{
	public float EXPLOSIONTIMER = 0f;
	private const int DROPCHANCE = PowerupPool.DROPCHANCE;
	public Parcel cell;
	private float xpos, ypos, zpos;
	
	GameObject []explosion = new GameObject[5];
	private int []reach = {0, 0, 0, 0, 0};
	private int flamePower;
	private float delay;
	private bool createBomb;
	private bool self = false;
	private bool bombDestroyed = false;
	private bool triggerBomb;
	
	private bool waitingForBombExplosion = true;
	
	private List<ExplosionField> explosionChain = new List<ExplosionField>();
	
	private float timer;
	private float createTime;
	private bool powerupsPlaced = false;
	
	private static GameObject guiObject;
	
	public static GameObject GUIObject {
		get {
			if (guiObject == null) {
				guiObject = new GameObject("Explosion");
			}
			return guiObject;
		}
	}
	
	// Factory-Klasse, um einen Konstruktor auf einem Monobehaviour-Objekt zu emulieren, der die Explosion auf einer Zelle startet
	public static Explosion createExplosionOnCell(Parcel cell, int flamePower, float delay, bool createBomb, bool self) {
		Explosion thisObj = GUIObject.AddComponent<Explosion>();
		//calls Start() on the object and initializes it.
		thisObj.cell = cell;
		thisObj.flamePower = flamePower;
		thisObj.delay = delay;
		thisObj.createBomb = createBomb;
		thisObj.transform.position = cell.getCenterPos();
		thisObj.self = self;
		return thisObj;
	}

	// Factory-Klasse, um einen Konstruktor auf einem Monobehaviour-Objekt zu emulieren, der die Explosion auf einer Zelle startet
	public static Explosion createExplosionOnCell(Parcel cell, int flamePower, float delay, bool createBomb) {
		Explosion thisObj = GUIObject.AddComponent<Explosion>();
		//calls Start() on the object and initializes it.
		thisObj.cell = cell;
		thisObj.flamePower = flamePower;
		thisObj.createBomb = createBomb;
		return thisObj;
	}

	void Start() {
		timer = 0.0f;
		createTime = Time.time;
		triggerBomb = Player.getTriggerbomb();
		
		instantiatePSystems();

		if (createBomb) {
			GameObject bomb = GameObject.Instantiate(Static.bombPrefab, transform.position, Quaternion.identity) as GameObject;
			EXPLOSIONTIMER = bomb.GetComponent<anim>().timer;
			bomb.GetComponent<anim>().triggerBomb = triggerBomb;
			cell.setGameObject(bomb);
		}
		cell.setExplosion(this);
		cell.setBomb(true);

	}
	
	public void startExplosion(){
		
		waitingForBombExplosion = false;
		createTime = Time.time;

		// ALTERNATIV: dropPowerup() hierher, und dann langsam einfaden
	}
	
	void Update() {
		float elapsedTime = Time.time - createTime;
		if (waitingForBombExplosion) {
			if (elapsedTime > EXPLOSIONTIMER && !triggerBomb) {
				waitingForBombExplosion = false;
				startExplosion();
			}
		} else {
			if (elapsedTime > 1.0f) {					// ist eine halbe Sekunde nichts passiert: GameObjekt zerstören
				Destroy (this);
			}

			if (elapsedTime > 0.3f) {					// nach 300 ms ohne Aktualisierung:
				foreach (ExplosionField explosionField in explosionChain) {
					explosionField.getCell().setExploding(false);
					//explosionField.getCell().colorCell(Color.green);
				}
			}

			// Explosionskette startet
			if (elapsedTime > delay) {					// alle 100 ms
				if (!bombDestroyed) {
					// Zerstöre Bombe
					cell.setBomb(false);
					if (createBomb)
						cell.destroyGameObject();
					if (self)
						Player.removeBomb();
					bombDestroyed = true;
				}
				foreach (ExplosionField explosionField in explosionChain) {
					bool stillRunning = false;
					if (explosionField.getDelay() == 0 && !explosionField.getCell().isExploding()) {
						Vector3 position = explosionField.getCell().getCenterPos();
						GameObject explosion = GameObject.Instantiate(Static.explosionPrefab, position, Quaternion.identity) as GameObject;
						explosion.transform.position = new Vector3(position.x + 0.05f, position.y + 0.05f, position.z + 0.05f);
						//explosion.GetComponent<Detonator>().size = 10f;
						Detonator detonator = explosion.GetComponent<Detonator>();
						explosionField.getCell().decreaseHeight();
						if (Player.getSuperbomb()) {
							explosionField.getCell().decreaseHeight();
							explosionField.getCell().decreaseHeight();
						}
						float explosionSize = 300f;
						detonator.setSize(explosionSize);
						
						if (explosionField.getCell().getType() == 2 && explosionField.getCell().getHeight() > 1.0f) // kleine Explosion in den Steinblöcken
							detonator.setSize(explosionSize*4); // in Wirklichkeit geviertelt
						
						detonator.setDuration(15f);
						Parcel explodingCell = explosionField.getCell();
						/*
						DetonatorComponent detonatorComponent = explosion.GetComponent<DetonatorComponent>();
						detonatorComponent.force = explodingCell.getSurroundingCell(explodingCell.getLpos(),explodingCell.getBpos()).getCenterPos();
						detonatorComponent.startForce = explodingCell.getSurroundingCell(explodingCell.getLpos(),explodingCell.getBpos()).getCenterPos();
						detonatorComponent.velocity = explodingCell.getSurroundingCell(explodingCell.getLpos(),explodingCell.getBpos()).getCenterPos();
						detonatorComponent.startVelocity = explodingCell.getSurroundingCell(explodingCell.getLpos(),explodingCell.getBpos()).getCenterPos();
						//detonatorComponent.startSize = 100f;
						*/
						
						// Explosionslautstärke der Spielerentfernung anpassen:
						float distance = Vector3.Distance (GameObject.Find("Player").transform.position, position);
						detonator.GetComponent<AudioSource>().volume /= 2*distance;
						detonator.GetComponent<AudioSource>().Play();
						//Debug.Log ("Explosion Volume: " + (100/(2*distance)) + " %");
						
						// Besonders hervorheben
						if (flamePower == Player.MAXFLAMEPOWER) {
							detonator.color = Color.yellow;
						}
						if (Player.getSuperbomb()) {
							detonator.color = Color.blue;
							detonator.addShockWave();
						}
						if (flamePower == Player.MAXFLAMEPOWER && Player.getSuperbomb()) {
							detonator.color = Color.cyan;
							detonator.addShockWave();
						}
						
						detonator.Explode();
						explosionField.getCell().setExploding(true);
						//explosionField.getCell().colorCell(Color.black);
						
						// Wand zerstören, ggfls. Powerup setzen
						if (PowerupPool.getDestroyable()) {
							if (explosionField.getCell().hasPowerup()) {
								if (Preferences.getExplodingPowerups() == true) {
									float flameDelay = 0.2f;
									int flameReach = explosionField.getCell().getPowerupValue();
										flameDelay = 0.15f;
									if (flameReach == 10)
										flameDelay = 0.1f;
									Explosion ex = Explosion.createExplosionOnCell(explosionField.getCell(), flameReach, flameDelay, false, false);
									ex.startExplosion();
								}
								explosionField.getCell().destroyPowerup(true);
							}
						}
						
						GameObject obj;
						switch (explodingCell.getType()) {
						case 0:
							// Inaktiv derzeit und wird wohl nicht implementiert
							// explodingCell.decreaseFloor();
							break;
						case 1:
							explodingCell.setType(0);
							
							// Kiste explodieren lassen
							obj = GameObject.Instantiate(explodingCell.getMeshManipulator().getBoxObject(), explodingCell.getCenterPos(), Quaternion.identity) as GameObject;
							SplitMeshIntoTriangles.createMeshExplosion(obj, cell.getCenterPos(), 1);
							
							int random = new System.Random().Next(0, (int) 100/DROPCHANCE);
							//Debug.Log("Placing Powerup for cell " + explodingCell.getCoordinates() + ": " + (random == 0 ? "yes" : "no"));
							if (random == 0) { // Random().Next(0, 4) € {0, 1, 2, 3}
								PowerupPool.setPowerup(explodingCell);
							}
							break;
						case 2:
							// Steinblock explodieren lassen
							obj = GameObject.Instantiate(Static.stoneCube2Prefab, explodingCell.getCenterPos(), Quaternion.identity) as GameObject;
							SplitMeshIntoTriangles.createMeshExplosion(obj, cell.getCenterPos(), 1);
							
							if (explodingCell.getHeight() == 1f) {
								explodingCell.setType(0);
								PowerupPool.setPowerup(explodingCell);
							}
							break;
						}

						explodingCell.getMeshManipulator().updateCoordinates();

						// Bomben jagen sich gegenseitig hoch:
						if (explosionField.getCell().hasBomb()) {
							explosionField.getCell().getExplosion().startExplosion();
						}
						
						stillRunning = true;

						
					} else if ((explosionField.getDelay() * delay) < -0.3f) { // Zellen sind wieder betretbar nach 300 ms
						explosionField.getCell().setExploding(false);
						//explosionField.getCell().colorCell(Color.green);
					}

					explosionField.decrement(); // Zähle Delay-Ticker runter
					if (stillRunning)
						createTime = Time.time;
				}
			}
		}
	}
	
	private void instantiatePSystems(){
		
		if (true) {
			//cell.colorCell(Color.red);
			explosionChain.Add(new ExplosionField(0, cell, 0, 0));
		}

		int[] stop = {0, 0, 0, 0};
		
		bool SURROUNDING_DEBUG = false;

		if (SURROUNDING_DEBUG)
			Debug.Log("I am here: " + this.cell.getCoordinates());
			
		for (int i = 1; i <= flamePower; i++) {
			for (int j = 0; j < 4; j++) {
				if (stop[j] == 0) {
					int lpos = 0;
					int bpos = 0;
					switch (j) {
					case 0:
						lpos = -i;
						break;
					case 1:
						lpos = i;
						break;
					case 2:
						bpos = -i;
						break;
					case 3:
						bpos = i;
						break;
					}
					Parcel cell = this.cell.getSurroundingCell(lpos, bpos);
					if (SURROUNDING_DEBUG)
						Debug.Log("Surrounding Cell: " + cell.getCoordinates() + ", Height: " + cell.getHeight());
					switch (cell.getType()) {
					case 0:
						//cell.colorCell(Color.red);
						break;
					case 1:
						if (!Player.getSuperbomb())
							stop[j] = 1;
						//cell.colorCell(Color.red);
						break;
					case 2:
						stop[j] = 2;
						//cell.colorCell(Color.gray);
						break;
					}
					explosionChain.Add(new ExplosionField(i,cell, lpos, bpos));
				}
				
			}
		}
		//Debug.Log ("#ExplosionFields: " + explosionChain.Count);
	}
}


