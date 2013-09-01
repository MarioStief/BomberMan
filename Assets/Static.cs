using UnityEngine;
using AssemblyCSharp;

public static class Static
{
	public static SphereBuilder sphereHandler;
	public static InputHandler inputHandler;
	public static Rink rink;
	public static Parcel[][] gameArea;
	public static MeshManipulator[][] drawnArea;
    public static Player player = new Player();
	public static Menu menuHandler;
	public static Camera camera;

	public static Object bombPrefab;
	public static Object triggerbombPrefab;
	public static Object contactMinePrefab;
	public static Object powerupPrefab;
	public static Object explosionPrefab;

	public static Object boxCube1Prefab;
	public static Object boxCube2Prefab;
	public static Object stoneCube1Prefab;
	public static Object stoneCube2Prefab;

	public static Shader transparentDiffuseShader;

	public static Texture2D dynamiteIconPrefab;
	public static Texture2D playerSpeedIconPrefab;
	public static Texture2D delaySpeedIconPrefab;

	// dynamically filled prefabs:
	public static Texture2D flameIconPrefab;
	public static Texture2D superBombIconPrefab;
	public static Texture2D extraIconPrefab;

	//Sound effects:
	public static AudioClip bombDropSoundEffect;
	public static AudioClip contactMineDropSoundEffect;
	public static AudioClip contactMineExplosionSoundEffect;
	public static AudioClip powerupSoundEffect;
	public static AudioClip powerdownSoundEffect;
	public static AudioClip superPowerupSoundEffect;
	public static AudioClip extraPowerupSoundEffect;
	public static AudioClip playerDeadSoundEffect;
	public static AudioClip superExplosionSoundEffect;

	/*
	 * if needed: the static ones:
	 * public static Object emptyIconPrefab;
	 * public static Object triggerBombIconPrefab;
	 * public static Object contactMineIconPrefab;
	 * public static Object goldenFlameIconPrefab;
	 * public static Object normalFlameIconPrefab;
	 * public static Object superBombActiveIconPrefab;
	 * public static Object superBombIconInactivePrefab;
	 */

	static Static() {

		//bombPrefab = Resources.Load("Prefabs/bombPrefab");
		bombPrefab = Resources.Load("Prefabs/bomb/bomb");
		triggerbombPrefab = Resources.Load("Prefabs/triggerbomb/triggerbomb");
		//contactMinePrefab = Resources.Load("Prefabs/contactMinePrefab");
		contactMinePrefab = Resources.Load("Prefabs/contactmine/contactMine");
		powerupPrefab = Resources.Load("Prefabs/powerupPrefab");
		explosionPrefab = Resources.Load("Prefabs/explosionPrefab");

		boxCube1Prefab = Resources.Load("Prefabs/boxCube1Prefab");
		boxCube2Prefab = Resources.Load("Prefabs/boxCube2Prefab");
		stoneCube1Prefab = Resources.Load("Prefabs/stoneCube1Prefab");
		stoneCube2Prefab = Resources.Load("Prefabs/stoneCube2Prefab");

		transparentDiffuseShader = Shader.Find("Transparent/Diffuse");

		delaySpeedIconPrefab = Resources.Load("Textures/Menu/delaySpeed") as Texture2D;
		superBombIconPrefab = Resources.Load("Textures/Menu/superBombInactive") as Texture2D;
		extraIconPrefab = Resources.Load("Textures/Menu/empty") as Texture2D;

		//again: dynamic:
		dynamiteIconPrefab = Resources.Load("Textures/Menu/dynamite") as Texture2D;
		playerSpeedIconPrefab = Resources.Load("Textures/Menu/playerSpeed") as Texture2D;
		flameIconPrefab = Resources.Load("Textures/Menu/flame") as Texture2D;

		/* static:
		emptyIconPrefab = Resources.Load("Textures/Menu/empty.png");
		triggerBombIconPrefab = Resources.Load("Textures/Menu/triggerBomb.png");
		contactMineIconPrefab = Resources.Load("Textures/Menu/contactMine.png");
		goldenFlameIconPrefab = Resources.Load("Textures/Menu/goldenFlame.png");
		normalFlameIconPrefab = Resources.Load("Textures/Menu/flame.png");
		superBombActiveIconPrefab = Resources.Load("Textures/Menu/superBombActive.png");
		superBombIconInactivePrefab = Resources.Load("Textures/Menu/superBombInactive.png");
		*/

		//Sound effects:
		bombDropSoundEffect = Resources.Load("Sounds/bombDrop") as AudioClip;
		contactMineDropSoundEffect = Resources.Load("Sounds/contactMineDrop") as AudioClip;
		contactMineExplosionSoundEffect = Resources.Load("Sounds/contactMineExplode") as AudioClip;
		powerupSoundEffect = Resources.Load("Sounds/powerup") as AudioClip;
		powerdownSoundEffect = Resources.Load("Sounds/powerdown") as AudioClip;
		superPowerupSoundEffect = Resources.Load("Sounds/super") as AudioClip;
		extraPowerupSoundEffect = Resources.Load("Sounds/extra") as AudioClip;
		playerDeadSoundEffect = Resources.Load("Sounds/dead") as AudioClip;
		superExplosionSoundEffect = Resources.Load("Sounds/superExplosion") as AudioClip;
	}

	public static void setSphereBuilder(SphereBuilder s) {
		sphereHandler = s;
	}

	public static void setInputHandler(InputHandler i) {
		inputHandler = i;
	}

	public static void setMenu(Menu m) {
		menuHandler = m;
	}

	public static void setCamera(Camera c) {
		camera = c;
	}

	public static void setExtra(int type) {
		switch (type) {
		case 0:
			extraIconPrefab = Resources.Load("Textures/Menu/empty") as Texture2D;
			break;
		case 1:
			extraIconPrefab = Resources.Load("Textures/Menu/triggerBomb") as Texture2D;
			break;
		case 2:
			extraIconPrefab = Resources.Load("Textures/Menu/contactMine") as Texture2D;
			break;
		}
	}

	public static void setGoldenFlame(bool goldenFlame) {
		if (goldenFlame)
			flameIconPrefab = Resources.Load("Textures/Menu/goldenFlame") as Texture2D;
		else
			flameIconPrefab = Resources.Load("Textures/Menu/flame") as Texture2D;
	}

	public static void setSuperbomb(bool superbomb) {
		if (superbomb)
			superBombIconPrefab = Resources.Load("Textures/Menu/superBombActive") as Texture2D;
		else
			superBombIconPrefab = Resources.Load("Textures/Menu/superBombInactive") as Texture2D;
	}

	public static void setRink(Rink r) {
		rink = r;
		gameArea = rink.getGameArea();
		drawnArea = rink.drawnArea;
	}

	public static AudioClip selectRandomMusic() {
		Object[] musicClips = Resources.LoadAll("Sounds/Music", typeof(AudioClip));
		AudioClip clip = musicClips[UnityEngine.Random.Range(0, musicClips.Length)] as AudioClip;
		Debug.Log("Playing background music \"" + clip.name + "\"");
		return clip;
	}
}