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
	
	private int flamePower;
	private float delay;
	private int extra = 0;
	private bool createBomb;
	private bool self = false;
	private bool bombDestroyed = false;
	private bool superbomb;
	private bool triggerBomb = false;
	private bool contactMine = false;
	private	bool contactMineActive = false;

	
	private bool waitingForBombExplosion = true;
	
	private List<ExplosionField> explosionChain = new List<ExplosionField>();
	
	private float createTime;

	
	
	// Factory-Klasse, um einen Konstruktor auf einem Monobehaviour-Objekt zu emulieren, der die Explosion auf einer Zelle startet
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
		if (extra != 2)
			cell.setBomb(true);
		
		if (!self)
			startExplosion(false);
		
		return this;
	}

	
	GameObject bomb;
	void Start() {
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
				if (triggerBomb) {
					bomb = GameObject.Instantiate(Static.triggerbombPrefab, transform.position, Quaternion.identity) as GameObject;
				} else {
					bomb = GameObject.Instantiate(Static.bombPrefab, transform.position, Quaternion.identity) as GameObject;
					EXPLOSIONTIMER = bomb.GetComponent<anim>().timer;
				}
			}
			cell.setGameObject(bomb);
		}
		Random.seed = Menu.rSeed;
		//cell.setExplosion(this);
		//cell.setBomb(true);
	}
	
	[RPC]
	public void startExplosion(bool instantly) {
		if (waitingForBombExplosion) {
			waitingForBombExplosion = false;
			createTime = Time.time;
			
			if (contactMine) {
				if (instantly) {
					EXPLOSIONTIMER = 0f;
					waitingForBombExplosion = false;
				}
				Static.menuHandler.playSound(Static.contactMineExplosionSoundEffect, false);
				createTime += EXPLOSIONTIMER;
			}
		}
	}
	
	void Update() {
		
		if (this.cell == null)
			return;
		
		if (bomb != null)
			bomb.transform.up = transform.position;
		
		float elapsedTime = Time.time - createTime;
		if (waitingForBombExplosion) {
			if (elapsedTime > EXPLOSIONTIMER && extra == 0) {
				waitingForBombExplosion = false;
				createTime = Time.time;
			} else if (elapsedTime > 3.0f && extra == 2 && !contactMineActive) {
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
						if (networkView.isMine)
							Static.player.removeContactMine();
					}
					cell.setBomb(false);
					if (createBomb)
						cell.destroyGameObject();
					if (self && extra == 0 && networkView.isMine) {
						Static.player.removeBomb();
					}
					bombDestroyed = true;
				}
				foreach (ExplosionField explosionField in explosionChain) {
					bool stillRunning = false;
					Parcel explodingCell = explosionField.getCell();
					if (explosionField.getDelay() == 0 && !explodingCell.isExploding()) {
						Vector3 position = explodingCell.getCenterPos();
						GameObject explosion = GameObject.Instantiate(Static.explosionPrefab, position, Quaternion.identity) as GameObject;
						explosion.transform.position = new Vector3(position.x + 0.05f, position.y + 0.05f, position.z + 0.05f);
						//explosion.GetComponent<Detonator>().size = 10f;
						Detonator detonator = explosion.GetComponent<Detonator>();
						explodingCell.decreaseHeight();
                        if (superbomb) // superbomb
                        {
							explodingCell.decreaseHeight();
							explodingCell.decreaseHeight();
						}
						float explosionSize = 300f;
						detonator.setSize(explosionSize);
						
						if (explodingCell.getType() == 2 && explodingCell.getHeight() > 1.0f) // kleine Explosion in den Steinblöcken
							detonator.setSize(explosionSize*4); // in Wirklichkeit geviertelt
						
						detonator.setDuration(15f);
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
						AudioSource audioSource = detonator.GetComponent<AudioSource>();
						if (superbomb) {
							audioSource.clip = Static.superExplosionSoundEffect;
							audioSource.volume /= 3*distance;
						} else {
							audioSource.volume /= 6*distance;
						}
						audioSource.Play();
						//Debug.Log ("Explosion Volume: " + (100/(2*distance)) + " %");
						//Debug.Log ("Distanz: " + distance + ", Explosion Volume: " + audioSource.volume);
						
						// Besonders hervorheben
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
						
						detonator.Explode();
						explodingCell.setExploding(true);
						//explodingCell.colorCell(Color.black);
						
						// Powerup hochjagen
						if (PowerupPool.getDestroyable()) {
							if (explodingCell.hasPowerup() && explodingCell.getGameObject().GetComponent<Active>().isActive()) {
								if (Preferences.getExplodingPowerups()) {
									float flameDelay;
									int flameReach = explodingCell.getPowerupValue();
									bool superPowerup = false;
									switch (flameReach) {
									case 10:
										flameDelay = 0.1f;
										superPowerup = true;
										break;
									case 5:
										flameDelay = 0.15f;
										break;
									default:
										flameDelay = 0.2f;
										break;
									}
									GameObject ex = Network.Instantiate(Resources.Load("Prefabs/Bombe"), explodingCell.getCenterPos(), Quaternion.identity, 0) as GameObject;
									ex.networkView.RPC("createExplosionOnCell", RPCMode.All, explodingCell.getLpos(), explodingCell.getBpos(), 
									                   flameReach, flameDelay, superPowerup, 0, false, false);
									//ex.startExplosion(false);
								}
								explodingCell.destroyPowerup(false, true);
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
							
							int random = Random.Range(0, (int) 100/DROPCHANCE);
							//Debug.Log("Placing Powerup for cell " + explodingCell.getCoordinates() + ": " + (random == 0 ? "yes" : "no"));
							if (random == 0) { // Random().Next(0, 4) € {0, 1, 2, 3}
								PowerupPool.setPowerup(explodingCell);
							}
							break;
						case 2:
							// Steinblock explodieren lassen
							//obj = GameObject.Instantiate(Static.stoneCube2Prefab, explodingCell.getCenterPos(), Quaternion.identity) as GameObject;
							//SplitMeshIntoTriangles.createMeshExplosion(obj, cell.getCenterPos(), 1);
							
							if (explodingCell.getHeight() == 1f) {
								explodingCell.setType(0);
								PowerupPool.setPowerup(explodingCell);
							}
							break;
						}

						explodingCell.getMeshManipulator().updateCoordinates();

						// Bomben jagen sich gegenseitig hoch:
						if (explodingCell.hasExplosion()) {
							explodingCell.getExplosion().startExplosion(true);
							if (explodingCell.getExplosion().isTriggerBomb()) { // remove triggerBomb from list
								Dictionary<Parcel,GameObject> triggerBombs = new Dictionary<Parcel,GameObject>(Static.player.getTriggerBombs());
								foreach (var entry in triggerBombs) {
									if (explodingCell == entry.Key) {
										Static.player.removeTriggerBomb(entry.Key);
									}
								}
							}
						}

                        if (explodingCell.getType() == 1 || (explodingCell.getType() == 2 && explodingCell.getHeight() == 1f))
                        {
                            explodingCell.setType(0);
                            explodingCell.getMeshManipulator().updateCoordinates();
                        }   
						
						stillRunning = true;

						
					} else if ((explosionField.getDelay() * delay) < -0.3f) { // Zellen sind wieder betretbar nach 300 ms
						explodingCell.setExploding(false);
						//explodingCell.colorCell(Color.green);
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


