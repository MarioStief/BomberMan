using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class PlayerMoveScript : MonoBehaviour {

	public float speed = 2.0F;
    public float gravity = 1.0F;
    private Vector3 moveDirection = Vector3.zero;
	
	private Cell currCell;
	
	// Update is called once per frame
	void Update () {
	
		if ( !Data.initialized) return;
		
        if (Data.controller.isGrounded) {
            moveDirection = new Vector3((-1)*Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal"));
            moveDirection *= speed;     
			
			currCell = Data.area.getCell(Data.controller.transform.position.x, Data.controller.transform.position.z);
			if ( currCell.getKillOrder()){
				renderer.material.color = Color.black;		
			}
			
			if ( Input.GetKeyDown(KeyCode.Space)){ // Lege Bombe -- erstelle Partikelwolke
					
				//TODO: CHange to TryAddExplosion!
				if ( !currCell.hasBomb()){
					new Explosion(currCell.getXPos(), currCell.getZPos());
				}
			}
        }
    
		moveDirection.y -= gravity * Time.deltaTime;
        Data.controller.Move(moveDirection * Time.deltaTime);
		
	}
}
