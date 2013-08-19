using UnityEngine;

namespace AssemblyCSharp
{
	public class PowerupTexture  : MonoBehaviour {
		
<<<<<<< HEAD
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
||||||| merged common ancestors
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
=======
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
		
		float t = 0.0f;
		Vector3 diff = Vector3.zero;
		GameObject powerup;
		
		void Start() {
			//powerup = this.transform.Find("powerup").gameObject;
		}
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
		
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
			float deltaTime = Time.deltaTime;
			t += deltaTime;
			transform.RotateAround(Vector3.zero, transform.position, 20 * deltaTime);
			transform.localPosition = new Vector3(0f, 0.12f + Mathf.Sin(t)/12, 0f);
		}
	}
}