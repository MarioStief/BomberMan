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
		
		float height;		// value in percent, meaning: 1.1f = 110% of 
		
		GameObject obj;		// Object auf der Parzelle, das gezeichnet werden soll.
		
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
		
		public bool hasGameObject(){
			return obj == null;	
		}
		
		public void setGameObjet(GameObject obj){
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

	}
}

