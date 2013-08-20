using UnityEngine;
using System.Collections;

public class anim : MonoBehaviour {

    float t = 0.0f;
	public float timer = 3.0f;
	Renderer renderer;
	
	void Start() {
		// Aufblinken beim Bombenlegen verbeiden
		renderer = transform.Find("child").gameObject.transform.Find("default").gameObject.transform.renderer;
		renderer.material.SetColor("_Color", Color.black);
		transform.localScale = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
        t += Time.deltaTime;
        float s = Mathf.Abs(Mathf.Sin(2*t));
       	//float f = 0.2f + 0.25f*s;
       	float f = 0.4f + 0.15f*s;
       	f += (t/timer)/4;
		f *= 0.3f; // Größenanpassung auf Sphere
        transform.localScale = new Vector3(f, f, f);
		renderer.material.SetColor("_Color", new Color(t/1.5f, 0f, 0f));
	}
}
