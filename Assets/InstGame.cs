using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class InstGame : MonoBehaviour {
	
	public FileExtractor initFile;
	public GameArea area;
	
	// Use this for initialization
	void Start () {
		
		// Lade init.txt
		initFile = new FileExtractor("init.txt");
		
		// Erstelle Spielfläche abhängig von den Werten aus der init-datei
		area = new GameArea((int)initFile.getValue("width"), (int)initFile.getValue("height"),
							initFile.getValue("cWidth"),initFile.getValue("cHeight"));
		
		
		for(int i = 0; i < (int)initFile.getValue("width"); i ++){
			for(int j = 0; j < (int)initFile.getValue("height"); j ++){
				
				if ( i%2 == 0 && j%2 == 0){
					area.getCell(i,j).setType(2);
				} else{
					area.getCell(i,j).setType( Random.value < 0.1f ? 1 : 0);
				}
				
			}
		}
		
		// Create Border-Cubes
		GameObject borderNorth = GameObject.CreatePrimitive(PrimitiveType.Cube);
		borderNorth.transform.localScale = new Vector3((int)initFile.getValue("width")+1, 1, 1);
		borderNorth.transform.position = new Vector3(initFile.getValue("width")/2f,0.5f,-0.5f);
						
		GameObject borderSouth = GameObject.CreatePrimitive(PrimitiveType.Cube);
		borderSouth.transform.localScale = new Vector3((int)initFile.getValue("width")+1, 1, 1);
		borderSouth.transform.position = new Vector3(initFile.getValue("width")/2f,0.5f,initFile.getValue("height")+0.5f);
		
		GameObject borderWest = GameObject.CreatePrimitive(PrimitiveType.Cube);
		borderWest.transform.localScale = new Vector3(1, 1, (int)initFile.getValue("height")+2);
		borderWest.transform.position = new Vector3(-0.5f,0.5f,initFile.getValue("height")/2f);
		
		GameObject borderEast = GameObject.CreatePrimitive(PrimitiveType.Cube);
		borderEast.transform.localScale = new Vector3(1, 1, (int)initFile.getValue("height")+2);
		borderEast.transform.position = new Vector3((int)initFile.getValue("width")+0.5f,0.5f,
													initFile.getValue("height")/2f);
	}
	
	// Update is called once per frame
	void Update () {
	}
}
