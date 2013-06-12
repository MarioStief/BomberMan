using System;
using UnityEngine;

namespace AssemblyCSharp
{
	/**
	 * Cell-Objekte zu
	 * */
	public class Cell
	{
		private int xpos, zpos;
		private float width, height;
		
		private int type;
		private GameObject obj;
		
		private bool bombPlaced;
		private Explosion explosion;
		
		private bool powerupOnCell;
		private PowerupType powerupType;
		private GameObject powerup;
		
		private bool exploding;
		private bool deadlyCell;
		
		public Cell()
		{
			xpos = 0;
			zpos = 0;
			setTypeObject();	
		}
		
		public Cell (int xpos, int zpos, float width, float height)
		{
			this.xpos = xpos;
			this.zpos = zpos;
			this.width = width;
			this.height = height;
			setTypeObject();
		}
		
		public Cell (int xpos, int zpos, float width, float height, int type){
			this.xpos = xpos;
			this.zpos = zpos;
			this.width = width;
			this.height = height;
			this.type = type;
			setTypeObject();
		}
		
		private void setTypeObject(){
			
			switch(type){
			case 0:	// Das Feld ist leer: Erstelle ein Partikelsystem an der Stelle zur Darstellung von Explosionen
				// NÖCHTS!
				GameObject.Destroy(obj);
				break;
			case 1:	// Das Feld enthält eine zerstörbare Kiste ( gefärbter Würfel)
				
				GameObject.Destroy(obj);
				obj = GameObject.Instantiate(Data.boxCube, new Vector3(xpos * width +0.5f, 0.5f, zpos * height +0.5f), Quaternion.identity) as GameObject;
				obj.name = "Box"+xpos+zpos;
				break;
				
			case 2:	// Das Feld enthält einen nicht-zerstörbaren Würfel
				GameObject.Destroy(obj);
				obj = GameObject.Instantiate(Data.solidCube, new Vector3(xpos * width +0.5f, 0.5f, zpos * height +0.5f), Quaternion.identity) as GameObject;
				obj.name = "Solid"+xpos+zpos;
				break;
			case 3:
				/* GameObject.Destroy(obj);
				obj = GameObject.CreatePrimitive(Primitive
				.Cube);
				obj = GameObject.Instantiate(Data.solidCube, new Vector3(xpos * width +0.5f, 0.5f, zpos * height +0.5f), Quaternion.identity) as GameObject;
				obj.renderer.material.color = Color.blue;
				obj.name = "Spawn"+xpos+zpos;//*/
				break;//*/
				//type = 1;
			default:
				Debug.Log("No such type!");
				break;
			}
		}
		
		public int getXPos(){
			return xpos;	
		}
		
		public int getZPos(){
			return zpos;	
		}
		
		public int getType(){
			return type;	
		}
		
		public float getWidth(){
			return width;	
		}
		
		public float getHeight(){
			return height;	
		}
		
		public bool setBomb(bool val, Explosion ex){
			if ( bombPlaced == false){
				bombPlaced = val;	
				//Debug.Log("Explo: " + ex);
				explosion = ex;
				return true;
			}	else{
				bombPlaced = val;
				return false;	
			}
		}
		
		public void setExploding(bool kill){
			exploding = kill;
			if (exploding) {
				if (bombPlaced) {
					Debug.Log("Explo: " + explosion +"("+ xpos + "," + zpos +")");
					if (explosion != null) {
						bombPlaced = false;
						explosion.startExplosion();
					}
				}
				if (type == 1){
					setType(0);	
				}
			} 			
		}
		
		public bool isExploding(){
			return exploding;	
		}
		
		public void addPowerup(GameObject powerup, PowerupType powerupType) {
			this.powerup = powerup;
			this.powerupType = powerupType;
			powerupOnCell = true;
		}

		public PowerupType destroyPowerup() {
			GameObject.Destroy(powerup);
			powerup = null;
			powerupOnCell = false;
			return powerupType;
		}

		
		public GameObject getGameObject(){
			return obj;	
		}
		
		public void setXPos(int aXPos){
			xpos = 	aXPos;
		}
		
		public void setZPos(int aZPos){
			zpos = aZPos;	
		}
		
		public void setType(int aType){
			type = aType;	
			setTypeObject();
		}
		
		public void setWidth(float aWidth){
			width = aWidth;	
		}
		
		public void setHeight(float aHeight){
			height = aHeight;	
		}
		
		public bool hasBomb(){
			return bombPlaced;
		}
		
		public bool hasPowerup() {
			return powerupOnCell;
		}
		
}
}