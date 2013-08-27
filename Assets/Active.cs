using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
	public class Active : MonoBehaviour {
	
		private bool active = false;
		private float createTime = Time.time;
		
		void Update () {
			if (!active)
				if (Time.time - createTime > 1f)
					active = true;
		}
		
		public bool isActive() {
			return active;
		}
	}
}