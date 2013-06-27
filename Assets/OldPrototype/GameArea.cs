/*
using System;
using System.Collections.Generic;
using UnityEngine;
//using AssemblyCSharp;

namespace AssemblyCSharp
{
	/**
	 * Die Spielfläche eist ein Array aus Zellen ( Cell-Objekten)
	 * */
/*	public class GameArea
	{
		private Cell[][] plane;
				
		public int xMax, zMax;
		public static float cWidth, cHeight;
		
		
		public GameArea (int xMax, int zMax, float width, float height)
		{
			
			this.xMax = xMax;
			this.zMax = zMax;
			
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
			//Debug.Log(x + "," + z);
			return plane[x][z];	
		}
		
		/**
		 * Gibt Zelle zurück, die x,z enthält. 
		 * */
/*		public Cell getCell(float x, float z){
		
			int x_m = (int)(x); // TODO!!
			int z_m = (int)(z);
				
			//Debug.Log(x_m + ", " + z_m + ": Cell-Type: " + plane[x_m][z_m].getType());
			
			return plane[x_m][z_m];
		}
		
		public int getWidth(){
			return xMax;	
		}
		
		public int getHeight(){
			return zMax;	
		}
	}
}
*/