using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public static class Preferences
	{
		private static bool destroyablePowerups = true;
		private static bool explodingPowerups = true;
		private static bool negativePowerups = true;
		private static int explosionDetail = 1; // 0 = off, 1 = low ... 3 = high
		private static int chestDensity = 5; // 1 = low ... 5 = high
		private static bool backgroundMusic = true;
		private static int mouseSensitivity = 5; // 0f - 1f
		private static int roundsToWin = 3;
	
		public static void load() {
			destroyablePowerups = (PlayerPrefs.GetInt("Destroyable Powerups",(destroyablePowerups ? 1 : 0)) == 1 ? true : false);
			explodingPowerups = (PlayerPrefs.GetInt("Exploding Powerups",(explodingPowerups ? 1 : 0)) == 1 ? true : false);
			negativePowerups = (PlayerPrefs.GetInt("Negative Powerups",(negativePowerups ? 1 : 0)) == 1 ? true : false);
			explosionDetail = PlayerPrefs.GetInt("Explosion Detail", explosionDetail);
			chestDensity = PlayerPrefs.GetInt("Chest Density", chestDensity);
			mouseSensitivity = PlayerPrefs.GetInt("Mouse Sensitivity", mouseSensitivity);
			roundsToWin = PlayerPrefs.GetInt("Rounds TO Win", roundsToWin);
			backgroundMusic = (PlayerPrefs.GetInt("Background Music",(backgroundMusic ? 1 : 0)) == 1 ? true : false);
		}
		
		public static bool getDestroyablePowerups() {
			return destroyablePowerups;
		}
		
		public static void setDestroyablePowerups(bool destroyable) {
			destroyablePowerups = destroyable;
			PlayerPrefs.SetInt("Destroyable Powerups", (destroyable == true ? 1 : 0));
		}
	
		public static bool getExplodingPowerups() {
			return explodingPowerups;
		}
		
		public static void setExplodingPowerups(bool exploding) {
			explodingPowerups = exploding;
			PlayerPrefs.SetInt("Exploding Powerups", (exploding == true ? 1 : 0));
		}
	
		public static bool getNegativePowerups() {
			return negativePowerups;
		}
		
		public static void setNegative(bool negative) {
			negativePowerups = negative;
			PlayerPrefs.SetInt("Negative Powerups", (negative == true ? 1 : 0));
		}
		
		public static int getExplosionDetail() {
			return explosionDetail;
		}
		
		public static void setExplosionDetail(int explosion) {
			if (explosion != explosionDetail) {
				explosionDetail = explosion;
				PlayerPrefs.SetInt("Explosion Detail", explosionDetail);
			}
		}
		
		public static int getChestDensity() {
			return chestDensity;
		}
		
		public static void setChestDensity(int chest) {
			if (chest != chestDensity) {
				chestDensity = chest;
				PlayerPrefs.SetInt("Chest Density", chestDensity);
			}
		}
		
		public static int getMouseSensitivity() {
			return mouseSensitivity;
		}
		
		public static void setMouseSensitivity(int md) {
			if (md != mouseSensitivity) {
				mouseSensitivity = md;
				PlayerPrefs.SetInt("Mouse Sensitivity", mouseSensitivity);
			}
		}
		
		public static int getRoundsToWin() {
			return roundsToWin;
		}
		
		public static void setRoundsToWin(int r) {
			if (r!= roundsToWin) {
				roundsToWin = r;
				PlayerPrefs.SetInt("Rounds To Win", roundsToWin);
			}
		}
		
		public static bool getBackgroundMusic() {
			return backgroundMusic;
		}

		public static void setBackgroundMusic(bool b) {
			if (b!= backgroundMusic) {
				backgroundMusic = b;
				PlayerPrefs.SetInt("Background Music", (backgroundMusic == true ? 1 : 0));
			}
		}
	}
}