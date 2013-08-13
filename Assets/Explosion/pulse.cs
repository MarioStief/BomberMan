using UnityEngine;
using System.Collections;

public class pulse : MonoBehaviour {
	
	float t = 0f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		t += Time.deltaTime;
		transform.RotateAround (Vector3.zero, transform.position, 100 * Time.deltaTime);
		light.intensity = 4 * Mathf.Abs(Mathf.Tan(t));
	}
}
