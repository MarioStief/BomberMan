using UnityEngine;

namespace AssemblyCSharp
{
	public class PowerupTexture  : MonoBehaviour {
		
		public Material bombUp;
		public Material bombDown;
		public Material flameUp;
		public Material flameDown;
		public Material playerSpeedUp;
		public Material playerSpeedDown;
		public Material delaySpeedUp;
		public Material delaySpeedDown;
		public Material triggerBomb;
		public Material contactMine;
		public Material goldenFlame;
		public Material superBomb;
		
		public void setType (PowerupType type) {
			Material material = null;
			if	(type == PowerupType.BOMB_UP) {
				material = bombUp;
			} else if (type == PowerupType.BOMB_DOWN) {
				material = bombDown;
			} else if (type == PowerupType.FLAME_UP) {
				material = flameUp;
			} else if (type == PowerupType.FLAME_DOWN) {
				material = flameDown;
			} else if (type == PowerupType.PLAYER_SPEED_UP) {
				material = playerSpeedUp;
			} else if (type == PowerupType.PLAYER_SPEED_DOWN) {
				material = playerSpeedDown;
			} else if (type == PowerupType.DELAY_SPEED_UP) {
				material = delaySpeedUp;
			} else if (type == PowerupType.DELAY_SPEED_DOWN) {
				material = delaySpeedDown;
			} else if (type == PowerupType.TRIGGERBOMB) {
				material = triggerBomb;
			} else if (type == PowerupType.CONTACTMINE) {
				material = contactMine;
			} else if (type == PowerupType.GOLDEN_FLAME) {
				material = goldenFlame;
			} else if (type == PowerupType.SUPERBOMB) {
				material = superBomb;
			}
			Material[] boxMaterials = new Material[1];
			boxMaterials[0] = material;
			renderer.materials = boxMaterials;
		}
	
		void Update() {
			transform.RotateAround(Vector3.zero, transform.position, 20 * Time.deltaTime);
		}
	}
}