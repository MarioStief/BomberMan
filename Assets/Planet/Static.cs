using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public static class Static
	{
		public static SphereBuilder sphereHandler;
		public static InputHandler inputHandler;
		public static Rink rink;
		public static Parcel[][] gameArea;
		public static MeshManipulator[][] drawnArea;
        public static Player player = new Player();
		
		public static UnityEngine.Object bombPrefab;
		public static UnityEngine.Object contactMinePrefab;
		public static UnityEngine.Object powerupPrefab;
		public static UnityEngine.Object explosionPrefab;
		
		public static UnityEngine.Object boxCube1Prefab;
		public static UnityEngine.Object boxCube2Prefab;
		public static UnityEngine.Object stoneCube1Prefab;
		public static UnityEngine.Object stoneCube2Prefab;
		
<<<<<<< HEAD
		public static Shader transparentDiffuseShader;
		
		public static UnityEngine.Object bombIconPrefab;
		public static UnityEngine.Object playerSpeedIconPrefab;
		public static UnityEngine.Object delaySpeedIconPrefab;
		
		// dynamically filled prefabs:
		public static UnityEngine.Object flameIconPrefab;
		public static UnityEngine.Object superBombIconPrefab;
		public static UnityEngine.Object extraIconPrefab;
		
		/*
		 * if needed: the static ones:
		 * public static UnityEngine.Object emptyIconPrefab;
		 * public static UnityEngine.Object triggerBombIconPrefab;
		 * public static UnityEngine.Object contactMineIconPrefab;
		 * public static UnityEngine.Object goldenFlameIconPrefab;
		 * public static UnityEngine.Object normalFlameIconPrefab;
		 * public static UnityEngine.Object superBombActiveIconPrefab;
		 * public static UnityEngine.Object superBombIconInactivePrefab;
		 */

||||||| merged common ancestors
=======
		public static Shader transparentDiffuseShader;
		
		public static UnityEngine.Object bombIconPrefab;
		public static UnityEngine.Object playerSpeedIconPrefab;
		public static UnityEngine.Object delaySpeedIconPrefab;
		
		// dynamically filled prefabs:
		public static UnityEngine.Object flameIconPrefab;
		public static UnityEngine.Object superBombIconPrefab;
		public static UnityEngine.Object extraIconPrefab;
		
		//Sound effects:
		public static AudioClip bombDropSoundEffect;
		public static AudioClip contactMineDropSoundEffect;
		public static AudioClip contactMineExplosionSoundEffect;
		public static AudioClip powerupSoundEffect;
		public static AudioClip powerdownSoundEffect;
		public static AudioClip superPowerupSoundEffect;
		public static AudioClip extraPowerupSoundEffect;
		public static AudioClip playerDeadSoundEffect;
		
		/*
		 * if needed: the static ones:
		 * public static UnityEngine.Object emptyIconPrefab;
		 * public static UnityEngine.Object triggerBombIconPrefab;
		 * public static UnityEngine.Object contactMineIconPrefab;
		 * public static UnityEngine.Object goldenFlameIconPrefab;
		 * public static UnityEngine.Object normalFlameIconPrefab;
		 * public static UnityEngine.Object superBombActiveIconPrefab;
		 * public static UnityEngine.Object superBombIconInactivePrefab;
		 */

>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
		static Static() {
			
			bombPrefab = Resources.Load("Prefabs/bombPrefab");
			contactMinePrefab = Resources.Load("Prefabs/contactMinePrefab");
			powerupPrefab = Resources.Load("Prefabs/powerupPrefab");
			explosionPrefab = Resources.Load("Prefabs/explosionPrefab");
			
			boxCube1Prefab = Resources.Load("Prefabs/boxCube1Prefab");
			boxCube2Prefab = Resources.Load("Prefabs/boxCube2Prefab");
			stoneCube1Prefab = Resources.Load("Prefabs/stoneCube1Prefab");
			stoneCube2Prefab = Resources.Load("Prefabs/stoneCube2Prefab");
<<<<<<< HEAD
			
			transparentDiffuseShader = Shader.Find("Transparent/Diffuse");
			
			delaySpeedIconPrefab = Resources.Load("Textures/Menu/delaySpeed");
			superBombIconPrefab = Resources.Load("Textures/Menu/superBombInactive");
			extraIconPrefab = Resources.Load("Textures/Menu/empty");
			
			//again: dynamic:
			bombIconPrefab = Resources.Load("Textures/Menu/bomb");
			playerSpeedIconPrefab = Resources.Load("Textures/Menu/playerSpeed");
			flameIconPrefab = Resources.Load("Textures/Menu/flame");

			/* static:
			emptyIconPrefab = Resources.Load("Textures/Menu/empty.png");
			triggerBombIconPrefab = Resources.Load("Textures/Menu/triggerBomb.png");
			contactMineIconPrefab = Resources.Load("Textures/Menu/contactMine.png");
			goldenFlameIconPrefab = Resources.Load("Textures/Menu/goldenFlame.png");
			normalFlameIconPrefab = Resources.Load("Textures/Menu/flame.png");
			superBombActiveIconPrefab = Resources.Load("Textures/Menu/superBombActive.png");
			superBombIconInactivePrefab = Resources.Load("Textures/Menu/superBombInactive.png");
			*/
||||||| merged common ancestors
			alphaBlended = Shader.Find("Particles/Alpha Blended");
=======
			
			transparentDiffuseShader = Shader.Find("Transparent/Diffuse");
			
			delaySpeedIconPrefab = Resources.Load("Textures/Menu/delaySpeed");
			superBombIconPrefab = Resources.Load("Textures/Menu/superBombInactive");
			extraIconPrefab = Resources.Load("Textures/Menu/empty");
			
			//again: dynamic:
			bombIconPrefab = Resources.Load("Textures/Menu/bomb");
			playerSpeedIconPrefab = Resources.Load("Textures/Menu/playerSpeed");
			flameIconPrefab = Resources.Load("Textures/Menu/flame");

			/* static:
			emptyIconPrefab = Resources.Load("Textures/Menu/empty.png");
			triggerBombIconPrefab = Resources.Load("Textures/Menu/triggerBomb.png");
			contactMineIconPrefab = Resources.Load("Textures/Menu/contactMine.png");
			goldenFlameIconPrefab = Resources.Load("Textures/Menu/goldenFlame.png");
			normalFlameIconPrefab = Resources.Load("Textures/Menu/flame.png");
			superBombActiveIconPrefab = Resources.Load("Textures/Menu/superBombActive.png");
			superBombIconInactivePrefab = Resources.Load("Textures/Menu/superBombInactive.png");
			*/
			
			//Sound effects:
			bombDropSoundEffect = Resources.Load("Sounds/bombDrop") as AudioClip;
			contactMineDropSoundEffect = Resources.Load("Sounds/contactMineDrop") as AudioClip;
			contactMineExplosionSoundEffect = Resources.Load("Sounds/contactMineExplode") as AudioClip;
			powerupSoundEffect = Resources.Load("Sounds/powerup") as AudioClip;
			powerdownSoundEffect = Resources.Load("Sounds/powerdown") as AudioClip;
			superPowerupSoundEffect = Resources.Load("Sounds/super") as AudioClip;
			extraPowerupSoundEffect = Resources.Load("Sounds/extra") as AudioClip;
			playerDeadSoundEffect = Resources.Load("Sounds/dead") as AudioClip;
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
		}
		
		public static void setSphereBuilder(SphereBuilder s) {
			sphereHandler = s;
		}

		public static void setInputHandler(InputHandler i) {
			inputHandler = i;
		}

		public static void setExtra(int type) {
			switch (type) {
			case 0:
				extraIconPrefab = Resources.Load("Textures/Menu/empty");
				break;
			case 1:
				extraIconPrefab = Resources.Load("Textures/Menu/triggerBomb");
				break;
			case 2:
				extraIconPrefab = Resources.Load("Textures/Menu/contactMine");
				break;
			}
		}
		
		public static void setGoldenFlame(bool goldenFlame) {
			if (goldenFlame)
				flameIconPrefab = Resources.Load("Textures/Menu/goldenFlame");
			else
				flameIconPrefab = Resources.Load("Textures/Menu/flame");
		}
		
<<<<<<< HEAD
		public static void setExtra(int type) {
			switch (type) {
			case 0:
				extraIconPrefab = Resources.Load("Textures/Menu/empty.png");
				break;
			case 1:
				extraIconPrefab = Resources.Load("Textures/Menu/triggerBomb.png");
				break;
			case 2:
				extraIconPrefab = Resources.Load("Textures/Menu/contactMine.png");
				break;
			}
		}
		
		public static void setGoldenFlame(bool goldenFlame) {
			if (goldenFlame)
				flameIconPrefab = Resources.Load("Textures/Menu/goldenFlame.png");
			else
				flameIconPrefab = Resources.Load("Textures/Menu/flame.png");
		}
		
		public static void setSuperbomb(bool superbomb) {
			if (superbomb)
				superBombIconPrefab = Resources.Load("Textures/Menu/superBombActive.png");
			else
				superBombIconPrefab = Resources.Load("Textures/Menu/superBombInactive.png");
		}
||||||| merged common ancestors
=======
		public static void setSuperbomb(bool superbomb) {
			if (superbomb)
				superBombIconPrefab = Resources.Load("Textures/Menu/superBombActive");
			else
				superBombIconPrefab = Resources.Load("Textures/Menu/superBombInactive");
		}
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
		
		public static void setRink(Rink r) {
			rink = r;
			gameArea = rink.getGameArea();
			drawnArea = rink.drawnArea;
		}
	}
}

