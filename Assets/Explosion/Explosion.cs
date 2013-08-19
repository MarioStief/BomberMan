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
<<<<<<< HEAD
	private int extra;
||||||| merged common ancestors
=======
	private int extra = 0;
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
	private bool createBomb;
	private bool self = false;
	private bool bombDestroyed = false;
<<<<<<< HEAD
	private bool superbomb;
	private bool triggerBomb = false;
	private bool contactMine = false;
||||||| merged common ancestors
	private bool triggerBomb;
=======
	private bool superbomb;
	private bool triggerBomb = false;
	private bool contactMine = false;
	private	bool contactMineActive = false;

>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
	
	private bool waitingForBombExplosion = true;
	
	private List<ExplosionField> explosionChain = new List<ExplosionField>();
	
	private float timer;
	private float createTime;
	private bool powerupsPlaced = false;
	
	
	// Factory-Klasse, um einen Konstruktor auf einem Monobehaviour-Objekt zu emulieren, der die Explosion auf einer Zelle startet
<<<<<<< HEAD
	public static Explosion createExplosionOnCell(Parcel cell, int flamePower, float delay, bool superbomb, int extra, bool createBomb, bool self) {
		Explosion thisObj = GUIObject.AddComponent<Explosion>();
		//calls Start() on the object and initializes it.
		thisObj.cell = cell;
		thisObj.flamePower = flamePower;
		thisObj.delay = delay;
		thisObj.superbomb = superbomb;
		thisObj.extra = extra;
		thisObj.createBomb = createBomb;
		thisObj.transform.position = cell.getCenterPos();
		thisObj.self = self;
		return thisObj;
	}

	// Factory-Klasse, um einen Konstruktor auf einem Monobehaviour-Objekt zu emulieren, der die Explosion auf einer Zelle startet
	public static Explosion createExplosionOnCell(Parcel cell, int flamePower, float delay, bool superbomb, int extra, bool createBomb) {
		Explosion thisObj = GUIObject.AddComponent<Explosion>();
		//calls Start() on the object and initializes it.
		thisObj.cell = cell;
		thisObj.flamePower = flamePower;
		thisObj.superbomb = superbomb;
		thisObj.extra = extra;
		thisObj.createBomb = createBomb;
		return thisObj;
||||||| merged common ancestors
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
=======
	[RPC]
	public Explosion createExplosionOnCell(int lpos, int bpos, int flamePower, float delay, bool superbomb, int extra, bool createBomb, bool self) {
		
		this.cell = Static.rink.gameArea[lpos][bpos];
		
		this.flamePower = flamePower;
		this.delay = delay;
		this.superbomb = superbomb;
		this.extra = extra;
		this.createBomb = createBomb;
		this.transform.position = cell.getCenterPos();
		this.self = self;
		
		cell.setExplosion(this);
		cell.setBomb(true);
		
		if (!self)
			startExplosion();
		
		return this;
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
	}

	
	GameObject bomb;
	void Start() {
		timer = 0.0f;
		createTime = Time.time;
		switch (extra) {
		case 1:
			triggerBomb = true;
			break;
		case 2:
			contactMine = true;
			break;
		}
		
		instantiatePSystems();

		if (createBomb) {
			if (contactMine) {
				bomb = GameObject.Instantiate(Static.contactMinePrefab, transform.position, Quaternion.identity) as GameObject;
				EXPLOSIONTIMER = 0.5f;
			} else {
				bomb = GameObject.Instantiate(Static.bombPrefab, transform.position, Quaternion.identity) as GameObject;
				EXPLOSIONTIMER = bomb.GetComponent<anim>().timer;
				bomb.GetComponent<anim>().triggerBomb = triggerBomb;
			}
			cell.setGameObject(bomb);
		}
<<<<<<< HEAD
		cell.setExplosion(this);
		if (extra == 2) {
			cell.setContactMine(true);
		} else {
			cell.setBomb(true);
		} 

||||||| merged common ancestors
		cell.setExplosion(this);
		cell.setBomb(true);

=======
		//cell.setExplosion(this);
		//cell.setBomb(true);
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
	}
	
	[RPC]
	public void startExplosion() {
		if (waitingForBombExplosion) {
			waitingForBombExplosion = false;
			createTime = Time.time;
			
			if (contactMine) {
				Static.inputHandler.playSound(Static.contactMineExplosionSoundEffect);
				createTime += 0.5f;
			}
		}
	}
	
	void Update() {
		
		if (this.cell == null)
			return;
		
		float elapsedTime = Time.time - createTime;
		if (waitingForBombExplosion) {
<<<<<<< HEAD
			if (true /* cj start immediately */ || (elapsedTime > EXPLOSIONTIMER && extra == 0)) {
||||||| merged common ancestors
			if (elapsedTime > EXPLOSIONTIMER && !triggerBomb) {
=======
			if (elapsedTime > EXPLOSIONTIMER && extra == 0) {
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
				waitingForBombExplosion = false;
				createTime = Time.time;
			} else if (elapsedTime > 3.0f && extra == 2 && !contactMineActive) {
				cell.setBomb(false);
				cell.setContactMine(true); // 3 s um in Deckung zu gehen ;)
				contactMineActive = true;
				//Debug.Log("Kontaktmine scharf");
			}
		} else {
			if (elapsedTime > 1.0f) { // ist eine Sekunde nichts passiert: GameObjekt zerstören
				Destroy (bomb);
				Destroy (gameObject);
			}

			if (elapsedTime > 0.3f) {					// nach 300 ms ohne Aktualisierung:
				foreach (ExplosionField explosionField in explosionChain) {
					explosionField.getCell().setExploding(false);
					//explosionField.getCell().colorCell(Color.green);
				}
			}

			// Explosionskette startet
			if (elapsedTime > delay) {

				if (!bombDestroyed) {
					// Zerstöre Bombe
					if (contactMine) {
						cell.setContactMine(false);
						Static.player.removeContactMine();
					}
					cell.setBomb(false);
					cell.setContactMine(false);
					if (createBomb)
						cell.destroyGameObject();
<<<<<<< HEAD
					if (self)
						Static.player.removeBomb();
||||||| merged common ancestors
					if (self)
						Player.removeBomb();
=======
					if (self && extra == 0) {
						Static.player.removeBomb();
					}
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
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
<<<<<<< HEAD
						explosionField.getCell().decreaseHeight();
                        if (superbomb) // superbomb
                        {
||||||| merged common ancestors
						explosionField.getCell().decreaseHeight();
						if (Player.getSuperbomb()) {
=======
						explosionField.getCell().decreaseHeight();
                        if (superbomb) // superbomb
                        {
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
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
						float distance = Vector3.Distance (GameObject.FindGameObjectWithTag("Player").transform.position, position);
						detonator.GetComponent<AudioSource>().volume /= 2*distance;
						detonator.GetComponent<AudioSource>().Play();
						//Debug.Log ("Explosion Volume: " + (100/(2*distance)) + " %");
						
						// Besonders hervorheben
<<<<<<< HEAD
						if (superbomb) {
							if (flamePower == Player.MAXFLAMEPOWER)
								detonator.color = Color.cyan;
							else
								detonator.color = Color.blue;
							detonator.addShockWave();
						} else {
							// normal bomb
							if (flamePower == Player.MAXFLAMEPOWER)
								detonator.color = Color.yellow;
						}

||||||| merged common ancestors
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
						
=======
						if (superbomb) {
							if (flamePower == Static.player.getMaxFlamePower())
								detonator.color = Color.cyan;
							else
								detonator.color = Color.blue;
							detonator.addShockWave();
						} else {
							// normal bomb
							if (flamePower == Static.player.getMaxFlamePower())
								detonator.color = Color.yellow;
						}
						
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
						detonator.Explode();
						explosionField.getCell().setExploding(true);
						//explosionField.getCell().colorCell(Color.black);

						
                        /*
						// Wand zerstören, ggfls. Powerup setzen
						if (PowerupPool.getDestroyable()) {
							if (explosionField.getCell().hasPowerup()) {
                         
								if (Preferences.getExplodingPowerups() == true) {
									float flameDelay = 0.2f;
									int flameReach = explosionField.getCell().getPowerupValue();
										flameDelay = 0.15f;
									bool superPowerup = false;
									if (flameReach == 10) {
										flameDelay = 0.1f;
<<<<<<< HEAD
									Explosion ex = Explosion.createExplosionOnCell(explosionField.getCell(), flameReach, flameDelay, false, false);
						
                                ex.startExplosion();
||||||| merged common ancestors
									Explosion ex = Explosion.createExplosionOnCell(explosionField.getCell(), flameReach, flameDelay, false, false);
									ex.startExplosion();
=======
										superPowerup = true;
									}
									GameObject ex = Network.Instantiate(Resources.Load("Prefabs/Bombe"), explosionField.getCell().getCenterPos(), Quaternion.identity, 0) as GameObject;
									ex.networkView.RPC("createExplosionOnCell", RPCMode.All, explosionField.getCell().getLpos(), explosionField.getCell().getBpos(), 
									                   flameReach, flameDelay, superPowerup, 0, false, false);
									//ex.startExplosion();
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
								}
								explosionField.getCell().destroyPowerup(true);
							}
						}
<<<<<<< HEAD
||||||| merged common ancestors
						
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
=======
						
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
							
							int random = Random.Range(0, (int) 100/DROPCHANCE);
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
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779

						explodingCell.getMeshManipulator().updateCoordinates();

						// Bomben jagen sich gegenseitig hoch:
						if (explodingCell.hasBomb() || explodingCell.hasContactMine()) {
							explodingCell.getExplosion().startExplosion();
							if (explodingCell.getExplosion().isTriggerBomb()) { // remove triggerBomb from list
								Dictionary<Parcel,GameObject> triggerBombs = new Dictionary<Parcel,GameObject>(Static.player.getTriggerBombs());
								foreach (var entry in triggerBombs) {
									if (explodingCell == entry.Key) {
										Static.player.removeTriggerBomb(entry.Key);
									}
								}
							}
						}
<<<<<<< HEAD
                        */

                        if (explosionField.getCell().getType() == 1 || (explosionField.getCell().getType() == 2 && explosionField.getCell().getHeight() == 1f))
                        {
                            explosionField.getCell().setType(0);
                            explodingCell.getMeshManipulator().updateCoordinates();
                        }   
||||||| merged common ancestors
=======

                        if (explosionField.getCell().getType() == 1 || (explosionField.getCell().getType() == 2 && explosionField.getCell().getHeight() == 1f))
                        {
                            explosionField.getCell().setType(0);
                            explodingCell.getMeshManipulator().updateCoordinates();
                        }   
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
						
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
					switch (cell.getType()) {
					case 0:
						//cell.colorCell(Color.red);
						break;
					case 1:
                        if (!superbomb)
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
							
	public bool isTriggerBomb() {
		return triggerBomb;
	}
}


