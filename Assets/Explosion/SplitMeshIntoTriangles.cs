using UnityEngine;
using System.Collections;

public class SplitMeshIntoTriangles : MonoBehaviour
{
	
	private static GameObject guiObject;
	private GameObject gObject;
	private Vector3 position;
	private int simplify = 3;
	private Vector3 scale;

	
	public static GameObject GUIObject {
		get {
			if (guiObject == null) {
				guiObject = new GameObject("Explosion");
			}
			return guiObject;
		}
	}
	
	// Factory-Klasse, um einen Konstruktor auf einem Monobehaviour-Objekt zu emulieren
	public static SplitMeshIntoTriangles createMeshExplosion(GameObject obj, Vector3 position, int simplify) {
		SplitMeshIntoTriangles thisObj = GUIObject.AddComponent<SplitMeshIntoTriangles>();
		thisObj.gObject = obj;
		thisObj.position = position;
		thisObj.simplify = 11-simplify;
		//thisObj.child = child;
		thisObj.scale = obj.transform.localScale;
		return thisObj;
	}
	
    IEnumerator SplitMesh ()
    {
		if (gObject.transform.Find("child") != null) {
			gObject = gameObject.transform.Find("child").gameObject;
			if (gObject.transform.Find("MineBody") != null)
				gObject = gameObject.transform.Find("MineBody").gameObject;
			if (gObject.transform.Find("default") != null)
				gObject = gameObject.transform.Find("default").gameObject;
		}
        MeshFilter MF = gObject.GetComponent<MeshFilter>();
        MeshRenderer MR = gObject.GetComponent<MeshRenderer>();
		
        Mesh M = MF.mesh;
        Vector3[] verts = M.vertices;
        Vector3[] normals = M.normals;
        Vector2[] uvs = M.uv;
        for (int submesh = 0; submesh < M.subMeshCount; submesh++)
        {
            int[] indices = M.GetTriangles(submesh);
            for (int i = 0; i < indices.Length; i += 3*simplify)
            {
                Vector3[] newVerts = new Vector3[3];
                Vector3[] newNormals = new Vector3[3];
                Vector2[] newUvs = new Vector2[3];
                for (int n = 0; n < 3; n++)
                {
                    int index = indices[i + n];
                    newVerts[n] = verts[index];
                    newUvs[n] = uvs[index];
                    newNormals[n] = normals[index];
                }
                Mesh mesh = new Mesh();
                mesh.vertices = newVerts;
                mesh.normals = newNormals;
                mesh.uv = newUvs;
 
                mesh.triangles = new int[] { 0, 1, 2, 2, 1, 0 };
 
                GameObject GO = new GameObject("Triangle " + (i / 3));
                GO.transform.position = gObject.transform.position;
                GO.transform.rotation = gObject.transform.rotation;
                GO.AddComponent<MeshRenderer>().material = MR.materials[submesh];
                GO.AddComponent<MeshFilter>().mesh = mesh;
                GO.AddComponent<BoxCollider>();
				GO.transform.localScale = scale * simplify;
                GO.AddComponent<Rigidbody>().AddExplosionForce(200f, position, 100f, 0f, ForceMode.Force);
 
                Destroy(GO, 5 + Random.Range(0.0f, 5.0f));
            }
        }
        MR.enabled = false;
        //Time.timeScale = 0.2f;
        yield return new WaitForSeconds(0.8f);
        //Time.timeScale = 1.0f;
        Destroy(this);
    }
    void Start()
    {
		//Debug.Log (gameObject.ToString() + " has been destroyed.");
        StartCoroutine(SplitMesh());
    }
}