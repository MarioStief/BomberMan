using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class MeshManipulator : MonoBehaviour {
	
	Parcel meshParcel;
	
	SphereBuilder sphere;		// Der Spherebuilder, um ggf. eigene Ecken nachzufragen
	public Mesh cubeMesh;				// Der eigene Mesh
	
	public Vector3 []vertexPosition;	// Position der eigenen Punkte in 3-dimensionalem Array im Spherebuilder.
	
	float height;
	float lift = 0.0f;
	private int boxTexture;
	
	public Texture grassTex;
	public Texture rockTex;
	public Texture boxTex1;
	public Texture boxTex2;
	public Texture grassBump;
	public Texture rockBump;
	public Texture boxBump1;
	public Texture boxBump2;
	
	// Use this for initialization
	void Awake () {
				
		sphere = Static.sphereHandler;
		cubeMesh = GetComponent<MeshFilter>().mesh;
		vertexPosition = new Vector3[8];
		boxTexture = Random.Range(0, 2);
		
		//grassTex = Resources.Load("Textures\\grassPlane2.jpg") as Texture;
	}
	
	public void setParcel(Parcel p){
		meshParcel = p;	
	}
	
	public Object getBoxObject() {
		if (boxTexture == 0) {
			return Static.boxCube1Prefab;
		} else {
			return Static.boxCube2Prefab;
		}
	}
	
	
	public void updateTexture(){
		int type = meshParcel.getType();
		if ( type == 0) {
			renderer.material.mainTexture = grassTex;
			renderer.material.mainTextureScale = new Vector2(0.3f,0.3f);
			renderer.material.SetTexture("_BumpMap", grassBump);
		}
		else if ( type == 2){
			renderer.material.mainTexture = rockTex;
			renderer.material.SetTexture("_BumpMap", rockBump); 
		}
		else {
			if (boxTexture == 0) {
				renderer.material.mainTexture = boxTex1;
				renderer.material.SetTexture("_BumpMap", boxBump1); 
			} else {
				renderer.material.mainTexture = boxTex2;
				renderer.material.SetTexture("_BumpMap", boxBump2); 
			}

		}
	}
	
	public Vector3 getCenter(){
		return (cubeMesh.vertices[4] + 0.5f*height*(cubeMesh.vertices[9]-cubeMesh.vertices[4]))*(1.01f);
	}
	
	// <summary>
	// Setze Höhe auf h;
	// h ist prozentuale Höhe zur Ausgangshöhe.
	// Würfel zeichnet sich im Anschluss neu.
	// </summary>
	public void setHeight(float h){
		
		this.height = h;

		Vector3 []v = cubeMesh.vertices;
		for(int i = 0; i < v.Length; i++){
			v[i] *= h;
		}
				
		cubeMesh.vertices = v;
		cubeMesh.RecalculateNormals();
		cubeMesh.RecalculateBounds();
		cubeMesh.Optimize();
		
	}
	
	// <summary>
	// Übergabe der aller Eckpunkte des Würfels über die  _POSITION_ im 3-dim Vertex-Array
	// der Kugel
	// </summary>
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
	
	// <summary>
	// Schlägt aktuelle Eck-Koordinaten in Sphere-Klasse nach und ersetzt mit diesen seine eigenen.
	// Die Höhe des Würfels wird im Aktualisierungsschritt beachtet.
	// </summary>
	public void updateCoordinates(){
		if ( meshParcel != null) {
			height = meshParcel.getHeight();
			updateTexture();
		}
		
		if (height == 0)
			setVertices(sphere.sphereVertices[(int)vertexPosition[0].x][(int)vertexPosition[0].y][(int)vertexPosition[0].z],
						sphere.sphereVertices[(int)vertexPosition[1].x][(int)vertexPosition[1].y][(int)vertexPosition[1].z],
						sphere.sphereVertices[(int)vertexPosition[2].x][(int)vertexPosition[2].y][(int)vertexPosition[2].z],
						sphere.sphereVertices[(int)vertexPosition[3].x][(int)vertexPosition[3].y][(int)vertexPosition[3].z],
						sphere.sphereVertices[(int)vertexPosition[4].x][(int)vertexPosition[4].y][(int)vertexPosition[4].z],
						sphere.sphereVertices[(int)vertexPosition[5].x][(int)vertexPosition[5].y][(int)vertexPosition[5].z],
						sphere.sphereVertices[(int)vertexPosition[6].x][(int)vertexPosition[6].y][(int)vertexPosition[6].z],
						sphere.sphereVertices[(int)vertexPosition[7].x][(int)vertexPosition[7].y][(int)vertexPosition[7].z]
			);	
		else
			setVertices(sphere.sphereVertices[(int)vertexPosition[0].x][(int)vertexPosition[0].y][(int)vertexPosition[0].z]*height,
						sphere.sphereVertices[(int)vertexPosition[1].x][(int)vertexPosition[1].y][(int)vertexPosition[1].z]*height,
						sphere.sphereVertices[(int)vertexPosition[2].x][(int)vertexPosition[2].y][(int)vertexPosition[2].z]*height,
						sphere.sphereVertices[(int)vertexPosition[3].x][(int)vertexPosition[3].y][(int)vertexPosition[3].z]*height,
						sphere.sphereVertices[(int)vertexPosition[4].x][(int)vertexPosition[4].y][(int)vertexPosition[4].z]*height,
						sphere.sphereVertices[(int)vertexPosition[5].x][(int)vertexPosition[5].y][(int)vertexPosition[5].z]*height,
						sphere.sphereVertices[(int)vertexPosition[6].x][(int)vertexPosition[6].y][(int)vertexPosition[6].z]*height,
						sphere.sphereVertices[(int)vertexPosition[7].x][(int)vertexPosition[7].y][(int)vertexPosition[7].z]*height
			);
		
		//if ( meshParcel != null) meshParcel.setCenter((height*cubeMesh.vertices[4] + 0.5f*height*(cubeMesh.vertices[9]-cubeMesh.vertices[4]))*(1.01f));
		
		Vector3 center = (lift == 0.0f ? getCenter() : lift * getCenter());
		if ( meshParcel != null) meshParcel.setGameObjectPosition(center);
		if ( meshParcel != null) renderer.material.color = meshParcel.getColor();
	}
	
	public void liftObject(float factor) {
		lift = factor;
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
			
			// JA: Die Eckfolge beim Unity-Würfel ist echt so bescheuert! :-D
			
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
		cubeMesh.RecalculateNormals();
		cubeMesh.RecalculateBounds();
		cubeMesh.Optimize();
	}
}
