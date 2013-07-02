using UnityEngine;

namespace AssemblyCSharp
{
	public class PowerupTexture  : MonoBehaviour {
		
		public Material powerdown;
		public Material powerup;
		public Material superPowerup;

		public Texture bomb;
		public Texture flame;
		public Texture playerSpeed;
		public Texture delaySpeed;
		public Texture goldenFlame;
		public Texture superBomb;
		public Texture triggerBomb;
		
		public void setType (PowerupType type) {
			Material boxMaterial = null;
			Texture powerupTexture = null;
			if	(type == PowerupType.BOMB_UP) {
				boxMaterial = powerup;
				powerupTexture = bomb;
			} else if (type == PowerupType.BOMB_DOWN) {
				boxMaterial = powerdown;
				powerupTexture = bomb;
			} else if (type == PowerupType.FLAME_UP) {
				boxMaterial = powerup;
				powerupTexture = flame;
			} else if (type == PowerupType.FLAME_DOWN) {
				boxMaterial = powerdown;
				powerupTexture = flame;
			} else if (type == PowerupType.PLAYER_SPEED_UP) {
				boxMaterial = powerup;
				powerupTexture = playerSpeed;
			} else if (type == PowerupType.PLAYER_SPEED_DOWN) {
				boxMaterial = powerdown;
				powerupTexture = playerSpeed;
			} else if (type == PowerupType.DELAY_SPEED_UP) {
				boxMaterial = powerup;
				powerupTexture = delaySpeed;
			} else if (type == PowerupType.DELAY_SPEED_DOWN) {
				boxMaterial = powerdown;
				powerupTexture = delaySpeed;
			} else if (type == PowerupType.GOLDEN_FLAME) {
				boxMaterial = superPowerup;
				powerupTexture = goldenFlame;
			} else if (type == PowerupType.SUPERBOMB) {
				boxMaterial = superPowerup;
				powerupTexture = superBomb;
			} else if (type == PowerupType.TRIGGERBOMB) {
				boxMaterial = superPowerup;
				powerupTexture = triggerBomb;
			}
			Material[] boxMaterials = new Material[2];
			boxMaterials[0] = boxMaterial;
			boxMaterials[1] = new Material(Static.alphaBlended);
			boxMaterials[1].mainTexture = powerupTexture;
			renderer.materials = boxMaterials;
		}
	
		void Update() {
			transform.RotateAround(Vector3.zero, transform.position, 20 * Time.deltaTime);
		}
	}
}