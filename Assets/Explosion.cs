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
	
	/* Auskommentierter Code Henning
	private GameObject explo0;
	private GameObject exploUp;
	private GameObject exploDown;
	private GameObject exploLeft;
	private GameObject exploRight;
	*/
	
	private int []dists;
	private bool waitingForBombExplosion = true;

	private List<ExplosionField> explosionChain = new List<ExplosionField>();
	
	private float timer;
	private float createTime;
	
	/* Auskommentierter Code Henning
	private bool bSecond;
	
	private const float limit = 3.0f;
	private float exDuration = 0.3f;
	*/
	
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
		
		/* Auskommentierter Code Henning
		bSecond = true;
		timer = limit-exDuration;
		*/
		
		Data.area.getCell(xpos,zpos).setBomb(false, this);
		GameObject.Destroy(bomb);
		bomb = null;
		
		
		/* Auskommentierter Code Henning
		if ( explo0 != null) explo0.particleSystem.enableEmission = true;	// HACK
		if ( exploUp != null) exploUp.particleSystem.enableEmission = true;
		if ( exploDown != null) exploDown.particleSystem.enableEmission = true;
		if ( exploLeft != null) exploLeft.particleSystem.enableEmission = true;
		if ( exploRight != null) exploRight.particleSystem.enableEmission = true;
		*/
		
		foreach (ExplosionField explosionField in explosionChain) {
			if (explosionField.getDelay() == 0)
				explosionField.getExplosion().GetComponent<ParticleEmitter>().enabled = true;
			explosionField.decrement(); // Zähle Delay-Ticker runter
		}
		
		updateCells(true);
	}
	
	void Update() {
		float elapsedTime = Time.time - createTime;
		Debug.Log(elapsedTime);
		if (waitingForBombExplosion) {
			if (elapsedTime > EXPLOSIONTIMER) {
				waitingForBombExplosion = false;
				startExplosion();
				createTime = Time.time;
			}
		} else {
			if (elapsedTime > 0.5f)
				endExplosion();

			if (elapsedTime > 0.3f) {
				foreach (ExplosionField explosionField in explosionChain)
					if (explosionField.getExplosion() != null)
						explosionField.getExplosion().GetComponent<ParticleEmitter>().maxEmission = 0;
			}

			
			if (elapsedTime > 0.1f) { // alle 100 ms
				foreach (ExplosionField explosionField in explosionChain) {
					bool stillRunning = false;
					if (explosionField.getDelay() == 0) {
						explosionField.getExplosion().GetComponent<ParticleEmitter>().enabled = true;
						stillRunning = true;
					} else if (explosionField.getDelay() == -2) {
						explosionField.getExplosion().GetComponent<ParticleEmitter>().maxEmission = 0;
					}

					explosionField.decrement(); // Zähle Delay-Ticker runter
					if (stillRunning)
						createTime = Time.time;
				}
			}
		}
	}
	
	public void endExplosion() {
		
		/* Auskommentierter Code Henning
		GameObject.Destroy(explo0); explo0 = null;
		GameObject.Destroy(exploUp); exploUp = null;
		GameObject.Destroy(exploDown); exploDown = null;
		GameObject.Destroy(exploLeft); exploLeft = null;
		GameObject.Destroy(exploRight); exploRight = null;
		*/
		
		foreach (ExplosionField explosionField in explosionChain)
			GameObject.Destroy(explosionField.getExplosion());

		// DEBUG: Bombe nach Explosion erzeugen mit 25 %
		Player.destroyBomb();
		if (new System.Random().Next(0, 1) == 0)
			PowerupPool.setPowerup(xpos, zpos);
		
		updateCells(false);
		Data.area.getCell(xpos, zpos).setKillOrder(false);
		
		Destroy (this);
	}
	
	private void updateCells(bool exploRunning){
		
		
		// upward-boxes
		for(int i = 1; i <= dists[0]; i++){
			Data.area.getCell(xpos-i, zpos).setKillOrder(exploRunning);
		}
		
		// downward-boxes
		for(int i = 1; i <= dists[1]; i++){
			Data.area.getCell(xpos+i, zpos).setKillOrder(exploRunning);
		}
		
		// right-boxes
		for(int i = 1; i <= dists[3]; i++){
			Data.area.getCell(xpos, zpos+i).setKillOrder(exploRunning);
		}
		
		// left-boxes
		for(int i = 1; i <= dists[2]; i++){
			Data.area.getCell(xpos, zpos-i).setKillOrder(exploRunning);
		}		
		
	}
	
	private void instantiatePSystems(){
		
		getDistances();
		
		GameObject explosion;
		
		explosion = GameObject.Instantiate(Data.explotionBombPrefab, new Vector3( xpos + 0.5f, 0.5f, zpos + 0.5f), Quaternion.Euler(270,0,0)) as GameObject;
		explosion.GetComponent<ParticleEmitter>().enabled = false;
		explosionChain.Add(new ExplosionField(explosion,0));

		Debug.Log(dists[0] + "," + dists[1] + "," + dists[2] + "," + dists[3]);
		
		for (int i = 1; i <= Player.getFlamePower(); i++) {
			if (i <= dists[0]) {
				explosion = GameObject.Instantiate(Data.explotionUpPrefab, new Vector3( (xpos-i) + 0.5f, 0.5f, zpos + 0.5f), Quaternion.Euler(0,-90,0)) as GameObject;
				explosion.GetComponent<ParticleEmitter>().enabled = false;
				explosionChain.Add(new ExplosionField(explosion,i));
			}

			if (i <= dists[1]) {
				explosion = GameObject.Instantiate(Data.explotionDownPrefab, new Vector3( (xpos+i) + 0.5f, 0.5f, zpos + 0.5f), Quaternion.Euler(0,-90,0)) as GameObject;
				explosion.GetComponent<ParticleEmitter>().enabled = false;
				explosionChain.Add(new ExplosionField(explosion,i));
			}

			if (i <= dists[2]) {
				explosion = GameObject.Instantiate(Data.explotionLeftPrefab, new Vector3( xpos + 0.5f, 0.5f, (zpos-i) + 0.5f), Quaternion.Euler(0,-90,0)) as GameObject;
				explosion.GetComponent<ParticleEmitter>().enabled = false;
				explosionChain.Add(new ExplosionField(explosion,i));
			}

			if (i <= dists[3]) {
				explosion = GameObject.Instantiate(Data.explotionRightPrefab, new Vector3( xpos + 0.5f, 0.5f, (zpos+i) + 0.5f), Quaternion.Euler(0,-90,0)) as GameObject;
				explosion.GetComponent<ParticleEmitter>().enabled = false;
				explosionChain.Add(new ExplosionField(explosion,i));
			}
		}
		
		
		/* Auskommentierter Code Henning
		explo0 = GameObject.Instantiate(Data.explo0, new Vector3( xpos + 0.5f, 0.5f, zpos + 0.5f), Quaternion.Euler(270,0,0)) as GameObject; 
		
		Debug.Log(dists[0] + "," + dists[1] + "," + dists[2] + "," + dists[3]);
		
		if ( dists[0] == 0){
			exploUp = null;	
		} else if (dists[0] == 1){
			exploUp = GameObject.Instantiate(Data.explo1, new Vector3( (xpos-1) + 0.5f, 0.5f, zpos + 0.5f), Quaternion.Euler(0,-90,0)) as GameObject;
		} else if (dists[0] == 2){
			exploUp = GameObject.Instantiate(Data.explo2, new Vector3( (xpos-1) + 0.5f, 0.5f, zpos + 0.5f), Quaternion.Euler(0,-90,0)) as GameObject;
		} else{
			exploUp = GameObject.Instantiate(Data.explo3, new Vector3( (xpos-1) + 0.5f, 0.5f, zpos + 0.5f), Quaternion.Euler(0,-90,0)) as GameObject;
		}
		
		if ( dists[1] == 0){
			exploDown = null;	
		} else if (dists[1] == 1){
			exploDown = GameObject.Instantiate(Data.explo1, new Vector3( (xpos+1) + 0.5f, 0.5f, zpos + 0.5f), Quaternion.Euler(0,-270,0)) as GameObject;
		} else if (dists[1] == 2){
			exploDown = GameObject.Instantiate(Data.explo2, new Vector3( (xpos+1) + 0.5f, 0.5f, zpos + 0.5f), Quaternion.Euler(0,-270,0)) as GameObject;
		} else{
			exploDown = GameObject.Instantiate(Data.explo3, new Vector3( (xpos+1) + 0.5f, 0.5f, zpos + 0.5f), Quaternion.Euler(0,-270,0)) as GameObject;
		}
		
		if ( dists[2] == 0){
			exploLeft = null;	
		} else if (dists[2] == 1){
			exploLeft = GameObject.Instantiate(Data.explo1, new Vector3( xpos + 0.5f, 0.5f, (zpos-1) + 0.5f), Quaternion.Euler(0,-180,0)) as GameObject;
		} else if (dists[2] == 2){
			exploLeft = GameObject.Instantiate(Data.explo2, new Vector3( xpos + 0.5f, 0.5f, (zpos-1) + 0.5f), Quaternion.Euler(0,-180,0)) as GameObject;
		} else{
			exploLeft = GameObject.Instantiate(Data.explo3, new Vector3( xpos + 0.5f, 0.5f, (zpos-1) + 0.5f), Quaternion.Euler(0,-180,0)) as GameObject;
		}
		
		if ( dists[3] == 0){
			exploRight = null;	
		} else if (dists[3] == 1){
			exploRight = GameObject.Instantiate(Data.explo1, new Vector3( xpos + 0.5f, 0.5f, (zpos+1) + 0.5f), Quaternion.identity) as GameObject;
		} else if (dists[3] == 2){
			exploRight = GameObject.Instantiate(Data.explo2, new Vector3( xpos + 0.5f, 0.5f, (zpos+1) + 0.5f), Quaternion.identity) as GameObject;
		} else{
			exploRight = GameObject.Instantiate(Data.explo3, new Vector3( xpos + 0.5f, 0.5f, (zpos+1) + 0.5f), Quaternion.identity) as GameObject;
		}//*/
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
	
	/* Auskommentierter Code Henning
	public bool addTime(float t){
		timer += t;
		
		if ( !bSecond && timer > limit){
			
			startExplosion();	
			timer -= exDuration;
		} else if ( bSecond && timer > limit){
			
			//endExplosion();
			bSecond = false;
			return false;
		}
		
		return true;
	}
	*/
}


