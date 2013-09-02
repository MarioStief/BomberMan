using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
	public class Active : MonoBehaviour {
	
		private bool activated = false;
		private float createTime = Time.time;
		
		void Update () {
			if (!activated)
				if (Time.time - createTime > 1f)
					activated = true;
		}
		
		public bool isActive() {
			return activated;
		}
	}
}