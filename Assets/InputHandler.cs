using UnityEngine;
using System.Collections;

// <summary>
// InputHandler nimmt jeglichen relevanten Input entgegen und verarbeitet diesen
// bzw. leiten ihn an verarbeitende Klassen weiter.
// </summary>

public class InputHandler : MonoBehaviour {
	
	public GameObject sphere;			
	private SphereBuilder sphereHandler;
	
	// Use this for initialization
	void Start () {
	
		sphereHandler = sphere.GetComponent<SphereBuilder>();
	}
	
	// Update is called once per frame
	void Update () {
	
		
		// Lese Bewegungsrichtung aus und lasse die Kugel entsprechend bewegen.
		Vector2 moveDirection = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
		sphereHandler.move(moveDirection);
		
	}
}
