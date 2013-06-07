using UnityEngine;
using System.Collections;

public class MeshManipulator : MonoBehaviour {
	
	SphereBuilder sphere;
	Mesh cubeMesh;
	
	Vector3 []vertexPosition;

	// Use this for initialization
	void Awake () {
		
		sphere = GameObject.Find("Sphere").GetComponent<SphereBuilder>();
		cubeMesh = GetComponent<MeshFilter>().mesh;
		vertexPosition = new Vector3[8];
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void setVertexPosition(Vector3 BNE, Vector3 BNW, Vector3 BSE, Vector3 BSW,
							Vector3 FNE, Vector3 FNW, Vector3 FSE, Vector3 FSW){

		vertexPosition[0] = BNE;
		vertexPosition[1] = BNW;
		vertexPosition[2] = BSE;
		vertexPosition[3] = BSW;
		
		vertexPosition[4] = FNE;
		vertexPosition[5] = FNW;
		vertexPosition[6] = FSE;
		vertexPosition[7] = FSW;
		
	}
	
	public void updateCoordinates(){
		
		cubeMesh.RecalculateBounds();
		setVertices(sphere.sphereVertices[(int)vertexPosition[0].x][(int)vertexPosition[0].y][(int)vertexPosition[0].z],
					sphere.sphereVertices[(int)vertexPosition[1].x][(int)vertexPosition[1].y][(int)vertexPosition[1].z],
					sphere.sphereVertices[(int)vertexPosition[2].x][(int)vertexPosition[2].y][(int)vertexPosition[2].z],
					sphere.sphereVertices[(int)vertexPosition[3].x][(int)vertexPosition[3].y][(int)vertexPosition[3].z],
					sphere.sphereVertices[(int)vertexPosition[4].x][(int)vertexPosition[4].y][(int)vertexPosition[4].z],
					sphere.sphereVertices[(int)vertexPosition[5].x][(int)vertexPosition[5].y][(int)vertexPosition[5].z],
					sphere.sphereVertices[(int)vertexPosition[6].x][(int)vertexPosition[6].y][(int)vertexPosition[6].z],
					sphere.sphereVertices[(int)vertexPosition[7].x][(int)vertexPosition[7].y][(int)vertexPosition[7].z]
		);	
	}
	
	// <summary>
	// Sets all 24 vertices of an Unity-Cube by placing all eight different.
	// real vertices at their corresponding positions in the unity-vertex-array.
	// </summary>
	// <param name="v">Array containing all real 8 vertices. The order in which they
	// must be presented has to equal the parameter-order of the overloaded function.
	// </param>
	public void setVertices(Vector3[] v){
		
		if ( v.Length != 8 ){
			Debug.Log("Möp. Möp!");
			return;
		}
		
		setVertices(v[0], v[1], v[2], v[3], v[4], v[5], v[6], v[7] );
	}
	
	// <summary>
	// Sets all 24 vertices of an Unity-Cube by placing all eight different.
	// real vertices at their corresponding positions in the unity-vertex-array.
	// </summary>
	// <param name="BNE">Back, North-East Vertex</param>
	// <param name="BNW">Back, North-West Vertex</param>
	// <param name="BSE">Back, South-East Vertex</param>
	// <param name="BSW">Back, South-West Vertex</param>
	// <param name="FNE">Front, North-East Vertex</param>
	// <param name="FNW">Front, North-West Vertex</param>
	// <param name="FSE">Front, Sotuh-East Vertex</param>
	// <param name="FSW">Front, South-West Vertex</param>
	public void setVertices(Vector3 BNE, Vector3 BNW, Vector3 BSE, Vector3 BSW,
							Vector3 FNE, Vector3 FNW, Vector3 FSE, Vector3 FSW){
				
		Vector3[] verts = new Vector3[]{
			
		
			/* #0 */ BSE,
			/* #1 */ BSW,
			/* #2 */ BNE,
			/* #3 */ BNW,
			
			/* #4 */ FNE,
			/* #5 */ FNW,
			/* #6 */ FSE,
			/* #7 */ FSW,
			
			/* #8 */ BNE,
			/* #9 */ BNW,
			/* #10 */ FNE,
			/* #11 */ FNW,
			
			/* #12 */ FSE,
			/* #13 */ BSW,
			/* #14 */ FSW,
			/* #15 */ BSE,
			
			/* #16 */ BSW,
			/* #17 */ FNW,
			/* #18 */ FSW,
			/* #19 */ BNW,
			
			/* #20 */ FSE,
			/* #21 */ BNE,
			/* #22 */ BSE,
			/* #23 */ FNE,
			
		};
			
		//Debug.Log("set em!");
		cubeMesh.vertices = verts;
	}
}
