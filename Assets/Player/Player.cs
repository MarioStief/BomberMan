using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public static class Player
	{
		private static Parcel currentCell;	// current Parcel
		private static float xpos, zpos;	// Player's position in the current Parcel
		
		private static GameObject sphere; // TEMP
		public static SphereBuilder sphereHandler;
		
		private static float verticalHelper		= 0.0f;
		private static float horizontalHelper 	= 0.0f;
		
		private const float MAXSPEED = 4.0f;
		private const int MAXFLAMEPOWER = 10;
		private const int MAXHP = 100;
		
		private static int bombs = 1;
		private static int bombsActive = 0;
		private static int flamePower = 1;
		private static float speed = 0.4f;
		private static int hp = MAXHP;
		
		private static bool dead = false;
		
		static void Awake() {
			sphereHandler = sphere.GetComponent<SphereBuilder>();
		}
		
		public static void powerupCollected(PowerupType type)
		{
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
		
		public static void setXPos(float x){
			if ( x > 1) xpos = 1;
			xpos = x;	
		}
		
		public static float getXPos(){
			return xpos;	
		}
		
		public static void setZPos(float z){
			if ( zpos > 1) zpos = 1;
			zpos = z;	
		}
		
		public static float getZPos(){
			return zpos;	
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
		
		public static void setCurrentParcel(Parcel parcel){
			
			if ( currentCell != null){
				currentCell.hightlightColor(false);	
			}
			
			currentCell = parcel;	
			
			currentCell.hightlightColor(true);
		}
		
		public static Parcel getCurrentParcel(){
			return currentCell;	
		}
	}
}

