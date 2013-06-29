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

