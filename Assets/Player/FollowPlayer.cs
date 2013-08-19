using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class FollowPlayer : MonoBehaviour {
	
	public Transform player;
	

	
	// Update is called once per frame
	void Update () {
	

		transform.position = new Vector3(player.position.x + 3, transform.position.y, player.position.z);

		
		if (Input.GetKeyDown(KeyCode.LeftControl)){
			transform.position = new Vector3(transform.position.x, transform.position.y + 10, transform.position.z);
		} 
		
		if (Input.GetKeyUp(KeyCode.LeftControl)){
			transform.position = new Vector3(transform.position.x, transform.position.y - 10, transform.position.z);
		}
	}
}
