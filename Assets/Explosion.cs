using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class Explosion
{
	private int xpos, zpos;
	
	private GameObject bomb;
	
	private GameObject explo0;
	private GameObject exploUp;
	private GameObject exploDown;
	private GameObject exploLeft;
	private GameObject exploRight;
	
	private int []dists;
	
	private float timer;
	
	private bool bSecond;
	
	private const float limit = 4.0f;
	private const float exDuration = 0.1f;
	
	public Explosion(int x, int z){
		xpos = x;
		zpos = z;
		
		bomb = GameObject.Instantiate(Data.bombPrefab, new Vector3(x + 0.5f, 0.3f, z + 0.5f), Quaternion.identity) as GameObject; 
		timer = 0.0f;
		
		dists = new int[4];
				instantiatePSystems();

		Data.explosions.Add(this);
		Data.area.getCell(xpos,zpos).setBomb(true, this);
	}
	
	public void startExplosion(){
		
		bSecond = true;
		timer = limit-exDuration;
		
		Data.area.getCell(xpos,zpos).setBomb(false, this);
		GameObject.Destroy(bomb);
		bomb = null;
		
		if ( explo0 != null) explo0.particleSystem.enableEmission = true;	// HACK
		if ( exploUp != null) exploUp.particleSystem.enableEmission = true;
		if ( exploDown != null) exploDown.particleSystem.enableEmission = true;
		if ( exploLeft != null) exploLeft.particleSystem.enableEmission = true;
		if ( exploRight != null) exploRight.particleSystem.enableEmission = true;
		
		updateCells(true);
	}	
	
	public void endExplosion(){
		
		GameObject.Destroy(explo0); explo0 = null;
		GameObject.Destroy(exploUp); exploUp = null;
		GameObject.Destroy(exploDown); exploDown = null;
		GameObject.Destroy(exploLeft); exploLeft = null;
		GameObject.Destroy(exploRight); exploRight = null;
		
		updateCells(false);
		Data.area.getCell(xpos, zpos).setKillOrder(false);
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
		
		explo0 = GameObject.Instantiate(Data.explo0, new Vector3( xpos + 0.5f, 0.5f, zpos + 0.5f), Quaternion.Euler(270,0,0)) as GameObject; 
		
		//Debug.Log(dists[0] + "," + dists[1] + "," + dists[2] + "," + dists[3]);
		
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
		
		int range = 3; // TODO: Player.range
		
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
	
	public bool addTime(float t){
		timer += t;
		
		if ( !bSecond && timer > limit){
			
			startExplosion();	
			timer -= exDuration;
		} else if ( bSecond && timer > limit){
			
			endExplosion();
			bSecond = false;
			return false;
		}
		
		return true;
	}
}


