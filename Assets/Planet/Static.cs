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
		//public static GameObject bombPrefab;
		public static GameObject bombUpPowerupPrefab;
		public static GameObject bombDownPowerupPrefab;
		public static GameObject flameUpPowerupPrefab;
		public static GameObject flameDownPowerupPrefab;
		public static GameObject playerSpeedUpPowerupPrefab;
		public static GameObject playerSpeedDownPowerupPrefab;
		public static GameObject delaySpeedUpPowerupPrefab;
		public static GameObject delaySpeedDownPowerupPrefab;
		public static GameObject goldenFlamePowerupPrefab;
		public static GameObject superbombPowerupPrefab;
		public static GameObject triggerbombPowerupPrefab;
		public static GameObject explosionPrefab;
		//public static GameObject deadPlayerPrefab;
		public static GameObject boxCubePrefab;
		public static GameObject stoneCubePrefab;
		
		static Static() {
			//bombPrefab = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("Prefabs/bombPrefab"));
			bombUpPowerupPrefab = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/bombUpPrefab")) as GameObject;
			bombDownPowerupPrefab = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/bombDownPrefab")) as GameObject;
			flameUpPowerupPrefab = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/flameUpPrefab")) as GameObject;
			flameDownPowerupPrefab = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/flameDownPrefab")) as GameObject;
			playerSpeedUpPowerupPrefab = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/playerSpeedUpPrefab")) as GameObject;
			playerSpeedDownPowerupPrefab = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/playerSpeedDownPrefab")) as GameObject;
			delaySpeedUpPowerupPrefab = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/delaySpeedUpPrefab")) as GameObject;
			delaySpeedDownPowerupPrefab = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/delaySpeedDownPrefab")) as GameObject;
			goldenFlamePowerupPrefab = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/goldenFlamePrefab")) as GameObject;
			superbombPowerupPrefab = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/superbombPrefab")) as GameObject;
			triggerbombPowerupPrefab = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/triggerbombPrefab")) as GameObject;
			explosionPrefab = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/explosionPrefab")) as GameObject;
			//deadPlayerPrefab = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/deadPlayerPrefab")) as GameObject;
			boxCubePrefab = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/boxCubePrefab")) as GameObject;
			stoneCubePrefab = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/stoneCubePrefab")) as GameObject;
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

