using UnityEngine;
using System;
using System.Collections.Generic;
using AssemblyCSharp;

public class Data : MonoBehaviour
{
	public static bool initialized;
	
	public static int maxPlayers;
	public static int width, height;
	
	public static GameObject bombPrefab;
	public static GameObject bombUpPowerupPreftab;
	public static GameObject flameUpPowerupPrefab;
	public static GameObject playerSpeedUpPowerupPrefab;
	public static GameObject explo0;
	public static GameObject explo1;
	public static GameObject explo2;
	public static GameObject explo3;
	
	public static GameObject player;
	public static CharacterController controller;
	
	public static GameObject solidCube;
	public static GameObject boxCube;
	
	public static GameArea area;
	
	public static FileExtractor initFile;
	
	public static List<Explosion> explosions;
	
	public void Start ()
	{
		// Lade init.txt
		initFile = new FileExtractor("init.txt");
		
		maxPlayers = (int)initFile.getValue("maxPlayers");
		width = (int)initFile.getValue("width");
		height = (int)initFile.getValue("height");
				
		player = GameObject.Find("Sphere");
		controller = player.GetComponent<CharacterController>();
		
		solidCube = GameObject.Find("SolidCube");
		boxCube = GameObject.Find("BoxCube");
		
		bombPrefab = GameObject.Find("bomb");
		bombUpPowerupPreftab = GameObject.Find("BombUp");
		flameUpPowerupPrefab = GameObject.Find("FlameUp");
		playerSpeedUpPowerupPrefab = GameObject.Find("PlayerSpeedUp");
		
		explo0 = GameObject.Find("Explo0");
		explo1 = GameObject.Find("Explo1");
		explo2 = GameObject.Find("Explo2");
		explo3 = GameObject.Find("Explo3");
		
		createArea();
		
		explosions = new List<Explosion>();
		
		// Erstelle Pool aus Powerups
		// Mit dem Parameter lässt sich die Poolgröße variieren
		Pool.createPool(1);
		
		initialized = true;
	}
	
	private void createArea(){
		// Erstelle Spielfläche abhängig von den Werten aus der init-datei
		area = new GameArea(width, height, initFile.getValue("cWidth"),initFile.getValue("cHeight"));
			
		fillArea();
			
		// Create Border-Cubes
		for(int i = 0; i < width; i++){
			Instantiate(Data.solidCube, new Vector3(i+0.5f, 0.5f, -0.5f), Quaternion.identity);	
		}
		
		for(int i = 0; i < width; i++){
			Instantiate(Data.solidCube, new Vector3(i+0.5f, 0.5f, height+0.5f), Quaternion.identity);	
		}
		
		for(int i = 0; i < height+2; i++){
			Instantiate(Data.solidCube, new Vector3(-0.5f, 0.5f, i-0.5f), Quaternion.identity);	
		}
		
		for(int i = 0; i < height+2; i++){
			Instantiate(Data.solidCube, new Vector3(width+0.5f, 0.5f, i-0.5f), Quaternion.identity);	
		}
	}
	
	private void fillArea(){
		
		// Array of Cell-Types
		//Debug.Log("Array-dim: " + width/4 + " , " + height/4);
		CellBlock[][] cellBlocks = new CellBlock[width/4][];
		for(int i = 0; i < width/4; i++){
			cellBlocks[i] = new CellBlock[height/4];
		}
		
		// 1.: Place Solid Rocks!
		calcSolidRocks(cellBlocks);
		
		// 2.: Find possible, random SpawnPoints
		Cell[] spawnPoints = calcSpawnPoints(cellBlocks);
		
		// 3.: Fill remaining entries with density% BoxCubes & create cells
		float density = initFile.getValue("Box-density");
		for(int i = 0; i < width/4; i++){
			for(int j = 0; j < height/4; j++){
				
				cellBlocks[i][j].fillWithBoxes(density);
				cellBlocks[i][j].createCells();
			}
		}
		
		// 4.: Set some solid rocks near north/west border
		for(int i = 0; i < width; i++){
		
			if ( UnityEngine.Random.value < 0.1f){
				area.getCell(i,height-1).setType(2);	
			}
		}
		for(int j = 0; j < height; j++){
		
			if ( UnityEngine.Random.value < 0.1f){
				area.getCell(0,j).setType(2);	
			}
		}
		
		
		//
		freeSingleBoxes();
		
		// Set Player-Position(s)
		
		int spawn = (int)(UnityEngine.Random.value*(maxPlayers-1));
		//Debug.Log(spawnPoints[spawn]);
		//spawnPoints[0] = area.getCell(0,0);
		
		player.transform.position = new Vector3(spawnPoints[spawn].getXPos() * initFile.getValue("cWidth") + 0.5f, 0.5f, spawnPoints[spawn].getZPos() * initFile.getValue("cHeight") + 0.5f);

	}
	
	private void calcSolidRocks(CellBlock[][] cellBlocks){
		
		//Debug.Log("YEAH");
		int blubb = 0;
		for(int i = 0; i < width; i += 4){
			for(int j = 0; j < height; j += 4){

				
				if (( i == 0 || i+4 >= width ||  j == 0 || j+4 >= height)){	// NO FiveDice at border-positions!
					blubb = (int)(UnityEngine.Random.value*96+1);	
				} else{
					blubb = (int)(UnityEngine.Random.value*99+1);
				}
				
				// Probabilities of cell-blocks:
				// TODO -> put it into enum
				// 50% FourDice
				// 35% Triangle
				// 12% Artifact
				// 3% FiveDice
//				Debug.Log(i + " , " + j);
				if ( blubb < 50){
				
					cellBlocks[i/4][j/4] = new CellBlock( i, j, CellBlockType.FOURDICE);
				} else if ( blubb < 85){
				
					// Determine kind of triangle
					blubb = (int)(UnityEngine.Random.value*3);
					
					if ( blubb == 0){
						
						cellBlocks[i/4][j/4] = new CellBlock( i, j, CellBlockType.TRIANGELDOWNLEFT);
						
					} else if ( blubb == 1){
						
						cellBlocks[i/4][j/4] = new CellBlock( i, j, CellBlockType.TRIANGLEUPLEFT);
					} else if ( blubb == 2){
						
						cellBlocks[i/4][j/4] = new CellBlock( i, j, CellBlockType.TRIANGLEUPRIGHT);
					} else if ( blubb == 3){
						
						cellBlocks[i/4][j/4] = new CellBlock( i, j, CellBlockType.TRIANGLEDOWNRIGHT);
					}
					
				} else if ( blubb < 97){
				
					cellBlocks[i/4][j/4] = new CellBlock( i, j, CellBlockType.ARTIFAKT);
				} else{
					cellBlocks[i/4][j/4] = new CellBlock( i, j, CellBlockType.FIVEDICE);	
				}
			}
		}	
	}
	
	private Cell[] calcSpawnPoints(CellBlock[][] cellBlocks){
		
		Cell[] spawnPoints = new Cell[maxPlayers];
		
		int i = 0;
		float angle = 2*Mathf.PI/maxPlayers;
		int x,z;
		float x_start = (width-1)/2 - 1, z_start =  0;
		while( i < maxPlayers){
			
			x = (int) (Mathf.Cos(angle*i)*x_start - Mathf.Sin(angle*i)*z_start + (width-1)/2);
			z = (int) (Mathf.Sin(angle*i)*x_start + Mathf.Cos(angle*i)*z_start + (height-1)/2);
			
			//Debug.Log(x + " : " +z);
			
			//if ( cellBlocks[x/4][z/4].readyForSpawn()){
				spawnPoints[i++] = cellBlocks[x/4][z/4].setSpawnPoint();
			//}
				//spawnPoints[i++] = cellBlocks[x/3xaf][z/3].setSpawnPoint();	
			//}
			
		}
		
		return spawnPoints;
	}
	
	private void freeSingleBoxes(){
		
		Cell currCell;
		Cell left, right, up, down;
		int non_null;
		for(int i = 0; i < width; i++){
			for(int j = 0; j < height; j++){
			
				non_null = 0;
				
				currCell = area.getCell(i,j);
				
				if ( i != 0){
					left = area.getCell(i-1,j);
					non_null++;
				} else{
					left = null;	
				}
				
				if ( i != width-1){
					right = area.getCell(i+1,j);
					non_null++;
				}else{
					right = null;	
				}
				
				if ( j != 0){
					up = area.getCell(i,j-1);
					non_null++;
				}else{
					up = null;	
				}
				
				if ( j != height-1){
					down = area.getCell(i,j+1);
					non_null++;
				}else{
					down = null;	
				}
				
				// Is it a single box?
				if (( left == null || left.getType() == 2) &&
					( right == null || right.getType() == 2) &&
					( up == null || up.getType() == 2) &&
					( down == null || down.getType() == 2)){
					
					
					int rand = (int)(UnityEngine.Random.value * (non_null-1));
					
					if ( rand == 0){
						if ( left != null){
							left.setType(1);
						} else if (right != null){
							right.setType(1);	
						} else if ( up != null){
							up.setType(1);	
						}else{
							down.setType(1);	
						}
					} else if (rand == 1){
						
						if (right != null){
							right.setType(1);	
						} else if ( up != null){
							up.setType(1);	
						}else if ( down != null){
							down.setType(1);	
						} else{
							left.setType(1);
						} 
						
					} else if (rand == 2){
						
						if ( up != null){
							up.setType(1);	
						}else if ( down != null){
							down.setType(1);	
						} else if (right != null){
							right.setType(1);	
						} else{
							left.setType(1);
						} 
						
					} else if (rand == 3){
						
						if ( down != null){
							down.setType(1);	
						} if ( up != null){
							up.setType(1);	
						}else if ( left != null){
							left.setType(1);
						} else{
							right.setType(1);	
						}
					}
				}
			}			
		}
	}
}


