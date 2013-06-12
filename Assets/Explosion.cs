using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class Explosion : MonoBehaviour
{
	private const int DELAY = 100;
	private const float EXPLOSIONTIMER = 3.0f;
	private Cell currCell;
	private int xpos, zpos;
	
	private GameObject bomb;
	GameObject []explosion = new GameObject[5];
	private int []reach = {0, 0, 0, 0, 0};
	
	private int []dists;
	private bool waitingForBombExplosion = true;
	private bool itemDrop = false;

	private List<ExplosionField> explosionChain = new List<ExplosionField>();
	
	private float timer;
	private float createTime;
	
	void Start() {
		currCell = Data.area.getCell(Data.controller.transform.position.x, Data.controller.transform.position.z);
		xpos = currCell.getXPos();
		zpos = currCell.getZPos();
		
		bomb = GameObject.Instantiate(Data.bombPrefab, new Vector3(xpos + 0.5f, 0.3f, zpos + 0.5f), Quaternion.identity) as GameObject; 
		timer = 0.0f;
		createTime = Time.time;
		
		dists = new int[4];
		instantiatePSystems();

		Data.explosions.Add(this);
		Data.area.getCell(xpos,zpos).setBomb(true, this);
	}
	
	public void startExplosion(){
		
		Data.area.getCell(xpos,zpos).setBomb(false, this);
		GameObject.Destroy(bomb);
		bomb = null;
		
		Debug.Log ("Flammenstaerke: " + reach[1] + ", " + reach[2] + ", " + reach[3] + ", " + reach[4]);
		
		for (int i = 1; i <= 4; i++) {
			explosion[i].GetComponent<ParticleEmitter>().minSize = 0.0f;
			explosion[i].GetComponent<ParticleEmitter>().maxSize = 1.5f;
			explosion[i].GetComponent<ParticleEmitter>().minEnergy = 0.2f;
			explosion[i].GetComponent<ParticleEmitter>().maxEnergy = 0.2f * reach[i];
			explosion[i].GetComponent<ParticleEmitter>().minEmission = 10000;
			explosion[i].GetComponent<ParticleEmitter>().maxEmission = 10000;
		}
		foreach (ExplosionField explosionField in explosionChain) {
			explosionField.decrement(); // Zähle Delay-Ticker runter
		}
		
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
			if (elapsedTime > 0.5f) // ist eine halbe Sekunde nichts passiert: GameObjekt zerstören
				Destroy (this);

			if (elapsedTime > 0.3f) { // nach 300 ms ohne Aktualisierung: keine neuen Partikel mehr
				dropPowerup();

				foreach (ExplosionField explosionField in explosionChain) {
					for (int i = 1; i <= 4; i++) {
						explosion[i].GetComponent<ParticleEmitter>().maxEmission = 0;
					}
					explosionField.getCell().setExploding(false);
				}
			}

			
			if (elapsedTime > 0.1f) { // alle 100 ms
				foreach (ExplosionField explosionField in explosionChain) {
					bool stillRunning = false;
					if (explosionField.getDelay() == 0) {
						explosionField.getCell().setExploding(true);
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
	
	private void dropPowerup() {
		if (!itemDrop) {
			if (!currCell.hasPowerup()) {
				Player.destroyBomb();
				if (new System.Random().Next(0, 1) == 0) // DEBUG: Bombe nach Explosion erzeugen mit 25 %
					PowerupPool.setPowerup(xpos, zpos);
			}
			itemDrop = true;
		}
	}
	
	private void instantiatePSystems(){
		
		for (int i = 0; i < 5; i++) {
			explosion[i] = GameObject.Instantiate(Data.explotionPrefab, new Vector3( xpos + 0.5f, 0.5f, zpos + 0.5f), Quaternion.identity) as GameObject;
			explosion[i].GetComponent<ParticleEmitter>().maxEmission = 0;
			explosionChain.Add(new ExplosionField(0,Data.area.getCell(xpos, zpos)));
		}
		
		explosion[1].GetComponent<ParticleEmitter>().worldVelocity = new Vector3(-5.0f, 0.3f, 0.0f);
		explosion[2].GetComponent<ParticleEmitter>().worldVelocity = new Vector3(5.0f, 0.3f, 0.0f);
		explosion[3].GetComponent<ParticleEmitter>().worldVelocity = new Vector3(0.0f, 0.3f, -5.0f);
		explosion[4].GetComponent<ParticleEmitter>().worldVelocity = new Vector3(0.0f, 0.3f, 5.0f);

		getDistances();
		Debug.Log(dists[0] + "," + dists[1] + "," + dists[2] + "," + dists[3]);
		
		for (int i = 1; i <= Player.getFlamePower(); i++) {
			if (i <= dists[0]) {
				//explosion = GameObject.Instantiate(Data.explotionUpPrefab, new Vector3( (xpos-i) + 0.5f, 0.5f, zpos + 0.5f), Quaternion.identity) as GameObject;
				//explosion.GetComponent<ParticleEmitter>().maxEmission = 0;
				reach[1]++;
				explosionChain.Add(new ExplosionField(i,Data.area.getCell((xpos-i), zpos)));
			}

			if (i <= dists[1]) {
				//explosion = GameObject.Instantiate(Data.explotionDownPrefab, new Vector3( (xpos+i) + 0.5f, 0.5f, zpos + 0.5f), Quaternion.identity) as GameObject;
				//explosion.GetComponent<ParticleEmitter>().maxEmission = 0;
				reach[2]++;
				explosionChain.Add(new ExplosionField(i,Data.area.getCell((xpos+i), zpos)));
			}

			if (i <= dists[2]) {
				//explosion = GameObject.Instantiate(Data.explotionLeftPrefab, new Vector3( xpos + 0.5f, 0.5f, (zpos-i) + 0.5f), Quaternion.identity) as GameObject;
				//explosion.GetComponent<ParticleEmitter>().maxEmission = 0;
				reach[3]++;
				explosionChain.Add(new ExplosionField(i,Data.area.getCell(xpos, (zpos-i))));
			}

			if (i <= dists[3]) {
				//explosion = GameObject.Instantiate(Data.explotionRightPrefab, new Vector3( xpos + 0.5f, 0.5f, (zpos+i) + 0.5f), Quaternion.identity) as GameObject;
				//explosion.GetComponent<ParticleEmitter>().maxEmission = 0;
				reach[4]++;
				explosionChain.Add(new ExplosionField(i,Data.area.getCell(xpos, (zpos+i))));
			}
		}
	}
	
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
}


