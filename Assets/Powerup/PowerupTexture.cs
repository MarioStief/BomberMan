using UnityEngine;

namespace AssemblyCSharp
{
	public class PowerupTexture  : MonoBehaviour {
		
		public Texture bomb;
		public Texture flame;
		public Texture playerSpeed;
		public Texture delaySpeed;
		public Texture goldenFlame;
		public Texture superBomb;
		public Texture triggerBomb;
		
		public void setType (PowerupType type) {
			Texture texture = null;
			if	(type == PowerupType.BOMB_UP)
				texture = bomb;
			else if	(type == PowerupType.BOMB_DOWN)
				texture = bomb;
			else if (type == PowerupType.FLAME_UP)
				texture = flame;
			else if (type == PowerupType.FLAME_DOWN)
				texture = flame;
			else if (type == PowerupType.PLAYER_SPEED_UP)
				texture = playerSpeed;
			else if (type == PowerupType.PLAYER_SPEED_DOWN)
				texture = playerSpeed;
			else if (type == PowerupType.DELAY_SPEED_UP)
				texture = delaySpeed;
			else if (type == PowerupType.DELAY_SPEED_DOWN)
				texture = delaySpeed;
			else if (type == PowerupType.GOLDEN_FLAME)
				texture = goldenFlame;
			else if (type == PowerupType.SUPERBOMB)
				texture = superBomb;
			else if (type == PowerupType.TRIGGERBOMB)
				texture = triggerBomb;
			renderer.material.mainTexture = texture;
		}
	
		void Start() {
			renderer.material.shader = Shader.Find("Particles/Alpha Blended");
		}
		
		void Update() {
			//renderer.material.mainTexture = bomb;
			transform.RotateAround(Vector3.zero, transform.position, 20 * Time.deltaTime);
		}
	}
}