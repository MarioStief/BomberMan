using UnityEngine;

public class takeColor : MonoBehaviour {
	
	void OnColorChange(HSBColor color) {
		Menu.setPlayerColor(color.ToColor());
	}
	
}
