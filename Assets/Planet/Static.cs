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
		public static UnityEngine.Object explosionPrefab;
		public static UnityEngine.Object boxCube1Prefab;
		public static UnityEngine.Object boxCube2Prefab;
		public static UnityEngine.Object stoneCube1Prefab;
		public static UnityEngine.Object stoneCube2Prefab;
		public static Shader alphaBlended;
		
		static Static() {
			
			bombPrefab = Resources.Load("Prefabs/bombPrefab");
			powerupPrefab = Resources.Load("Prefabs/powerupPrefab");
			explosionPrefab = Resources.Load("Prefabs/explosionPrefab");
			boxCube1Prefab = Resources.Load("Prefabs/boxCube1Prefab");
			boxCube2Prefab = Resources.Load("Prefabs/boxCube2Prefab");
			stoneCube1Prefab = Resources.Load("Prefabs/stoneCube1Prefab");
			stoneCube2Prefab = Resources.Load("Prefabs/stoneCube2Prefab");
			alphaBlended = Shader.Find("Particles/Alpha Blended");
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

