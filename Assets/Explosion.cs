using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class Explosion : MonoBehaviour
{
	private const int DELAY = 100;
	private const float EXPLOSIONTIMER = 3.0f;
	private const int DROPCHANCE = 25; // Drop chance in %
	float SCALE = 0.25f;
	public GameObject sphere; // DELETE?
	private SphereBuilder sphereHandler;
	public Parcel cell;
	private float xpos, ypos, zpos;
	
	public static GameObject bombPrefab;
	public static GameObject explotionPrefab;
	//private GameObject bomb;
	GameObject []explosion = new GameObject[5];
	private int []reach = {0, 0, 0, 0, 0};
	
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
	public static Explosion createExplosionOnCell(Parcel cell) {
		Explosion thisObj = GUIObject.AddComponent<Explosion>();
		//calls Start() on the object and initializes it.
		thisObj.cell = cell;
		return thisObj;
	}
	
	void Awake() {
		bombPrefab = GameObject.Find("bomb");
		explotionPrefab = GameObject.Find("Explotion");
		sphere = GameObject.Find("Sphere");
		explotionPrefab.transform.localScale *= SCALE;
		sphereHandler = sphere.GetComponent<SphereBuilder>();
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
		Vector3 position = GameObject.Find("Sphere").GetComponent<SphereBuilder>().getGameArea().drawnArea[cell.getLpos()][cell.getBpos()].getCenter();
		cell.setGameObject(GameObject.Instantiate(bombPrefab, position, Quaternion.identity) as GameObject);
		cell.setBomb(true);

	}
	
	public void startExplosion(){
		
		cell.setBomb(false);
		//GameObject.Destroy(bomb);
		cell.destroyGameObject();
		Player.removeBomb();
		//bomb = null;
		sphereHandler.getGameArea().clearBlue();
		
		Debug.Log ("Flammenstaerke: " + reach[1] + ", " + reach[2] + ", " + reach[3] + ", " + reach[4]);
		
		for (int i = 1; i <= 4; i++) {
			explosion[i].GetComponent<ParticleEmitter>().minSize = 0.0f;
			explosion[i].GetComponent<ParticleEmitter>().maxSize = 2.5f;
			explosion[i].GetComponent<ParticleEmitter>().minEnergy = 0.2f;
			explosion[i].GetComponent<ParticleEmitter>().maxEnergy = 0.2f * reach[i];
			explosion[i].GetComponent<ParticleEmitter>().minEmission = 2000;
			explosion[i].GetComponent<ParticleEmitter>().maxEmission = 2000;
		}
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
			if (elapsedTime > EXPLOSIONTIMER) {
				waitingForBombExplosion = false;
				startExplosion();
			}
		} else {
			if (elapsedTime > 1.0f) {					// ist eine halbe Sekunde nichts passiert: GameObjekt zerstören
				Destroy (this);
			}

			if (elapsedTime > 0.3f) {					// nach 300 ms ohne Aktualisierung:
				placePowerup();							// Lasse Powerup erscheinen
				for (int i = 1; i <= 4; i++) {			// keine neuen Partikel mehr
					if (explosion[i] != null) {
						explosion[i].GetComponent<ParticleEmitter>().maxEmission = 0;
					}
				}
			}

			
			if (elapsedTime > 0.1f) {					// alle 100 ms
				foreach (ExplosionField explosionField in explosionChain) {
					bool stillRunning = false;
					if (explosionField.getDelay() == 0) {
						//explosionField.getCell().setExploding(true);
						if (PowerupPool.getDestroyable())
							if (explosionField.getCell().hasPowerup())
								explosionField.getCell().destroyPowerup();
						stillRunning = true;
					} /*else if (explosionField.getDelay() == -5) {
						explosionField.getExplosion().GetComponent<ParticleEmitter>().maxEmission = 0;
					}*/

					explosionField.decrement(); // Zähle Delay-Ticker runter
					if (stillRunning)
						createTime = Time.time;
				}
			}
		}
	}
	
	private void placePowerup() {
		if (!powerupsPlaced) {
			Debug.Log ("#Still in ExplosionFields: " + explosionChain.Count);
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
	
	private void instantiatePSystems(){
		
		for (int i = 0; i < 5; i++) {
			//explosion[i] = GameObject.Instantiate(explotionPrefab, new Vector3( xpos + 0.5f, 0.5f, zpos + 0.5f), Quaternion.identity) as GameObject;
			explosion[i] = GameObject.Instantiate(explotionPrefab, new Vector3(xpos, ypos, zpos), Quaternion.identity) as GameObject;
			explosion[i].GetComponent<ParticleEmitter>().maxEmission = 0;
			if (i == 0)
				explosionChain.Add(new ExplosionField(0,cell));
		}
		
		explosion[1].GetComponent<ParticleEmitter>().worldVelocity = new Vector3(-5.0f, 0.3f, 0.0f);
		explosion[2].GetComponent<ParticleEmitter>().worldVelocity = new Vector3(5.0f, 0.3f, 0.0f);
		explosion[3].GetComponent<ParticleEmitter>().worldVelocity = new Vector3(0.0f, 0.3f, -5.0f);
		explosion[4].GetComponent<ParticleEmitter>().worldVelocity = new Vector3(0.0f, 0.3f, 5.0f);

		//getDistances();
		// DEBUG START
		for (int i = 0; i < dists.Length; i++) {
			dists[i] = 10;
		}
		// DEBUG END
		
		Debug.Log("Dists: [" + dists[0] + "," + dists[1] + "," + dists[2] + "," + dists[3] + "]");
		
		
			
		for (int i = 1; i <= Player.getFlamePower(); i++) {
			if (i <= dists[0]) {
				int x = (int) transform.position.x;
				int z = (int) transform.position.z;
				Parcel cell = this.cell.getSurroundingCell(-i, 0);
				if (cell.getHeight() < 1.1f) {
					cell.blueColor();
					explosionChain.Add(new ExplosionField(i,cell));
					reach[1]++;
				}
			}

			if (i <= dists[1]) {
				int x = (int) transform.position.x;
				int z = (int) transform.position.z;
				Parcel cell = this.cell.getSurroundingCell(i, 0);
				if (cell.getHeight() < 1.1f) {
					cell.blueColor();
					explosionChain.Add(new ExplosionField(i,cell));
					reach[2]++;
				}
			}

			if (i <= dists[2]) {
				int x = (int) transform.position.x;
				int z = (int) transform.position.z;
				Parcel cell = this.cell.getSurroundingCell(0, -i);
				if (cell.getHeight() < 1.1f) {
					cell.blueColor();
					explosionChain.Add(new ExplosionField(i,cell));
					reach[3]++;
				}
			}

			if (i <= dists[3]) {
				int x = (int) transform.position.x;
				int z = (int) transform.position.z;
				Parcel cell = this.cell.getSurroundingCell(0, i);
				if (cell.getHeight() < 1.1f) {
					cell.blueColor();
					explosionChain.Add(new ExplosionField(i,cell));
					reach[4]++;
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


