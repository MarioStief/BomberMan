using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace AssemblyCSharp
{
	public static class PowerupPool
	{
		private static bool destroyable = true;
		private static bool negative = true;
		private static GameObject sphere;
		private static SphereBuilder sphereHandler;
		private static List<Powerup> pool = new List<Powerup>();
		public static GameObject bombUpPowerupPreftab = GameObject.Find("BombUp");
		public static GameObject bombDownPowerupPreftab = GameObject.Find("BombDown");
		public static GameObject flameUpPowerupPrefab = GameObject.Find("FlameUp");
		public static GameObject flameDownPowerupPrefab = GameObject.Find("FlameDown");
		public static GameObject playerSpeedUpPowerupPrefab = GameObject.Find("PlayerSpeedUp");
		public static GameObject playerSpeedDownPowerupPrefab = GameObject.Find("PlayerSpeedDown");
		public static GameObject goldenFlamePowerupPrefab = GameObject.Find("GoldenFlame");
		
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
				for (int j = 0; j < 1; j++)
					pool.Add(new Powerup(PowerupType.GOLDEN_FLAME));
				if (negative) {
					for (int j = 0; j < 2; j++)
						pool.Add(new Powerup(PowerupType.BOMB_DOWN));
					for (int j = 0; j < 2; j++)
						pool.Add(new Powerup(PowerupType.FLAME_DOWN));
					for (int j = 0; j < 2; j++)
						pool.Add(new Powerup(PowerupType.PLAYER_SPEED_DOWN));
				}
			}
			pool = shuffleList(pool);
			
			// DEBUG: show pool content
			poolContent();
			
		}
		
		private static List<Powerup> shuffleList(List<Powerup> sortedList)
		{
			List<Powerup> randomList = new List<Powerup>();
			
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
		
		public static void setPowerup(int x, int y)
		{
			Powerup p = pool[0];
			PowerupType type = p.getType();

			// DEBUG
			Debug.Log (p.getType().ToString());


			pool.RemoveAt(0);
			if (pool.Count == 0)
				createPool(1);
			
			GameObject powerup = null;
			
				 if	(p.getType() == PowerupType.BOMB_UP)
					powerup = GameObject.Instantiate(bombUpPowerupPreftab, new Vector3(x + 0.5f, 0.3f, y + 0.5f), Quaternion.identity) as GameObject; 
			else if	(p.getType() == PowerupType.BOMB_DOWN)
					powerup = GameObject.Instantiate(bombDownPowerupPreftab, new Vector3(x + 0.5f, 0.3f, y + 0.5f), Quaternion.identity) as GameObject; 
			else if (p.getType() == PowerupType.FLAME_UP)
					powerup = GameObject.Instantiate(flameUpPowerupPrefab, new Vector3(x + 0.5f, 0.3f, y + 0.5f), Quaternion.identity) as GameObject; 
			else if (p.getType() == PowerupType.FLAME_DOWN)
					powerup = GameObject.Instantiate(flameDownPowerupPrefab, new Vector3(x + 0.5f, 0.3f, y + 0.5f), Quaternion.identity) as GameObject; 
			else if (p.getType() == PowerupType.PLAYER_SPEED_UP)
					powerup = GameObject.Instantiate(playerSpeedUpPowerupPrefab, new Vector3(x + 0.5f, 0.3f, y + 0.5f), Quaternion.identity) as GameObject; 
			else if (p.getType() == PowerupType.PLAYER_SPEED_DOWN)
					powerup = GameObject.Instantiate(playerSpeedDownPowerupPrefab, new Vector3(x + 0.5f, 0.3f, y + 0.5f), Quaternion.identity) as GameObject; 
			else if (p.getType() == PowerupType.GOLDEN_FLAME)
					powerup = GameObject.Instantiate(goldenFlamePowerupPrefab, new Vector3(x + 0.5f, 0.3f, y + 0.5f), Quaternion.identity) as GameObject; 
			
			Parcel cell = sphereHandler.getGameArea().getCurrentParcel(x, y);
			cell.addPowerup(powerup, type);
			

		}
		
		private static void poolContent()
		{
			StringBuilder builder = new StringBuilder();
			foreach (Powerup item in pool)
			{
				string newString = item.getType().ToString();
			    builder.Append(newString).Append(" | ");
			}
			string result = "New Pool: " + builder.ToString();
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

