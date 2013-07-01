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
		
		public static UnityEngine.Object bombPrefab;
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
		public static UnityEngine.Object explosionPrefab;
		public static UnityEngine.Object boxCubePrefab;
		public static UnityEngine.Object stoneCubePrefab;
		
		static Static() {
			bombPrefab = Resources.Load("Prefabs/bombPrefab");
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
			explosionPrefab = Resources.Load("Prefabs/explosionPrefab");
			boxCubePrefab = Resources.Load("Prefabs/boxCubePrefab");
			stoneCubePrefab = Resources.Load("Prefabs/stoneCubePrefab");
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

