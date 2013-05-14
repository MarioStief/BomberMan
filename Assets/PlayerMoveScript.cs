using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class PlayerMoveScript : MonoBehaviour {

	public float speed = 2.0F;
    public float gravity = 20.0F;
    private Vector3 moveDirection = Vector3.zero;
	
	// Update is called once per frame
	void Update () {
	
		CharacterController controller = GetComponent<CharacterController>();
        if (controller.isGrounded) {
            moveDirection = new Vector3((-1)*Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal"));
            moveDirection *= speed;     
			
			if ( Input.GetKeyDown(KeyCode.Space)){ // Lege Bombe -- erstelle Partikelwolke
				
				GameArea area = GameObject.Find("MainObject").GetComponent<InstGame>().area;
				if ( area != null){
					Cell currCell = area.getCell(controller.transform.position.x, controller.transform.position.z);
					
					if ( currCell != null){
						currCell.getGameObject().GetComponent<ParticleSystem>().Emit(1000);
					}
				}
			}
        }
    
		moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
		
	}
}
