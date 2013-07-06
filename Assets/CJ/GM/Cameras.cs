using UnityEngine;
using System.Collections;

// allows easy switching between the two relevant cameras.
// attached to the gamecontroller
public class Cameras : MonoBehaviour {

    // set these in the editor
    public GameObject obj_menuCam; // camera used in the menu
    public GameObject obj_gameCam; // camera used ingame

    public enum CAM_TYPE { MENU, GAME };

    public void SetCamera(CAM_TYPE type)
    {
        bool enableMenu = CAM_TYPE.MENU == type;
        obj_menuCam.camera.enabled = enableMenu;
        obj_gameCam.camera.enabled = !enableMenu;
    }
}
