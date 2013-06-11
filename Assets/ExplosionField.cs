using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class ExplosionField
	{
		private GameObject explosion;
		private int delay;
		
		public ExplosionField(GameObject explosion, int delay) {
			this.explosion = explosion;
			this.delay = delay;
		}
		
		public GameObject getExplosion() {
			return explosion;
		}

		public int getDelay() {
			return delay;
		}
		
		public void decrement() {
			delay--;
		}
	}
}

