using UnityEngine;
using System.Collections;

public class SphereBuilder : MonoBehaviour {
	
	
	private Vector3 [] sphereVertices;
	private Vector2[] sphereUV;
	private int [] sphereTriangles;
	int k = 0;
	float r = 1; // Radius, todo: implement in spherecreation!
	
	private int n_B = 32; // Auflösung Breitenkreise; NET KLEINER ALS 4
	private int n_L = 16; // Auflösung Längenkreise ; ""     ""
	
	// Use this for initialization
	void Start () {
		
		tesselateSphere();

		Mesh mesh = new Mesh();
		mesh.vertices = sphereVertices;
        mesh.uv = sphereUV;
        mesh.triangles = sphereTriangles;
		mesh.RecalculateNormals();
		
		GetComponent<MeshFilter>().mesh = mesh;
		
	}
	
	private void tesselateSphere(){
	
		sphereVertices = new Vector3[(n_L-2)*n_B +2];
		sphereUV = new Vector2[(n_L-2)*n_B +2];
		sphereTriangles = new int[((n_L-2)*n_B +2)*12];
		
		calcSphereCoordinates();
		
		assignTriangles();
	}
	
	private void assignTriangles(){
	
		// Bereich am Nordpol
		int n = 0; 
		for(int i = 1; i <= n_B; i++){
		
			sphereTriangles[n] 	 = 0;				// POL
			if ( i+1 == n_B+1){
				sphereTriangles[n+1] = 1;
			} else{
				sphereTriangles[n+1] = i+1;
			}
			sphereTriangles[n+2] = i;
			
			n += 3;
		}//*/
		
		// Bereich am Südpol
		for(int i = 1; i <= n_B; i++){
		
			sphereTriangles[n] 	 = k-1;				// POL
			if ( i+1 == n_B+1){
				sphereTriangles[n+1] =  k - 2;
			} else{
				sphereTriangles[n+1] = k-1 -(i+1);
			}
			sphereTriangles[n+2] = k-1 -i;
			n += 3;

		}//*/

		// Alles zwischen den Polen
		for(int i = 1; i <= n_B; i++){
			for(int j = 0; j < n_L -3; j++){
				
				// Dreieck NE, NW, SW
				sphereTriangles[n] 	 = j*(n_B) + i;
				if ( i+1 == n_B+1){
					sphereTriangles[n+1] = j*(n_B) + 1;
					sphereTriangles[n+2] = (j+1)*(n_B) + 1;
				} else{
					sphereTriangles[n+1] = j*(n_B) + i + 1;
					sphereTriangles[n+2] = (j+1)*(n_B) + i + 1;
				}
				
				n += 3;
				
				// Dreieck NE, SE, SW
				sphereTriangles[n] 	 = j*(n_B) + i;
				if ( i+1 == n_B+1){
					sphereTriangles[n+1] = (j+1)*(n_B) + 1;
				} else{
					sphereTriangles[n+1] = (j+1)*(n_B) + i + 1;
				}
				sphereTriangles[n+2] = (j+1)*(n_B) + i;
				
				n += 3;	
				
			}			
		}//*/

	}
	
	// Alle Punkte für gegebenes n_B, n_L berechnen
	private void calcSphereCoordinates(){
		
		float u,v;		
		Vector3 val;

		
		for( int j = 0; j < n_L; j++){
			
			v = (j*Mathf.PI/(n_L-1)) - Mathf.PI/2;
			
			for( int i = 0; i < n_B; i++){
			
				if ( n_B != 0){
					u = (i*2.0f/n_B) * Mathf.PI;
				} else {
					u = 0;	
				}
				
				val = F(u,v);

				if ( val != Vector3.zero){
					sphereVertices[k] = F(u,v);	// Get Vertices
					
					sphereUV[k++] = new Vector2( u/(2*Mathf.PI) , (Mathf.PI/2 - v)/ Mathf.PI);
				}
				
				if ( j == 0) break;
				if ( j == n_L-1) break;
			}
		}	
	}
	
	// Berechnet Kugelkoordinate für
	// Längengrad u
	// und Breitengrad v
	private Vector3 F(float u, float v){
		
		if ( u < 0 || u > 2*Mathf.PI ||  v < (-1)*Mathf.PI/2 || v > Mathf.PI/2) {
			
			return Vector3.zero;
		}
		
		return new Vector3(	Mathf.Cos(u) * Mathf.Cos(v),
							Mathf.Sin(u) * Mathf.Cos(v),
							Mathf.Sin(v));
	}
	
	
//-------------------------------------------------------------
//>>>>>>>>>>>>>>>>>>>>>>>> UPDATE <<<<<<<<<<<<<<<<<<<<<<<<<<<<<
//-------------------------------------------------------------
	
	float x, y, z;
	
	Vector3 moveDirection;
	Vector3 rotation;
	
	float verticalAngle;
	float horizontalAngle;
	
	Mesh mesh;
	Vector3[] meshTriangles;
	
	public void Update(){
	
		x = transform.position.x;
		y = transform.position.y;
		z = transform.position.z;
		
		mesh = GetComponent<MeshFilter>().mesh;
		meshTriangles = mesh.vertices;
		
		moveDirection = new Vector3(Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal"));
		
		Debug.Log("hoz: " + moveDirection.z + " vert: " +moveDirection.x);
		
		//if ( moveDirection.z != 0){
			
			horizontalAngle += Mathf.Sign(moveDirection.z) * Mathf.Acos( 1.0f - (moveDirection.z* moveDirection.z) / ( 2 * r * r));
			
			for(int i = 0; i < sphereVertices.Length; i++){
				
				meshTriangles[i] = new Vector3(Mathf.Cos( horizontalAngle)*sphereVertices[i].x - Mathf.Sin( horizontalAngle)*sphereVertices[i].y ,
								 	   		   Mathf.Sin( horizontalAngle)*sphereVertices[i].x + Mathf.Cos( horizontalAngle)*sphereVertices[i].y ,
							      	  		   sphereVertices[i].z);
			}
			
		//}
		
		//if ( moveDirection.x != 0){
			//Debug.Log("vert: " +moveDirection.x);
			verticalAngle += Mathf.Sign(moveDirection.x) * Mathf.Acos( 1.0f - (moveDirection.x* moveDirection.x) / ( 2 * r * r));
			
			
			for(int i = 0; i < sphereVertices.Length; i++){
				
				meshTriangles[i] = new Vector3(Mathf.Cos( verticalAngle)*meshTriangles[i].x + Mathf.Sin( verticalAngle)*meshTriangles[i].z ,
								 	  		   meshTriangles[i].y ,
							      	  		   -Mathf.Sin( verticalAngle)*meshTriangles[i].x + Mathf.Cos( verticalAngle)*meshTriangles[i].z);
			}
			
		//}//*/
		
		mesh.vertices = meshTriangles;
	}
	
}
