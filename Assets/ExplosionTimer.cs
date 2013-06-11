using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExplosionTimer : MonoBehaviour {
	
	// ########## DEPRECATED ##########
	
	// Update is called once per frame
	void Update () {
		/* Auskommentierter Code Henning
	
		List<Explosion> delExplosions = new List<Explosion>();
		
		foreach(Explosion e in Data.explosions){
		
			if (!e.addTime(Time.deltaTime)){
				delExplosions.Add(e);
			}
		}	
		
		foreach(Explosion e in delExplosions){
			Data.explosions.Remove(e);	
		}
		*/
	}
}
