using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class Explosion : MonoBehaviour
{
	// private const float EXPLOSIONTIMER = 0.1f; // Debugwert
	private const float EXPLOSIONTIMER = 3.0f;
	private const int DROPCHANCE = PowerupPool.DROPCHANCE;
	public Parcel cell;
	private float xpos, ypos, zpos;
	
	GameObject []explosion = new GameObject[5];
	private int []reach = {0, 0, 0, 0, 0};
	private int flamePower;
	private float delay;
	private bool createBomb;
	private bool self = false;
	
	private int []dists;
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
		// NEWWWWWWWWWWWWWWWWWWWWWWWWWWWWW
		//cell = sphereHandler.getGameArea().getCurrentParcel(0, 0);
		//xpos = GameObject.Find("Player").GetComponent<InputHandler>().getXPos();
		//ypos = GameObject.Find("Player").GetComponent<InputHandler>().getYPos();
		//zpos = GameObject.Find("Player").GetComponent<InputHandler>().getZPos();
		
		//bomb = GameObject.Instantiate(bombPrefab, new Vector3(xpos + 0.5f, 0.3f, zpos + 0.5f), Quaternion.identity) as GameObject; 
		//bomb = GameObject.Instantiate(bombPrefab, new Vector3(xpos, ypos, zpos), Quaternion.identity) as GameObject; 
		timer = 0.0f;
		createTime = Time.time;
		
		dists = new int[4];
		instantiatePSystems();

		//explosions.Add(this);
		//transform.position = cell.getCenterPos();
		if (createBomb)
			cell.setGameObject(GameObject.Instantiate(GameObject.Find("bomb"), transform.position, Quaternion.identity) as GameObject);
		cell.setExplosion(this);
		cell.setBomb(true);

	}
	
	public void startExplosion(){
		
		cell.setBomb(false);
		if (createBomb)
			cell.destroyGameObject();
		if (self)
			Static.player.removeBomb();
		//bomb = null;
		
		Debug.Log ("Flammenstaerke: " + reach[1] + ", " + reach[2] + ", " + reach[3] + ", " + reach[4]);
		
		/*
		for (int i = 1; i <= 4; i++) { //  CHANGE TO i = 1; i <= 4
			explosion[i].GetComponent<ParticleEmitter>().minSize = 0.0f;
			explosion[i].GetComponent<ParticleEmitter>().maxSize = 2.5f;
			explosion[i].GetComponent<ParticleEmitter>().minEnergy = 0.2f;
			explosion[i].GetComponent<ParticleEmitter>().maxEnergy = 0.2f * reach[i];
			explosion[i].GetComponent<ParticleEmitter>().minEmission = 2000;
			explosion[i].GetComponent<ParticleEmitter>().maxEmission = 2000;
		}
		*/
		/*
		foreach (ExplosionField explosionField in explosionChain) {
			explosionField.decrement(); // Zähle Delay-Ticker runter
		}
		*/
		
		waitingForBombExplosion = false;
		createTime = Time.time;

		// ALTERNATIV: dropPowerup() hierher, und dann langsam einfaden
	}
	
	void Update() {
		float elapsedTime = Time.time - createTime;
		if (waitingForBombExplosion) {
			if (true /* cj start immediately */ || elapsedTime > EXPLOSIONTIMER) {
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
					explosionField.getCell().colorCell(Color.green);
				}
			}

			// Explosionskette startet
			if (elapsedTime > delay) {					// alle 100 ms
				foreach (ExplosionField explosionField in explosionChain) {
					bool stillRunning = false;
					if (explosionField.getDelay() == 0 && !explosionField.getCell().isExploding()) {
						Vector3 position = explosionField.getCell().getCenterPos();
						GameObject explosion = GameObject.Instantiate(Static.explosionPrefab, position, Quaternion.identity) as GameObject;
						explosion.transform.position = new Vector3(position.x + 0.05f, position.y + 0.05f, position.z + 0.05f);
						//explosion.GetComponent<Detonator>().size = 10f;
						Detonator detonator = explosion.GetComponent<Detonator>();
						explosionField.getCell().decreaseHeight();
                        if (Static.player.getSuperbomb())
                        {
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
						
						detonator.Explode();
						explosionField.getCell().setExploding(true);
						explosionField.getCell().colorCell(Color.black);
						
						// Wand zerstören, ggfls. Powerup setzen
						if (PowerupPool.getDestroyable()) {
							if (explosionField.getCell().hasPowerup()) {
								if (Preferences.getExplodingPowerups() == true) {
                                    Explosion ex = Explosion.createExplosionOnCell(explosionField.getCell(), explosionField.getCell().getPowerupValue(), Static.player.getDelay(), false, false);
									ex.startExplosion();
								}
								explosionField.getCell().destroyPowerup(true);
							}
						}

						if (explodingCell.getType() == 1) {
							explodingCell.setType(0);
							int random = new System.Random().Next(0, (int) 100/DROPCHANCE);
							Debug.Log("Placing Powerup for cell " + explodingCell.getCoordinates() + ": " + (random == 0 ? "yes" : "no"));
							if (random == 0) { // Random().Next(0, 4) € {0, 1, 2, 3}
								PowerupPool.setPowerup(explodingCell);
							}
						}
						if (explodingCell.getType() == 2 && explodingCell.getHeight() == 1f) {
							explodingCell.setType(0);
							PowerupPool.setPowerup(explodingCell);
						}

						explodingCell.getMeshManipulator().updateCoordinates();

						// Bomben jagen sich gegenseitig hoch:
						if (explosionField.getCell().hasBomb()) {
							explosionField.getCell().getExplosion().startExplosion();
						}
						
						stillRunning = true;

						
					} else if ((explosionField.getDelay() * delay) < -0.3f) { // Zellen sind wieder betretbar nach 300 ms
						explosionField.getCell().setExploding(false);
						explosionField.getCell().colorCell(Color.green);
					}

					explosionField.decrement(); // Zähle Delay-Ticker runter
					if (stillRunning)
						createTime = Time.time;
				}
			}
		}
	}
	/*
	private void placePowerup() {
		if (!powerupsPlaced) {
			foreach (ExplosionField explosionField in explosionChain) {
				Parcel cell = explosionField.getCell();
				Debug.Log("Cell " + cell.getCoordinates() + " has GameObject: " + (cell.hasGameObject() ? "yes" : "no"));
				if (!cell.hasGameObject()) {
					int random = new System.Random().Next(0, (int) 100/DROPCHANCE);
					Debug.Log("Placing Powerup for cell " + cell.getCoordinates() + ": " + (random == 0 ? "yes" : "no"));
					if (random == 0) { // Random().Next(0, 4) € {0, 1, 2, 3}
						PowerupPool.setPowerup(cell);
					}
				}
			}
		}
		powerupsPlaced = true;
	}
	*/
	
	private void instantiatePSystems(){
		
		if (true) {
			cell.colorCell(Color.red);
			explosionChain.Add(new ExplosionField(0, cell, 0, 0, 0));
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
						cell.colorCell(Color.red);
						break;
					case 1:
                        if (!Static.player.getSuperbomb())
							stop[j] = 1;
						cell.colorCell(Color.red);
						break;
					case 2:
						stop[j] = 2;
						cell.colorCell(Color.gray);
						break;
					}
					explosionChain.Add(new ExplosionField(i,cell, stop[j], lpos, bpos));
				}
				
			}
		}
		Debug.Log ("#ExplosionFields: " + explosionChain.Count);
	}
	
	/*
	private int[] getDistances(){
		
		int range = Player.getFlamePower();
		
		// Right
		int z = zpos+1;
		while(z < Data.height && Data.area.getCell(xpos, z).getType() != 2  && z-zpos-1 < range){
			if (Data.area.getCell(xpos, z).getType() == 1){
				z++;
				break;
			}
			z++;
		}
		dists[3] = z - zpos-1;
		
		// Left
		z = zpos-1;
		while( z >= 0 && Data.area.getCell(xpos, z).getType() != 2 &&  zpos-1-z < range){
			if (Data.area.getCell(xpos, z).getType() == 1){
				z--;
				break;
			}
			z--;
		}
		dists[2] = zpos - 1 - z;
		//Debug.Log("Right: " + dists[3] + ", LEFT: " + dists[2]);
		// Down
		int x = xpos+1;
		while(x < Data.width && Data.area.getCell(x, zpos).getType() != 2 &&  x - xpos -1 < range){
			if (Data.area.getCell(x, zpos).getType() == 1){
				x++;
				break;
			}
			x++;
		}
		dists[1] = x - xpos - 1;
		
		// Up
		x = xpos-1;
		while(x >= 0 && Data.area.getCell(x, zpos).getType() != 2 && xpos - 1 - x < range){
			if (Data.area.getCell(x, zpos).getType() == 1){
				x--;
				break;
			}
			x--;
		}
		dists[0] = xpos -1 - x;
		
		return dists;
	}
	*/
}


