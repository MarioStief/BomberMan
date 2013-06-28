using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class ExplosionField
	{
//		private GameObject explosion;
		private int delay;
		private Parcel cell;
		int barrier; // 0 = no barrier, 1 = destroyable block, 2 = barrier
		private int lpos;
		private int bpos;
		
		public ExplosionField(int delay, Parcel cell, int barrier, int lpos, int bpos) {
			//this.explosion = explosion;
			//this.direction = direction;
			this.delay = delay;
			this.cell = cell;
			this.barrier = barrier;
			this.lpos = lpos;
			this.bpos = bpos;
		}
		/*
		public GameObject getExplosion() {
			return explosion;
		}
*/
		public int getDelay() {
			return delay;
		}

		public Parcel getCell() {
			return cell;
		}
		
		public int getBarrier() {
			return barrier;
		}
		
		public int getLpos() {
			return lpos;
		}

		public int getBpos() {
			return bpos;
		}

		public void decrement() {
			delay--;
		}
	}
}

