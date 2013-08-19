using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace AssemblyCSharp
{
	public class Powerup
	{
		PowerupType type;
		int value = 0;
		
		public Powerup(PowerupType type)
		{
			this.type = type;
			if	(type == PowerupType.BOMB_DOWN)
					value = 1;
			else if (type == PowerupType.FLAME_DOWN)
					value = 1;
			else if (type == PowerupType.PLAYER_SPEED_DOWN)
					value = 1;
			else if (type == PowerupType.DELAY_SPEED_DOWN)
					value = 1;
			else if	(type == PowerupType.BOMB_UP)
					value = 3;
			else if (type == PowerupType.FLAME_UP)
					value = 3;
			else if (type == PowerupType.PLAYER_SPEED_UP)
					value = 3; 
			else if (type == PowerupType.DELAY_SPEED_UP)
					value = 3; 
			else if (type == PowerupType.TRIGGERBOMB)
					value = 5;
			else if (type == PowerupType.CONTACTMINE)
					value = 5;
			else if (type == PowerupType.GOLDEN_FLAME)
					value = 10;
			else if (type == PowerupType.SUPERBOMB)
					value = 10;
<<<<<<< HEAD
||||||| merged common ancestors
			else if (type == PowerupType.TRIGGERBOMB)
					value = 10;
=======
		}
		
		public PowerupType getType()
		{
			return type;
		}
		
		public int getValue() {
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
			return value;
		}
		
		public AudioClip getAudioClip() {
			AudioClip clip = null;
			switch (value) {
			case 1:
				clip = Static.powerdownSoundEffect;
				break;
			case 3:
				clip = Static.powerupSoundEffect;
				break;
			case 5:
				clip = Static.extraPowerupSoundEffect;
				break;
			case 10:
				clip = Static.superPowerupSoundEffect;
				break;
			}
			return clip;
		}
	}
}

