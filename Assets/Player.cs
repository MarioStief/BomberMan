using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public static class Player
	{
		private const float MAXSPEED = 4.0f;
		private const int MAXFLAMEPOWER = 10;
		
		private static int bombs = 1;
		private static int bombsActive = 0;
		private static int flamePower = 1;
		private static float speed = 1.0f;
		
		private static bool dead = false;
		
		public static void powerupCollected(PowerupType type)
		{
			Debug.Log (type == PowerupType.GOLDEN_FLAME);
			if (type == PowerupType.BOMB_UP) {
				bombs++;
			} else if (type == PowerupType.FLAME_UP) {
				if (flamePower < MAXFLAMEPOWER)
					flamePower++;
			} else if (type == PowerupType.GOLDEN_FLAME) {
					flamePower = MAXFLAMEPOWER;
			} else if (type == PowerupType.PLAYER_SPEED_UP) {
				if (speed < MAXSPEED)
					speed += 0.5f;
			}
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

