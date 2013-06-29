using System;
using UnityEngine;

namespace AssemblyCSharp
{
	// <summary>
	// Rink ist die Schnittstelle zwischen dem dargestellten Ausschnitt (drawnArea) der Gesamtspielfläche (gameArea)
	// Ihre Hauptaufgabe besteht darin die Konsistenz zwischen beiden zu wahren. Als Orientierung dient dazu die
	// Parzelle rinkPosition. Dieser Punkt ist im gezeichneten Ausschnitt IMMER am Pol, am äußersten rechten Rand.
	// Welche logische Parzelle im sich im Bild befindet wird über rinkPosition bestimmt. 
	//
	// Die Kugel rotiert gegebene Parzellen eigenständig in Bewegungsrichtung. Neue Parzellen müssen nur bestimmt werden
	// wenn in Bewegungsrichtung neue Parzellen auftauchen. 
	// => In dem Fall ruft der SphereBuilder die Methode updateHeight(int i, int j) auf!!
	//
	// Im DEBUG-Modus wird ausgehen von dem Fixpunkt rinkPosition gezielt die mittlere Parzelle im dargestellten Ausschnitt
	// ROT gefärbt. Zugleich gibt die Konsole die Parzellen-Nr. im dargestellten Bildbereich aus. Es dient also zur Orientierung
	// </summary>
	//
	public class Rink
	{
		private bool DEBUG = true;					// Debug-Modus aktivieren/ deaktivieren (s.o.)
		
		public Parcel[][] gameArea;					// Gesamtspielfläche LOGIK
		public MeshManipulator[][] drawnArea;		// Gesamtspielfläche WÜRFEL-Transform
		
		private Vector2 rinkPosition;				// Die Orientierungs-Parzelle (s.o.);
		
		public GameObject []players = new GameObject[1];
		public GameObject player;
		
		// Temporär, um das Überfluten der Console zu vermeiden
		//private int oldX = 99999;
		//private int oldY = 99999;
		
		// KONSTRUKTOREN
		
		public Rink (int width, int height, Vector2 rinkPosition)
		{
			initializeRink(width, height);
			this.rinkPosition = rinkPosition;
			
			// Erstelle Pool aus Powerups
			// Mit dem Parameter lässt sich die Poolgröße variieren
			Static.setRink(this);
			PowerupPool.createPool(1);
			
			Player.setCurrentParcel(gameArea[0][0]);
		}
		
		// <summary>
		// Erstellt die Gesamtspielfläche, bestehend aus Parzelen ( Parcel.cs);
		// Erstellt die dargestellte Spielfläche, bestehend aus MeshManipulator-Objekten.
		// </summary>
		private void initializeRink(int width, int height){
			
			height = height-1;
			// Init und Füllen von gameArea, so dass es einem BomberManspiel gleicht
			gameArea = new Parcel[height][];
			for(int i = 0; i < height; i++){
				gameArea[i] = new Parcel[width];
			}
			
			for(int i = 0; i < height; i++){
				for(int j = 0; j <width; j++){
					if ( i % 2 == 0 & j % 2 == 0){
						gameArea[i][j] = new Parcel(2);	// Hoher Steinquader
					} else{
						gameArea[i][j] = new Parcel(0);	// Bodenfläche
					}
					gameArea[i][j].setIdentity(i,j);	// DEBUG
				}
			}
			
			for(int i = 0; i < 3*width; i++){
				gameArea[(int)(UnityEngine.Random.value*height)][(int)(UnityEngine.Random.value*width)].setType(1);	
			}
			
			for(int i = 0; i < height; i++){
				for(int j = 0; j < width; j++){
					Parcel right = gameArea[i][(j-1) < 0? width-1 : j-1];
					Parcel left = gameArea[i][(j+1)%(width)];
					Parcel up = gameArea[(i+1)%(height)][j];
					Parcel down = gameArea[(i-1) < 0? height-1 : i-1][j];
					gameArea[i][j].setNeightbours(right,left,up,down);		// Hoher Steinquader
				}
			}
			
			// Init der gezeichneten Fläche
			drawnArea = new MeshManipulator[height][];
			for(int j = 0; j < height; j++){
				drawnArea[j] = new MeshManipulator[width];
			}
		}
		
		// <summary>
		// Aufruf nach jeder Neuberechnung aller Kugelpunkte.
		// Veranlasst jeden Würfel über updateCoordinates seine neuen Eckpunkte
		// auszulesen und sich im Anschluss neu zu zeichnen. Die Höhenwerte bleiben
		// erhalten.
		// </summary>
		public void renderAll(){
		
			for(int i = 0; i < drawnArea.Length; i++){
				for(int j = 0; j < drawnArea[i].Length; j++){
					drawnArea[i][j].updateCoordinates();	
				}
			}
//			highLightRinkPosition(true);
		}
		
		// <summary>
		// Wie oben, für einen einzelnen Würfel
		// </summary>
		public void render(int i, int j){
			drawnArea[i][j].updateCoordinates();
		}
		
		// <summary>
		// Einmaliger Aufruf nach Initialisierung der Kugel.
		// Übergibt jedem Würfel seine eigenen Höhenwerte, die in der 
		// zugehörigen Parzellen-Klasse Parcel.cs gespeichert sind: gameArea[r][s].getHeight()
		// Der Würfel zeichnet sich automatisch neu.
		// </summary>
		public void updateHeight(){
			for(int r = 0; r < drawnArea.Length; r++){
				for(int s = 0; s < drawnArea[r].Length; s++){
					drawnArea[r][s].setParcel( gameArea[r][s]);
					drawnArea[r][s].setHeight( gameArea[r][s].getHeight() );
				}	
			}	
		}
		
		// <summary>
		// s.o., für einen einzelnen Würfel
		// </summary>
		public void updateHeight(int i, int j){
			drawnArea[i][j].setHeight(gameArea[i][j].getHeight());
		}
		
		// <summary>
		// Erhöht die vertikale Lage des Orientierungspunktes rinkPosition
		// </summary>
		//public void incrPositionHeight(){
		//	highLightRinkPosition(false);
		//	if (++rinkPosition.x >= gameArea.Length){
		///		rinkPosition.x = 0;	
		//	}
		//}
		
		// <summary>
		// Vermindert die vertikale Lage des Orientierungspunktes rinkPosition
		// </summary>
		//public void decrPositionHeight(){
		//	highLightRinkPosition(false);
		//	if (--rinkPosition.x < 0){
		//		rinkPosition.x = gameArea.Length-1;	
		//	}
		//}
		
		// <summary>
		// Erhöht die horizontale Lage des Orientierungspunktes rinkPosition
		// </summary>
		//public void incrPositionWidth(){
		//	highLightRinkPosition(false);
		//	if (++rinkPosition.y >= gameArea[0].Length){
		//		rinkPosition.y = 0;	
		//	}
		//}
		
		// <summary>
		// Vermindert die vertikale Lage des Orientierungspunktes rinkPosition
		// </summary>
		//public void decrPositionWidth(){
		//	highLightRinkPosition(false);
		//	if (--rinkPosition.y < 0){
		//		rinkPosition.y = gameArea[0].Length-1;	
		//	}
		//}
		
		// <summary>
		// DEBUG-Methode;
		// Zweck: 
		// - Sicher stellen, dass rinkPosition korrekte Werte annimmt;
		// - Hilfe zur Orientierung auf dargestelltem Feld;
		//
		// mark == true: Färbt ausgehend von RinkPosition das mittlere Kugelfeld rot und gibt dess Parzellen-Nr. aus.
		// mark == false: Färbt momentanen Würfel weiß.
		// </summary>
		//public void highLightRinkPosition(bool mark){
			
		//	if (DEBUG == false) return;
			
		//	int x = (int)rinkPosition.x;
		//	int y = (int)rinkPosition.y;
			
		//	if (mark){
		//		if (oldX != x || oldY != y) { // Temporär, um das Überfluten der Console zu vermeiden
		//			oldX = x;
		//			oldY = y;
		//			Debug.Log("RedCube-Position: " + ((x+gameArea.Length/2)%(gameArea.Length)) + ", " + ((y+gameArea[0].Length/4)%(gameArea[0].Length/2)));
		//		}
		//		drawnArea[((x+gameArea.Length/2)%(gameArea.Length))][((y+gameArea[0].Length/4)%(gameArea[0].Length/2))].renderer.material.color = Color.red;	
		//	} else{
		//		drawnArea[((x+gameArea.Length/2)%(gameArea.Length))][((y+gameArea[0].Length/4)%(gameArea[0].Length/2))].renderer.material.color = Color.white;	
		//	}
		
		//}
		
		//public String printCellCoordinates(Parcel cell) {
		//	for (int i = 0; i < gameArea.Length; i++) {
		//		for (int j = 0; j < gameArea[i].Length; j++) {
		//			if (cell == gameArea[i][j]) {
		//				return "[" + i + "," + j + "]";
		//			}
		//		}
		//	}
		//	return "";
		//}
		
		//public Parcel getCurrentParcel(int xDiff, int yDiff) {
		//	int x = (int)rinkPosition.x;
		//	int y = (int)rinkPosition.y;
		//	int xpos = ((x+gameArea.Length/2)%(gameArea.Length)) + xDiff;
		//	int ypos = ((y+gameArea[0].Length/4)%(gameArea[0].Length/2)) + yDiff;
		//	if (xDiff != 0 || yDiff != 0) {
		//		Debug.Log("CurrCell-Position: " + ((x+gameArea.Length/2)%(gameArea.Length)) + ", " + ((y+gameArea[0].Length/4)%(gameArea[0].Length/2)));
		//		Debug.Log("Active Cell: " + xpos + ", " + ypos + " (" + (xDiff < 0 ? "" : "+") + xDiff + ", " + (yDiff < 0 ? "" : "+") + yDiff + ")");
		//		drawnArea[xpos][ypos].renderer.material.color = Color.blue;
		//	}
		//	Parcel currentParcel = gameArea[xpos][ypos];
		//	return currentParcel;
		//}
		
		public GameObject[] getPlayers() {
			return players;
		}
		
		public Parcel[][] getGameArea() {
			return gameArea;
		}
	}
}

