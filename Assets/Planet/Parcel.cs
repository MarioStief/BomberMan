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
		
		const float normalLevel = 1.0f;
		const float upperLevel = 1.1f;	
		
		public Parcel right, left, up, down;	// Nachbar-Parzellen
		
		int parcelType;					// Typ der Parzelle: 0 == leer, 1 == Holzkiste, 2 == Steinblock
		
		Vector3 center;					// Center of Parcel

		private int lpos;				// Position der aktuellen Parzelle rink.gameArea ist [lpos][bpos]
		private int bpos;
		
		Color color;					// Parcel-Farbe
		
		bool highlight = false;
		Color highlightColor = Color.cyan;
		
		float height = 1.0f;
		
		GameObject obj;					// Object auf der Parzelle, das gezeichnet werden soll.
		
		private bool bombOnCell; 		// Beschränkung von einer Bombe pro Feld überhaupt notwendig?
		private bool powerupOnCell;
		private Explosion explosion;
		private bool exploding;
		
		private PowerupType powerupType;
		
		public Parcel (){
			height = 1.0f;
		}
		
		public Parcel (int type)
		{
			setType(type);
		}
		
		public Parcel (float height, GameObject obj)
		{
			this.height = height;
			this.obj = obj;
		}
		
		public MeshManipulator getMeshManipulator() {
			return GameObject.Find("Sphere").GetComponent<SphereBuilder>().getRink().drawnArea[lpos][bpos];
		}
		
		public void setExploding (bool exploding) {
			this.exploding = exploding;
		}
		
		public bool isExploding () {
			return exploding;
		}

		public void decreaseHeight() {
			if (height > 1.009f) { // also 1.01 oder höher
				height -= 0.01f;
				for (int i = 0; i < 5; i++) {
					if (i < 2 || i > 3)
					getMeshManipulator().updateCoordinates();
					Debug.Log("MeshManipulator says: " + getMeshManipulator().vertexPosition[i].x);
				}
			}
		}
		
		public void setIdentity(int lpos, int bpos) {
			this.lpos = lpos;
			this.bpos = bpos;
		}
		
		public void setType(int type){
		
			//if ( type == parcelType) return;
			
			parcelType = type;
			if ( parcelType == 0){
				height = 1.0f;	
				color = Color.green;
			} else if ( parcelType == 1){
				height = 1.1f;	
				color = new Color(0.5f,0.5f,0.0f,1.0f);
			} else {
				height = 1.1f;	
				color = Color.gray;
			}
		}
		
		public int getType(){
			return parcelType;	
		}
		
		public void setNeightbours(Parcel right, Parcel left, Parcel up, Parcel down){
			this.right = right;
			this.left = left;
			this.up = up;
			this.down = down;
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

		public void addPowerup(Powerup powerup, GameObject prefab) {
			this.obj = GameObject.Instantiate(prefab, getCenterPos(), Quaternion.identity) as GameObject;
			this.powerupType = powerup.getType();
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
		
		public void setExplosion(Explosion explosion) {
			this.explosion = explosion;
		}
		
		public Explosion getExplosion() {
			return explosion;
		}
		
		public bool hasPowerup() {
			return powerupOnCell;
		}
		
		public void setColor(Color col){
		
			color = col;
		}
		
		public void hightlightColor(bool b){
		
			highlight = b;
		}
		
		public Color getColor(){
			
			
			return highlight? highlightColor : color;	
		}
		
		public String getCoordinates() {
			return "[" + lpos + "," + bpos + "]";
		}
		
		public int getLpos() {
			return lpos;
		}

		public int getBpos() {
			return bpos;
		}

		public Parcel getSurroundingCell(int lpos, int bpos) {
			GameObject sphere = GameObject.Find("Sphere");
			SphereBuilder sphereHandler = sphere.GetComponent<SphereBuilder>();
			Rink rink = sphereHandler.getRink();
			Parcel[][] cell = rink.getGameArea();
			
			lpos += this.lpos;
			bpos += this.bpos;
			
			if (lpos >= cell.Length)
				lpos %= cell.Length;
			if (lpos < 0)
				lpos += cell.Length;
			if (bpos >= cell[lpos].Length)
				bpos %= cell[lpos].Length;
			if (bpos < 0)
				bpos += cell[lpos].Length;
			
			return cell[lpos][bpos];
		}
		
		public void colorCell(Color color) {
			GameObject sphere = GameObject.Find("Sphere");
			SphereBuilder sphereHandler = sphere.GetComponent<SphereBuilder>();
			Rink rink = sphereHandler.getRink();
			rink.drawnArea[lpos][bpos].renderer.material.color = color;
		}
		
		public Vector3 getCenterPos() {
			return GameObject.Find("Sphere").GetComponent<SphereBuilder>().getRink().drawnArea[lpos][bpos].getCenter();
		}
		
		/*
		public Vector3 getNormal() {
			// Normale vom Zellenmittelpunkt fUr Objektausrichtung
		}
		*/
	}
}

