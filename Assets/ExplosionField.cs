using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class ExplosionField
	{
		private GameObject explosion;
		private int delay;
		private Parcel cell;
		
		public ExplosionField(int delay, Parcel cell) {
			//this.explosion = explosion;
			//this.direction = direction;
			this.delay = delay;
			this.cell = cell;
		}
		
		public GameObject getExplosion() {
			return explosion;
		}

		public int getDelay() {
			return delay;
		}

		public Parcel getCell() {
			return cell;
		}

		public void decrement() {
			delay--;
		}
	}
}

