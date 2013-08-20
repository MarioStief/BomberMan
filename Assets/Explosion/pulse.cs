using UnityEngine;
using System.Collections;

public class pulse : MonoBehaviour {
	
	float t = 0f;
	int angle = 0;
	
	void Update () {
		t += Time.deltaTime;
		angle = angle+1 % 360;
		//transform.RotateAround (Vector3.zero, transform.position, 100 * Time.deltaTime);
		transform.Rotate(0f, angle, 0f, Space.Self);
		light.intensity = 3 * Mathf.Abs(Mathf.Tan(t));
	}
}
