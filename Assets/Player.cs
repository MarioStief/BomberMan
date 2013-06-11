using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public static class Player
	{
		private const float MAXSPEED = 4.0f;
		
		private static int bombs = 1;
		private static int bombsActive = 0;
		private static int flamePower = 1;
		private static float speed = 1.0f;
		
		private static bool dead = false;
		
		public static void powerupCollected(PowerupType type)
		{
			if (type == PowerupType.BombUp)
				bombs++;
			else if (type == PowerupType.FlameUp)
				flamePower++;
			else if (type == PowerupType.PlayerSpeedUp)
				if (speed < MAXSPEED)
					speed += 0.5f;
			Debug.Log("bombs: " + bombs + ", flamePower: " + flamePower + ", speed: " + speed);
		}
		
		public static bool addBomb() {
			if (bombsActive < bombs) {
				bombsActive++;
				return true;
			} else {
				return false;
			}
		}
		
		public static void destroyBomb() {
			bombsActive--;
		}

		public static int getFlamePower() {
			return flamePower;
		}

		public static float getSpeed() {
			return speed;
		}
		
		public static void setDead(bool d) {
			dead = d;
		}
		
		public static bool isDead() {
			return dead;
		}
	}
}

