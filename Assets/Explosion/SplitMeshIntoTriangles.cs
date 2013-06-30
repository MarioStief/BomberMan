using UnityEngine;
using System.Collections;

public class SplitMeshIntoTriangles : MonoBehaviour
{
	
	private static GameObject guiObject;
	private GameObject gameObject;
	private Vector3 position;
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
	public static SplitMeshIntoTriangles createMeshExplosion(GameObject obj, Vector3 position) {
		SplitMeshIntoTriangles thisObj = GUIObject.AddComponent<SplitMeshIntoTriangles>();
		thisObj.gameObject = obj;
		thisObj.position = position;
		thisObj.scale = obj.transform.localScale;
		return thisObj;
	}
	
    IEnumerator SplitMesh ()
    {
        MeshFilter MF = gameObject.GetComponent<MeshFilter>();
        MeshRenderer MR = gameObject.GetComponent<MeshRenderer>();
        Mesh M = MF.mesh;
        Vector3[] verts = M.vertices;
        Vector3[] normals = M.normals;
        Vector2[] uvs = M.uv;
        for (int submesh = 0; submesh < M.subMeshCount; submesh++)
        {
            int[] indices = M.GetTriangles(submesh);
            for (int i = 0; i < indices.Length; i += 3)
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
                GO.transform.position = gameObject.transform.position;
                GO.transform.rotation = gameObject.transform.rotation;
                GO.AddComponent<MeshRenderer>().material = MR.materials[submesh];
                GO.AddComponent<MeshFilter>().mesh = mesh;
                GO.AddComponent<BoxCollider>();
				GO.transform.localScale = scale;
                GO.AddComponent<Rigidbody>().AddExplosionForce(200f, position, 0f, 3.0f, ForceMode.Force);
 
                Destroy(GO, 5 + Random.Range(0.0f, 5.0f));
            }
        }
        MR.enabled = false;
        //Time.timeScale = 0.2f;
        yield return new WaitForSeconds(0.8f);
        //Time.timeScale = 1.0f;
        Destroy(gameObject);
    }
    void Start()
    {
		Debug.Log (gameObject.ToString() + " has been destroyed.");
        StartCoroutine(SplitMesh());
    }
}