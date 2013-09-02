using UnityEngine;
using System.Collections;

public class setSize : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.position = Vector3.zero;
		transform.localScale = Vector3.zero;
		guiTexture.pixelInset = new Rect(0,Screen.height/3,Screen.width, Screen.height*2/3);
	}

}
