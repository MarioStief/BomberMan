using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace AssemblyCSharp
{
	public static class PowerupPool
	{
		public const int DROPCHANCE = 25; // Drop chance in %
		private static bool destroyable = Preferences.getDestroyablePowerups();
		private static bool negative = Preferences.getNegativePowerups();
		private static List<Powerup> pool = new List<Powerup>();
		
		static PowerupPool() {
			/*
			bombUpPowerupPreftab = GameObject.Find("BombUp");
			bombDownPowerupPreftab = bombUpPowerupPreftab;
			bombDownPowerupPreftab.GetComponent("RenderSettings").haloStrength = 0.5;
			((Behaviour)bombUpPowerupPreftab.GetComponent("Halo")).color = Color.red; // Does not work
			// IDEE: Ohne Halo starten und diesen langsam vergrößern.
			// So weiß man am Anfang noch nicht, ob es ein positives oder negatives Powerup ist.
			*/
		}
		
		public static void createPool(int size)
		{
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < 10; j++)
					pool.Add(new Powerup(PowerupType.BOMB_UP));
				for (int j = 0; j < 10; j++)
					pool.Add(new Powerup(PowerupType.FLAME_UP));
				for (int j = 0; j < 10; j++)
					pool.Add(new Powerup(PowerupType.PLAYER_SPEED_UP));
				for (int j = 0; j < 10; j++)
					pool.Add(new Powerup(PowerupType.DELAY_SPEED_UP));
				for (int j = 0; j < 1; j++)
					pool.Add(new Powerup(PowerupType.GOLDEN_FLAME));
				for (int j = 0; j < 1; j++)
					pool.Add(new Powerup(PowerupType.SUPERBOMB));
				for (int j = 0; j < 1; j++)
					pool.Add(new Powerup(PowerupType.TRIGGERBOMB));
				for (int j = 0; j < 1; j++)
					pool.Add(new Powerup(PowerupType.CONTACTMINE));
				if (negative) {
					for (int j = 0; j < 2; j++)
						pool.Add(new Powerup(PowerupType.BOMB_DOWN));
					for (int j = 0; j < 2; j++)
						pool.Add(new Powerup(PowerupType.FLAME_DOWN));
					for (int j = 0; j < 2; j++)
						pool.Add(new Powerup(PowerupType.PLAYER_SPEED_DOWN));
					for (int j = 0; j < 2; j++)
						pool.Add(new Powerup(PowerupType.DELAY_SPEED_DOWN));
				}
			}

			pool = shuffleList(pool);
			
			// DEBUG: show pool content
			poolContent();
		}
		
		private static List<Powerup> shuffleList(List<Powerup> sortedList)
		{
			List<Powerup> randomList = new List<Powerup>();
			
		    int randomIndex = 0;
		    while (sortedList.Count > 0)
		    {
		    	randomIndex = Random.Range(0, sortedList.Count);
		    	randomList.Add(sortedList[randomIndex]);
				sortedList.RemoveAt(randomIndex);
			}
			return randomList;
		}
		
		public static PowerupType setPowerup(Parcel cell)
		{
			Vector3 cellCenter = Vector3.zero; //cell.getCenterPoint();
			
			Powerup p = pool[0];
			PowerupType type = p.getType();

			// DEBUG
			Debug.Log (p.getType().ToString() + " spotted");


			pool.RemoveAt(0);
			if (pool.Count == 0)
				createPool(1);
			
			cell.addPowerup(new Powerup(type));
			
			/*
			UnityEngine.Object powerup = null;

			if	(p.getType() == PowerupType.BOMB_UP)
				powerup = Static.powerupPrefab;
			else if	(p.getType() == PowerupType.BOMB_DOWN)
					powerup = Static.powerdownPrefab; 
			else if (p.getType() == PowerupType.FLAME_UP)
					powerup = Static.powerupPrefab; 
			else if (p.getType() == PowerupType.FLAME_DOWN)
					powerup = Static.powerdownPrefab; 
			else if (p.getType() == PowerupType.PLAYER_SPEED_UP)
					powerup = Static.powerupPrefab; 
			else if (p.getType() == PowerupType.PLAYER_SPEED_DOWN)
					powerup = Static.powerdownPrefab; 
			else if (p.getType() == PowerupType.DELAY_SPEED_UP)
					powerup = Static.powerupPrefab; 
			else if (p.getType() == PowerupType.DELAY_SPEED_DOWN)
					powerup = Static.powerdownPrefab; 
			else if (p.getType() == PowerupType.GOLDEN_FLAME)
					powerup = Static.superPowerupPrefab;
			else if (p.getType() == PowerupType.SUPERBOMB)
					powerup = Static.superPowerupPrefab; 
			else if (p.getType() == PowerupType.TRIGGERBOMB)
					powerup = Static.superPowerupPrefab; 
					
			cell.addPowerup(new Powerup(type), powerup);
			*/

		return type;
		}
		
		private static void poolContent()
		{
			StringBuilder builder = new StringBuilder();
			foreach (Powerup item in pool)
			{
				string newString = item.getType().ToString();
			    builder.Append(newString).Append(" | ");
			}
			string result = "New Powerup Pool: " + builder.ToString();
			Debug.Log(result);
		}
		
		public static void setDestroyable(bool d) {
			destroyable = d;
		}
		
		public static bool getDestroyable() { 
			return destroyable;
		}

		public static void setNegative(bool n) {
			negative = n;
		}
		
		public static bool getNegative() {
			return negative;
		}
}
}

