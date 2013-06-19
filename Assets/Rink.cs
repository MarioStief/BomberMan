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
		
		// KONSTRUKTOREN
		
		public Rink (int width, int height, Vector2 rinkPosition)
		{
			initializeRink(width, height);
			this.rinkPosition = rinkPosition;
		}
		
		// <summary>
		// Erstellt die Gesamtspielfläche, bestehend aus Parzelen ( Parcel.cs);
		// Erstellt die dargestellte Spielfläche, bestehend aus MeshManipulator-Objekten.
		// </summary>
		private void initializeRink(int width, int height){
			
			// Init und Füllen von gameArea, so dass es einem BomberManspiel gleicht
			gameArea = new Parcel[height-1][];
			for(int i = 0; i < height-1; i++){
				gameArea[i] = new Parcel[width];
			}
			
			for(int i = 0; i < height-1; i++){
				for(int j = 0; j <width; j++){
					if ( i % 2 == 0 & j % 2 == 0){
						gameArea[i][j] = new Parcel(1.1f);	// Hoher Steinquader
					} else{
						gameArea[i][j] = new Parcel(1.0f);	// Bodenfläche
					}
				}
			}
			
			// Init der gezeichneten Fläche
			drawnArea = new MeshManipulator[height-1][];
			for(int j = 0; j < height-1; j++){
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
			highLightRinkPosition(true);
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
					drawnArea[r][s].setHeight( gameArea[r][s].getHeight() );
				}	
			}	
		}
		
		// <summary>
		// s.o., für einen einzelnen Würfel
		// </summary>
		public void updateHeight(int i, int j){
			
			//Debug.Log("drawnArea: " +drawnArea + "," + drawnArea[0].Length());
			//Debug.Log("gameArea: "  +gameArea.Length + "," + gameArea[0].Length());
			//Debug.Log("Input: "  + i + "," + j);


			drawnArea[i][j].setHeight(gameArea[i][/*(((int)rinkPosition.y+j)%(gameArea[0].Length/2))*/j].getHeight());
		}
		
		// <summary>
		// Erhöht die vertikale Lage des Orientierungspunktes rinkPosition
		// </summary>
		public void incrPositionHeight(){
			highLightRinkPosition(false);
			if (++rinkPosition.x >= gameArea.Length){
				rinkPosition.x = 0;	
			}
		}
		
		// <summary>
		// Vermindert die vertikale Lage des Orientierungspunktes rinkPosition
		// </summary>
		public void decrPositionHeight(){
			highLightRinkPosition(false);
			if (--rinkPosition.x < 0){
				rinkPosition.x = gameArea.Length-1;	
			}
		}
		
		// <summary>
		// Erhöht die horizontale Lage des Orientierungspunktes rinkPosition
		// </summary>
		public void incrPositionWidth(){
			highLightRinkPosition(false);
			if (++rinkPosition.y >= gameArea[0].Length){
				rinkPosition.y = 0;	
			}
		}
		
		// <summary>
		// Vermindert die vertikale Lage des Orientierungspunktes rinkPosition
		// </summary>
		public void decrPositionWidth(){
			highLightRinkPosition(false);
			if (--rinkPosition.y < 0){
				rinkPosition.y = gameArea[0].Length-1;	
			}
		}
		
		// <summary>
		// DEBUG-Methode;
		// Zweck: 
		// - Sicher stellen, dass rinkPosition korrekte Werte annimmt;
		// - Hilfe zur Orientierung auf dargestelltem Feld;
		//
		// mark == true: Färbt ausgehend von RinkPosition das mittlere Kugelfeld rot und gibt dess Parzellen-Nr. aus.
		// mark == false: Färbt momentanen Würfel weiß.
		// </summary>
		public void highLightRinkPosition(bool mark){
			
			if (DEBUG == false) return;
			
			int x = (int)rinkPosition.x;
			int y = (int)rinkPosition.y;
			
			if ( mark){
				
				//Debug.Log("RedCube- Position: " + ((x+gameArea.Length/2)%(gameArea.Length)) + ", " + ((y+gameArea[0].Length/4)%(gameArea[0].Length/2)));
				drawnArea[((x+gameArea.Length/2)%(gameArea.Length))][((y+gameArea[0].Length/4)%(gameArea[0].Length/2))].renderer.material.color = Color.red;	
			} else{
				drawnArea[((x+gameArea.Length/2)%(gameArea.Length))][((y+gameArea[0].Length/4)%(gameArea[0].Length/2))].renderer.material.color = Color.white;	
			}
		
		}
	}
}

