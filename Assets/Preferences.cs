using System;
using UnityEngine;

public static class Preferences
{
	private static bool destroyablePowerups = true;
	private static bool negativePowerups = true;
	
	static Preferences() {
		destroyablePowerups = (PlayerPrefs.GetInt("Destroyable Powerups",1) == 1 ? true : false);
		negativePowerups = (PlayerPrefs.GetInt("Negative Powerups",1) == 1 ? true : false);
	}
	
	public static bool getDestroyablePowerups() {
		return destroyablePowerups;
	}

	public static void setDestroyablePowerups(bool destroyable) {
		destroyablePowerups = destroyable;
		PlayerPrefs.SetInt("Destroyable Powerups", (destroyable == true ? 1 : 0));
	}

	public static bool getNegativePowerups() {
		return negativePowerups;
	}

	public static void setNegative(bool negative) {
		negativePowerups = negative;
		PlayerPrefs.SetInt("Negative Powerups", (negative == true ? 1 : 0));
	}
}