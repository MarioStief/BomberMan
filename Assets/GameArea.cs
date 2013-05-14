using System;
using UnityEngine;
//using AssemblyCSharp;

namespace AssemblyCSharp
{
	/**
	 * Die Spielfläche eist ein Array aus Zellen ( Cell-Objekten)
	 * */
	public class GameArea
	{
		private Cell[][] plane;
		
		private float cWidth, cHeight;
		
		//private GameObject subPlane;
		
		public GameArea (int xMax, int zMax, float width, float height)
		{
			//subPlane = GameObject.Find("Plane");
			//subPlane.transform.localScale = new Vector3( xMax/10f, subPlane.transform.localScale.y, zMax/10f);
			
			cWidth = width;
			cHeight = height;
			
			plane = new Cell[xMax][];
			for(int i = 0; i < xMax; i++){
				plane[i] = new Cell[zMax];	
				for(int j = 0; j < zMax; j++){
					plane[i][j] = new Cell(i,j,width,height,0);	
				}
			}
		}
		
		public Cell getCell(int x, int z){
			return plane[x][z];	
		}
		
		/**
		 * Gibt Zelle zurück, die x,z enthält. 
		 * */
		public Cell getCell(float x, float z){
		
			int x_m = (int)((x+0.5f - cWidth/2 )/ cWidth);
			int z_m = (int)((z+0.5f - cHeight/2)/ cHeight);
				
			Debug.Log(x_m + ", " + z_m + ": Cell-Type: " + plane[x_m][z_m].getType());
			
			return plane[x_m][z_m];
		}
	}
}

