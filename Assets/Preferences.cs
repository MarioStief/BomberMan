using System;
using UnityEngine;

public static class Preferences
{
	private static bool destroyablePowerups = true;
	private static bool explodingPowerups = true;
	private static bool negativePowerups = true;
	private static int explosionDetail = 3; // 1 = very simple ... 10 = ultimate
	private static int chestDensity = 30; // 10 = low ... 40 = high

	
	static Preferences() {
		destroyablePowerups = (PlayerPrefs.GetInt("Destroyable Powerups",(destroyablePowerups ? 1 : 0)) == 1 ? true : false);
		explodingPowerups = (PlayerPrefs.GetInt("Exploding Powerups",(explodingPowerups ? 1 : 0)) == 1 ? true : false);
		negativePowerups = (PlayerPrefs.GetInt("Negative Powerups",(negativePowerups ? 1 : 0)) == 1 ? true : false);
		explosionDetail = (PlayerPrefs.GetInt("Explosion Detail",explosionDetail));
		chestDensity = (PlayerPrefs.GetInt("Chest Density",chestDensity));
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
}