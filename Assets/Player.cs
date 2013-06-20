using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public static class Player
	{
		private const float MAXSPEED = 4.0f;
		private const int MAXFLAMEPOWER = 10;
		private const int MAXHP = 100;
		
		private static int bombs = 1;
		private static int bombsActive = 0;
		private static int flamePower = 1;
		private static float speed = 3;
		private static int hp = MAXHP;
		
		private static bool dead = false;
		
		public static void powerupCollected(PowerupType type)
		{
			Debug.Log (type == PowerupType.GOLDEN_FLAME);
			if (type == PowerupType.BOMB_UP) {
				bombs++;
			} else if (type == PowerupType.BOMB_DOWN) {
				if (bombs > 1)
					bombs--;
			} else if (type == PowerupType.FLAME_UP) {
				if (flamePower < MAXFLAMEPOWER)
					flamePower++;
			} else if (type == PowerupType.FLAME_DOWN) {
				if (flamePower > 1)
					flamePower--;
			} else if (type == PowerupType.GOLDEN_FLAME) {
					flamePower = MAXFLAMEPOWER;
			} else if (type == PowerupType.PLAYER_SPEED_UP) {
				if (speed < MAXSPEED)
					speed += 0.5f;
			} else if (type == PowerupType.PLAYER_SPEED_DOWN) {
				if (speed > 1.0f)
					speed -= 0.5f;
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
		
		public static void removeBomb() {
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
		
		public static void decreaseHP() {
			hp--;
			Debug.Log("Life: " + hp);
			if (hp == 0)
				dead = true;
		}

		public static void increaseHP() {
			if (hp < 100) {
				hp++;
				Debug.Log("Life: " + hp);
			}
		}
		
		public static int getHP() {
			return hp;
		}

		public static int getMaxHP() {
			return MAXHP;
		}
	}
}

