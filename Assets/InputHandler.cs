using UnityEngine;
using System.Collections;

// <summary>
// InputHandler nimmt jeglichen relevanten Input entgegen und verarbeitet diesen
// bzw. leiten ihn an verarbeitende Klassen weiter.
// </summary>

public class InputHandler : MonoBehaviour {
	
	public GameObject sphere;			
	private SphereBuilder sphereHandler;
	
	public GameObject camera;
	private Quaternion cameraRotation;
	
	// Use this for initialization
	void Start () {
	
		sphereHandler = sphere.GetComponent<SphereBuilder>();
		cameraRotation = camera.transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
	
		
		// Lese Bewegungsrichtung aus und lasse die Kugel entsprechend bewegen.
		float verticalMovement = Input.GetAxis("Vertical");
		sphereHandler.move(verticalMovement);
		
		float horizontalMovement = Input.GetAxis("Horizontal");
		if ( horizontalMovement != 0)
			moveAlongEquator( horizontalMovement/(-2)*Time.deltaTime);
	}
	
	private void moveAlongEquator(float movement){
	
		transform.position = new Vector3(Mathf.Cos(movement)* transform.position.x - Mathf.Sin(movement) * transform.position.y,
										Mathf.Sin(movement) * transform.position.x + Mathf.Cos(movement) * transform.position.y,
										transform.position.z);
		
		camera.transform.position = new Vector3(Mathf.Cos(movement)* camera.transform.position.x - Mathf.Sin(movement) * camera.transform.position.y,
										Mathf.Sin(movement) * camera.transform.position.x + Mathf.Cos(movement) * camera.transform.position.y,
										camera.transform.position.z);
		
		camera.transform.rotation.SetLookRotation(Vector3.zero);
	}
}
