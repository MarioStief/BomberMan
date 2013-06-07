using UnityEngine;
using System.Collections;

public class SphereBuilder : MonoBehaviour {
	
	private GameObject []spCubes;	
	
	public Vector3 [][][] sphereVertices;
	private MeshManipulator []spCubeMeshHandler;
	
	int r = 2; // Radius, todo: implement in spherecreation!
	
	public int n_B = 8; // Auflösung Breitenkreise; NET KLEINER ALS 4
	public int n_L = 8; // Auflösung Längenkreise ; ""     ""
		
	// Use this for initialization
	void Start () {
						
		tesselateSphere();		
	}
	
	private void tesselateSphere(){
	
		sphereVertices = new Vector3[r][][];
		for(int i = 0; i < r; i++){
			sphereVertices[i] = new Vector3[n_L][];
			for(int j = 0; j < n_L; j++){
				sphereVertices[i][j] = new Vector3[n_B/2+1];	
			}
		}
		
		calcSphereCoordinates();
		
		initializeCubes();
	}
	
	// Alle Punkte für gegebenes n_B, n_L berechnen
	private void calcSphereCoordinates(){
		
		float u,v;		
		Vector3 val;

		int m = 0;
		for( int j = 0; j < n_L; j++){
			
			v = (j*Mathf.PI/(n_L-1)) - Mathf.PI/2;
			
			for( int i = 0; i < n_B/2+1; i++){
			
				if ( n_B != 0){
					u = (i*2.0f/n_B) * Mathf.PI;
				} else {
					u = 0;	
				}
				
				val = F(1,u,v);

				if ( val != Vector3.zero){
					sphereVertices[0][j][i] = F(1,u,v);	// Get Vertices	
					sphereVertices[1][j][i] = F(2,u,v);	// Get Vertices	
				}
				
				if ( j == 0) break;
				if ( j == n_L-1) break;
				
				if ( m == 0) m = 1;
				else if ( m == 1) m = 0;
			}
		}	
	}
	
	// Berechnet Kugelkoordinate für
	// Längengrad u
	// und Breitengrad v
	private Vector3 F(float r, float u, float v){
		
		if ( u < 0 || u > 2*Mathf.PI ||  v < (-1)*Mathf.PI/2 || v > Mathf.PI/2) {
			
			return Vector3.zero;
		}
		
		return new Vector3(	r* Mathf.Cos(u) * Mathf.Cos(v),
							r* Mathf.Sin(u) * Mathf.Cos(v),
							r* Mathf.Sin(v));
	}
	
	
	private void initializeCubes(){
		
		spCubes = new GameObject[n_B/2*n_L+1];
		spCubeMeshHandler = new MeshManipulator[n_B/2*n_L+1];
		
		
		// Northpole		
		spCubes[0] = createSphereCube(Vector3.zero);	// Vector3.zero = no rotation!
		spCubeMeshHandler[0] = addMeshManipulator(spCubes[0], 
												new Vector3(1,0,0), new Vector3(1,0,0), new Vector3(0,0,0), new Vector3(0,0,0),
												new Vector3(1,1,1), new Vector3(1,1,0), new Vector3(0,1,1), new Vector3(0,1,0));	
		
		// All Cubes adjacent to the pole equal this one!
		
		for( int i = 1; i < n_B/2+1; i++){
			
			spCubes[i] = createSphereCube( new Vector3(0.0f,0.0f,360.0f/n_B*i)); 
			spCubeMeshHandler[i] = addMeshManipulator(spCubes[i],
														new Vector3(1,0,0), new Vector3(1,0,0), new Vector3(0,0,0), new Vector3(0,0,0),
														new Vector3(1,1,1), new Vector3(1,1,0), new Vector3(0,1,1), new Vector3(0,1,0));
		}
		
		// Southpole
		
		spCubes[n_B/2*n_L] = createSphereCube( Vector3.zero);	
		spCubeMeshHandler[n_B/2*n_L] = addMeshManipulator(spCubes[n_B/2*n_L],
															new Vector3(1,n_L-2,1), new Vector3(1,n_L-2,0), new Vector3(0,n_L-2,1), new Vector3(0,n_L-2,0),
															new Vector3(1,n_L-1,0), new Vector3(1,n_L-1,0), new Vector3(0,n_L-1,0), new Vector3(0,n_L-1,0));

		// All Cubes adjacent to the pole equal this one!
		
		for( int i = 1; i < n_B/2+1; i++){
			
			spCubes[n_B/2*n_L - i] = createSphereCube( new Vector3(0.0f,0.0f,360.0f/n_B*i)) ;
			spCubeMeshHandler[n_B/2*n_L - i] = addMeshManipulator(spCubes[n_B/2*n_L - i],
																	new Vector3(1,n_L-2,1), new Vector3(1,n_L-2,0), new Vector3(0,n_L-2,1), new Vector3(0,n_L-2,0),
																	new Vector3(1,n_L-1,0), new Vector3(1,n_L-1,0), new Vector3(0,n_L-1,0), new Vector3(0,n_L-1,0));	
		}
		
		// All Cubes in between
		for( int l = 1; l < n_L-2; l++){

			spCubes[l*n_B/2+1] = createSphereCube( Vector3.zero);
			spCubeMeshHandler[l*n_B/2+1] = addMeshManipulator(spCubes[l*n_B/2+1],
															new Vector3(1,l,1), new Vector3(1,l,0), new Vector3(0,l,1), new Vector3(0,l,0),
															new Vector3(1,l+1,1), new Vector3(1,l+1,0), new Vector3(0,l+1,1), new Vector3(0,l+1,0));

			for( int i = 1; i < n_B/2+1; i++){
	
				spCubes[l*n_B/2+i+1] = createSphereCube( new Vector3(0.0f,0.0f,360.0f/n_B*i));
				spCubeMeshHandler[l*n_B/2+i+1] = addMeshManipulator(spCubes[l*n_B/2+i+1],
																	new Vector3(1,l,1), new Vector3(1,l,0), new Vector3(0,l,1), new Vector3(0,l,0),
																	new Vector3(1,l+1,1), new Vector3(1,l+1,0), new Vector3(0,l+1,1), new Vector3(0,l+1,0));
			}
		}	
	}

	public GameObject createSphereCube(Vector3 quaternionEuler){
		
		GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
		c.transform.position = new Vector3(0,0,1);
		c.transform.rotation = Quaternion.Euler(quaternionEuler); // Rotation längs zum Äquator, d.h. auf momentanem breitengerad
		c.renderer.receiveShadows = false;
		c.renderer.castShadows = false;
		
		return c;
	}
	
	public MeshManipulator addMeshManipulator(GameObject c,
												Vector3 BNE, Vector3 BNW, Vector3 BSE, Vector3 BSW,
												Vector3 FNE, Vector3 FNW, Vector3 FSE, Vector3 FSW){
		MeshManipulator m = c.AddComponent<MeshManipulator>();
		m.setVertexPosition( BNE, BNW, BSE, BSW, FNE,  FNW,  FSE,  FSW);	// Eckpunkte	
		m.updateCoordinates();
		
		return m;
	}
	
}
