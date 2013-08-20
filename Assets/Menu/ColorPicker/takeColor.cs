using UnityEngine;

namespace AssemblyCSharp
{
	public class takeColor : MonoBehaviour {
		
		void OnColorChange(HSBColor color) {
			Color c = color.ToColor();
			c.a = 1f;
			Menu.setPlayerColor(c);
		}
		
	}
}