using System;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	public static class Player
	{
		private static Parcel currentCell;	// current Parcel
		private static float xpos, zpos;	// Player's position in the current Parcel
		
		private static float verticalHelper		= 0.0f;
		private static float horizontalHelper 	= 0.0f;
		
		private const float MAXSPEED = 4.0f;
		private const int MAXFLAMEPOWER = 10;
		private const int MAXHP = 100;
		
		private static int bombs = 10;
		private static int bombsActive = 0;
		private static int flamePower = 10;
		private static float speed = 0.4f;
		private static int hp = MAXHP;
		
		private static bool dead = false;
		
		static Player() {
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
					speed += 0.1f;
			} else if (type == PowerupType.PLAYER_SPEED_DOWN) {
				if (speed > 1.0f)
					speed -= 0.1f;
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
			if (d) {
				// Verteile Powerups über das Spielfeld
				List<Parcel> parcelPool = new List<Parcel>();
				Parcel[][] gameArea = Static.gameArea;
				for (int i = 0; i < gameArea.Length; i++) {
					for (int j = 0; j < gameArea[i].Length; j++) {
						if (gameArea[i][j].getType() == 0) {
							parcelPool.Add(gameArea[i][j]);
						}
					}
				}
				parcelPool = shuffleList(parcelPool);
				if (flamePower == MAXFLAMEPOWER && parcelPool.Count > 0) {
					parcelPool[0].addPowerup(new Powerup(PowerupType.GOLDEN_FLAME), Static.goldenFlamePowerupPrefab);
					parcelPool.RemoveAt(0);
					bombs = 1;
				}
				while (bombs > 1 && parcelPool.Count > 0) {
					parcelPool[0].addPowerup(new Powerup(PowerupType.BOMB_UP), Static.bombUpPowerupPrefab);
					parcelPool.RemoveAt(0);
					bombs--;
				}
				while (flamePower > 1 && parcelPool.Count > 0) {
					parcelPool[0].addPowerup(new Powerup(PowerupType.FLAME_UP), Static.flameUpPowerupPrefab);
					parcelPool.RemoveAt(0);
					flamePower--;
				}
				while (speed > 0.4f && parcelPool.Count > 0) {
					parcelPool[0].addPowerup(new Powerup(PowerupType.PLAYER_SPEED_UP), Static.playerSpeedUpPowerupPrefab);
					parcelPool.RemoveAt(0);
					speed -= 0.1f;
				}
			}
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
			
			//currentCell.setColor(Color.cyan);
			currentCell.hightlightColor(true);
		}
		
		public static Parcel getCurrentParcel(){
			return currentCell;	
		}
		
		private static List<Parcel> shuffleList(List<Parcel> sortedList)
		{
			List<Parcel> randomList = new List<Parcel>();
			
			System.Random r = new System.Random();
		    int randomIndex = 0;
		    while (sortedList.Count > 0)
		    {
		    	randomIndex = r.Next(0, sortedList.Count);
		    	randomList.Add(sortedList[randomIndex]);
				sortedList.RemoveAt(randomIndex);
			}
			return randomList;
		}
	}
}

