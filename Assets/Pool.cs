using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace AssemblyCSharp
{
	public static class Pool
	{
		private static List<Powerup> pool = new List<Powerup>();
		
		public static void createPool(int size)
		{
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < 3; j++)
					pool.Add(new Powerup(PowerupType.BombUp));
				for (int j = 0; j < 3; j++)
					pool.Add(new Powerup(PowerupType.FlameUp));
				for (int j = 0; j < 3; j++)
					pool.Add(new Powerup(PowerupType.PlayerSpeedUp));
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
		
		public static void setPowerup(int xpos, int zpos)
		{
			Powerup p = pool[0];
			PowerupType type = p.getType();

			// DEBUG
			Debug.Log (p.getType().ToString());


			pool.RemoveAt(0);
			if (pool.Count == 0)
				createPool(1);
			
			GameObject powerup = null;
			if (p.getType() == PowerupType.BombUp)
					powerup = GameObject.Instantiate(Data.bombUpPowerupPreftab, new Vector3(xpos + 0.5f, 0.3f, zpos + 0.5f), Quaternion.identity) as GameObject; 
			else if (p.getType() == PowerupType.FlameUp)
					powerup = GameObject.Instantiate(Data.flameUpPowerupPrefab, new Vector3(xpos + 0.5f, 0.3f, zpos + 0.5f), Quaternion.identity) as GameObject; 
			else if (p.getType() == PowerupType.PlayerSpeedUp)
					powerup = GameObject.Instantiate(Data.playerSpeedUpPowerupPrefab, new Vector3(xpos + 0.5f, 0.3f, zpos + 0.5f), Quaternion.identity) as GameObject; 
			
			Cell currCell = Data.area.getCell(xpos, zpos);
			currCell.addPowerup(powerup, type);
			

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
	}
}

