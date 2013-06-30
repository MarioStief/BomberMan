using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace AssemblyCSharp
{
	public class Powerup
	{
		PowerupType type;
		
		public Powerup(PowerupType type)
		{
			this.type = type;
		}
		
		public PowerupType getType()
		{
			return type;
		}
		
		public int getValue() {
			int value = 0;
			if	(type == PowerupType.BOMB_DOWN)
					value = 1; 
			else if (type == PowerupType.FLAME_DOWN)
					value = 1; 
			else if (type == PowerupType.PLAYER_SPEED_DOWN)
					value = 1; 
			else if (type == PowerupType.PLAYER_SPEED_UP)
					value = 3; 
			else if	(type == PowerupType.BOMB_UP)
					value = 3; 
			else if (type == PowerupType.FLAME_UP)
					value = 3; 
			else if (type == PowerupType.GOLDEN_FLAME)
					value = 10;
			else if (type == PowerupType.SUPERBOMB)
					value = 10;
			return value;
		}
	}
}

