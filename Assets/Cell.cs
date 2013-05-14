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
				GameObject.Destroy(obj);
				obj = new GameObject();
				obj.name = "cell"+xpos+""+zpos;
				ParticleSystem pSys = obj.AddComponent<ParticleSystem>();
				pSys.enableEmission = false;
				pSys.startLifetime = 0.15f;
				
				obj.transform.position = new Vector3(xpos * width +0.5f, 0.5f, zpos * height +0.5f);
				obj.transform.Rotate(new Vector3(-90.0f,0.0f,0.0f));
				break;
				
			case 1:	// Das Feld enthält eine zerstörbare Kiste ( gefärbter Würfel)
				GameObject.Destroy(obj);
				obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
				obj.name = "cell"+xpos+""+zpos;
				obj.transform.position = new Vector3(xpos * width +0.5f, 0.5f, zpos * height +0.5f);
				obj.renderer.material.color = new Color(0.7f,0.3f,0.3f,1.0f);
				break;
				
			case 2:	// Das Feld enthält einen nicht-zerstörbaren Würfel
				GameObject.Destroy(obj);
				obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
				obj.name = "cell"+xpos+""+zpos;
				obj.transform.position = new Vector3(xpos * width +0.5f, 0.5f, zpos * height +0.5f);
				break;
				
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
	}
}

