using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public static class Static
	{
		public static SphereBuilder sphereHandler;
		public static Rink rink;
		public static Parcel[][] gameArea;
		public static MeshManipulator[][] drawnArea;
        public static Player player = new Player();
		
		public static UnityEngine.Object bombPrefab;
		public static UnityEngine.Object powerupPrefab;
		public static UnityEngine.Object powerdownPrefab;
		public static UnityEngine.Object superPowerupPrefab;
		/*
		public static UnityEngine.Object bombUpPowerupPrefab;
		public static UnityEngine.Object bombDownPowerupPrefab;
		public static UnityEngine.Object flameUpPowerupPrefab;
		public static UnityEngine.Object flameDownPowerupPrefab;
		public static UnityEngine.Object playerSpeedUpPowerupPrefab;
		public static UnityEngine.Object playerSpeedDownPowerupPrefab;
		public static UnityEngine.Object delaySpeedUpPowerupPrefab;
		public static UnityEngine.Object delaySpeedDownPowerupPrefab;
		public static UnityEngine.Object goldenFlamePowerupPrefab;
		public static UnityEngine.Object superbombPowerupPrefab;
		public static UnityEngine.Object triggerbombPowerupPrefab;
		*/
		public static UnityEngine.Object explosionPrefab;
		public static UnityEngine.Object boxCube1Prefab;
		public static UnityEngine.Object boxCube2Prefab;
		public static UnityEngine.Object stoneCube1Prefab;
		public static UnityEngine.Object stoneCube2Prefab;
		
		static Static() {
			bombPrefab = Resources.Load("Prefabs/bombPrefab");
			powerupPrefab = Resources.Load("Prefabs/powerupPrefab");
			powerdownPrefab = Resources.Load("Prefabs/powerdownPrefab");
			superPowerupPrefab = Resources.Load("Prefabs/superPowerupPrefab");
			/*
			bombUpPowerupPrefab = Resources.Load("Prefabs/bombUpPrefab");
			bombDownPowerupPrefab = Resources.Load("Prefabs/bombDownPrefab");
			flameUpPowerupPrefab = Resources.Load("Prefabs/flameUpPrefab");
			flameDownPowerupPrefab = Resources.Load("Prefabs/flameDownPrefab");
			playerSpeedUpPowerupPrefab = Resources.Load("Prefabs/playerSpeedUpPrefab");
			playerSpeedDownPowerupPrefab = Resources.Load("Prefabs/playerSpeedDownPrefab");
			delaySpeedUpPowerupPrefab = Resources.Load("Prefabs/delaySpeedUpPrefab");
			delaySpeedDownPowerupPrefab = Resources.Load("Prefabs/delaySpeedDownPrefab");
			goldenFlamePowerupPrefab = Resources.Load("Prefabs/goldenFlamePrefab");
			superbombPowerupPrefab = Resources.Load("Prefabs/superbombPrefab");
			triggerbombPowerupPrefab = Resources.Load("Prefabs/triggerbombPrefab");
			*/
			explosionPrefab = Resources.Load("Prefabs/explosionPrefab");
			boxCube1Prefab = Resources.Load("Prefabs/boxCube1Prefab");
			boxCube2Prefab = Resources.Load("Prefabs/boxCube2Prefab");
			stoneCube1Prefab = Resources.Load("Prefabs/stoneCube1Prefab");
			stoneCube2Prefab = Resources.Load("Prefabs/stoneCube2Prefab");
		}
		
		public static void setSphereBuilder(SphereBuilder s) {
			sphereHandler = s;
		}
		
		
		public static void setRink(Rink r) {
			rink = r;
			gameArea = rink.getGameArea();
			drawnArea = rink.drawnArea;
		}
	}
}

