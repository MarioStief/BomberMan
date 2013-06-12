using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class PlayerMoveScript : MonoBehaviour {
	
    public float gravity = 1.0F;
    private Vector3 moveDirection = Vector3.zero;
	
	private Cell currCell;
	
	// Update is called once per frame
	void Update () {
	
		if (!Data.initialized) return;
		
        if (Data.controller.isGrounded && !Player.isDead()) {
            moveDirection = new Vector3((-1)*Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal"));
            moveDirection *= Player.getSpeed();     
			
			currCell = Data.area.getCell(Data.controller.transform.position.x, Data.controller.transform.position.z);
			if (currCell.isExploding()){
				renderer.material.color = Color.black;
				moveDirection = new Vector3(0, 0, 0);
				Player.setDead(true);
				GameObject deadPlayer = GameObject.Instantiate(Data.deadPlayerPrefab, new Vector3(currCell.getXPos() + 0.5f, 0.3f, currCell.getZPos() + 0.5f), Quaternion.identity) as GameObject; 
			}
			
			if (currCell.hasPowerup()) {
				Player.powerupCollected(currCell.destroyPowerup());
			}
			
			if ( Input.GetKeyDown(KeyCode.Space)){ // Lege Bombe -- erstelle Partikelwolke
					
				if ( !currCell.hasBomb()) {
					if (Player.addBomb()) {
						GameObject explosion = new GameObject("explosion");
						explosion.AddComponent<Explosion>();
					}
				}
			}
        }
    
		moveDirection.y -= gravity * Time.deltaTime;
        Data.controller.Move(moveDirection * Time.deltaTime);
		
	}
}