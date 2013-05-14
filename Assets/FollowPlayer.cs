using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {
	
	public Transform player;
	
	private float dx, dz;
	
	// Use this for initialization
	void Start () {
	
		
		
	}
	
	// Update is called once per frame
	void Update () {
	
		if ( player != null){
			transform.position = new Vector3( player.position.x - dx, transform.position.y, player.position.z - dz);
		} else{
			
			while( player == null){
				player = GameObject.Find("Sphere").transform;
			}
			//Debug.Log(player == null);
			transform.position = new Vector3( player.position.x+5, transform.position.y, player.position.z);
			dx = player.position.x - transform.position.x;
			dz = player.position.z - transform.position.z;	
		}
		
		if ( Input.GetKeyDown(KeyCode.LeftControl)){
			transform.position = new Vector3( transform.position.x, transform.position.y + 10, transform.position.z);
		} 
		
		if ( Input.GetKeyUp(KeyCode.LeftControl)){
			transform.position = new Vector3( transform.position.x, transform.position.y - 10, transform.position.z);
		}
	}
}
