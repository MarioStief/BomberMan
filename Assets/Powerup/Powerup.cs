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
	}
}

