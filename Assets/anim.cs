using UnityEngine;
using System.Collections;

public class anim : MonoBehaviour {

    public float fac = 0.5f;
	public float scale = 1.0f;
    float t = 0.0f;
	
	// Update is called once per frame
	void Update () {
        t += Time.deltaTime;
        float s = Mathf.Abs(Mathf.Sin(fac * t));
        float f = 0.25f/scale + 0.25f*s*scale;
        transform.localScale = new Vector3(f, f, f);
		transform.renderer.material.SetColor("_Color", new Color(s, 0.0f, 0.0f));
	}
}
