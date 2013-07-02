using UnityEngine;

namespace AssemblyCSharp
{
	public class PowerupTexture  : MonoBehaviour {
		
		public Material powerdownBox;
		public Material powerupBox;
		public Material superPowerupBox;

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
				boxMaterial = powerupBox;
				powerupTexture = bomb;
			} else if (type == PowerupType.BOMB_DOWN) {
				boxMaterial = powerdownBox;
				powerupTexture = bomb;
			} else if (type == PowerupType.FLAME_UP) {
				boxMaterial = powerupBox;
				powerupTexture = flame;
			} else if (type == PowerupType.FLAME_DOWN) {
				boxMaterial = powerdownBox;
				powerupTexture = flame;
			} else if (type == PowerupType.PLAYER_SPEED_UP) {
				boxMaterial = powerupBox;
				powerupTexture = playerSpeed;
			} else if (type == PowerupType.PLAYER_SPEED_DOWN) {
				boxMaterial = powerdownBox;
				powerupTexture = playerSpeed;
			} else if (type == PowerupType.DELAY_SPEED_UP) {
				boxMaterial = powerupBox;
				powerupTexture = delaySpeed;
			} else if (type == PowerupType.DELAY_SPEED_DOWN) {
				boxMaterial = powerdownBox;
				powerupTexture = delaySpeed;
			} else if (type == PowerupType.GOLDEN_FLAME) {
				boxMaterial = superPowerupBox;
				powerupTexture = goldenFlame;
			} else if (type == PowerupType.SUPERBOMB) {
				boxMaterial = superPowerupBox;
				powerupTexture = superBomb;
			} else if (type == PowerupType.TRIGGERBOMB) {
				boxMaterial = superPowerupBox;
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