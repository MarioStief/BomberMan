using System;
using UnityEngine;

namespace AssemblyCSharp
{
	// <summary>
	// Klasse zum Handeln jeweiliger Parzellen, insbesondere der Höhe
	// und etwaiger GameObjects, die starr auf dieser Parzelle liegen ( etwa Upgrades)
	// </summary>
	public class Parcel
	{
		Vector3 center;
		
		float height;		// value in percent, meaning: 1.1f = 110% of 
		
		Color color = Color.white;		// Parcel-Farbe
		
		GameObject obj;		// Object auf der Parzelle, das gezeichnet werden soll.
		
		private bool bombOnCell; // Beschränkung von einer Bombe pro Feld überhaupt notwendig?
		private bool powerupOnCell;
		
		private PowerupType powerupType;
		
		public Parcel (){
			height = 1.0f;
		}
		
		public Parcel (float height)
		{
			this.height = height;
		}
		
		public Parcel (float height, GameObject obj)
		{
			this.height = height;
			this.obj = obj;
		}
		
		public void setGameObjectPosition(Vector3 v){
		
			if ( obj == null) return;
			
			obj.transform.position = v;
		}
		
		public bool hasGameObject(){
			return (obj != null);	
		}
		
		public void setGameObject(GameObject obj){
			this.obj = obj;
		}
		
		public void initGameObject(GameObject prefab, Vector3 position, Quaternion rotation){
			this.obj = GameObject.Instantiate(prefab, position, rotation) as GameObject;
		}
		
		public void destroyGameObject(){
			GameObject.Destroy(obj);
			obj = null;
		}
		
		public GameObject getGameObject(){
			return obj;	
		}
		
		public void resetHeight(){
			height = 1.0f;
		}
		
		public void setHeight(float height){
			this.height = height;	
		}
		
		public float getHeight(){
			return height;	
		}
		
		public Vector3 getCenter(){
			
			return center;
		}
		
		public void setCenter(Vector3 v){
			center = v;	
		}
		
		public void addPowerup(GameObject powerup, PowerupType powerupType) {
			this.obj = powerup;
			this.powerupType = powerupType;
			powerupOnCell = true;
		}

		public PowerupType destroyPowerup() {
			GameObject.Destroy(obj);
			obj = null;
			powerupOnCell = false;
			return powerupType;
		}
		
		public void setBomb(bool bombOnCell) {
			this.bombOnCell = bombOnCell;
		}
		
		public bool hasBomb() {
			return bombOnCell;
		}

		public bool hasPowerup() {
			return powerupOnCell;
		}
		
		public void setColor(Color col){
		
			color = col;
		}
		
		public Color getColor(){
			return color;	
		}
		
		
		/*
		public Vector3 getNormal() {
			// Normale vom Zellenmittelpunkt fUr Objektausrichtung
		}
		*/
	}
}

