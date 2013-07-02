using UnityEngine;
using System.Collections;

public class anim : MonoBehaviour {

    float t = 0.0f;
	public float timer = 3.0f;
	public bool triggerBomb = false;
	
	void Start() {
		// Aufblinken beim Bomblegen verbeiden
		if (triggerBomb) {
			transform.renderer.material.SetColor("_Color", Color.blue);
		} else {
			transform.renderer.material.SetColor("_Color", Color.black);
		}
		transform.localScale = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
        t += Time.deltaTime;
        float s = Mathf.Abs(Mathf.Sin(2*t));
       	float f = 0.2f + 0.25f*s;
		if (!triggerBomb) {
        	f += (t/timer)/4;
		}
		f *= 0.3f; // Größenanpassung auf Sphere
        transform.localScale = new Vector3(f, f, f);
		if (triggerBomb) {
			transform.renderer.material.SetColor("_Color", new Color(0f, 0f, f));
		} else {
			transform.renderer.material.SetColor("_Color", new Color(t/2, 0f, 0f));
		}
	}
}
