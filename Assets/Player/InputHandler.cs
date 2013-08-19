using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

// <summary>
// InputHandler nimmt jeglichen relevanten Input entgegen und verarbeitet diesen
// bzw. leiten ihn an verarbeitende Klassen weiter.
// </summary>

public class InputHandler : MonoBehaviour {
	
	bool DEBUGPLAYERPOSITION = false;
	
	int angle = 0;
	
	private int n_L;				// Anzahl Längen und Breitengeraden
	private int n_B;
	
	private int lpos;				// Position der aktuellen Parzelle rink.gameArea ist [lpos][bpos]
	private int bpos;
	
	private float verticalAngle;
	private float horizontalAngle;
	
	private static float vertAngleM, vertAngle=0; // statische Kopie von verticalAngle
	
	private float verticalHelper;
	private float horizontalHelper;
	
	private int vDirection;			// Bewegungsrichtung
	private int hDirection;
	
	private GameObject cam;
	private GameObject sun;
	
	private Parcel currCell;
	
	private float createTime;

    private Player player = null;

    private NET_Client scr_netClient = null;
    private bool isActive = true;
	
	float verticalMovement;
	float horizontalMovement;
	
	bool running = false;
	
	private float playerRadius = 3.5f * Mathf.Deg2Rad;
	
	void Awake() {
		Static.setInputHandler(this);
		sun = GameObject.FindGameObjectWithTag("Sun");
		cam = GameObject.FindGameObjectWithTag("MainCamera");
		
		DontDestroyOnLoad(gameObject);
	}
<<<<<<< HEAD

    public void SetPlayer(Player player) 
    {
        this.player = player;
    }

    public void SetPosition(Rink.Pos rpos)
    {
        this.bpos = rpos.bpos;
        this.lpos = rpos.lpos;
    }

    public void SetActive(bool active)
    {
        isActive = active;
    }

    public float GetVerticalAngle() { return verticalAngle; }
    public float GetHorizontalAngle() { return horizontalAngle; }
||||||| merged common ancestors
=======

>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
	
	// Use this for initialization
	void Start () {
<<<<<<< HEAD
		camera = GameObject.FindGameObjectWithTag("MainCamera");

        GameObject obj_gameController = GameObject.FindGameObjectWithTag("GameController");
        scr_netClient = obj_gameController.GetComponent<NET_Client>();
		
		// set my color
		renderer.material.color = Menu.getPlayerColor();
		
||||||| merged common ancestors
		
		if (!networkView.isMine) {
			return;
		}
		
		camera = GameObject.FindGameObjectWithTag("MainCamera");
		
		// set my color
		renderer.material.color = Menu.getPlayerColor();
		
=======
		
		// colorate the player
		Texture2D illuminColor = Instantiate(Resources.Load("Textures/Player/astrod00d_selfillum") as Texture2D) as Texture2D;
		Color[] color = illuminColor.GetPixels();
		
		Color pColor = Menu.getPlayerColor(networkView.owner);
		for (int i = 0; i < color.Length; i++)
			if (color[i] != color[0])
				color[i] = pColor;
		
		illuminColor.SetPixels(color);
		illuminColor.Apply();
		
		renderer.material.SetTexture("_SelfIllumin", illuminColor);
		
		if (!networkView.isMine) {
			verticalAngle = vertAngle;
			return;
		}
		
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
		Static.sphereHandler.move(0.000001f); // CK, fixed color on startup :)
		
<<<<<<< HEAD
		createTime = Time.time;

        n_L = GM_World.N_L;
        n_B = GM_World.N_B;

        lpos = GM_World.N_L / 2 - 1;
        bpos = GM_World.N_B / 4;

        // be compatible with SP
        if (null == player) player = Static.player;
		
		currCell = Static.rink.gameArea[lpos][bpos];
        player.setCurrentParcel(currCell);
||||||| merged common ancestors
		createTime = Time.time;
		
		
		n_L = Static.sphereHandler.n_L;
		n_B = Static.sphereHandler.n_B;
		
		lpos = n_L/2-1;
		bpos = n_B/4;
		
		currCell = Static.rink.gameArea[lpos][bpos];
		Player.setCurrentParcel(currCell);
=======
		createTime = Time.time;
		
		n_L = Static.sphereHandler.n_L;
		n_B = Static.sphereHandler.n_B;
		
		lpos = n_L/2-1;
		bpos = n_B/4;
		
		currCell = Static.rink.gameArea[lpos][bpos];
		Static.player.setCurrentParcel(currCell);
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
		
		vDirection = 0;
		hDirection = 0;
		
		verticalAngle = 0.0f;
		horizontalAngle = 0.0f;
		
		verticalHelper = 0.0f;
		horizontalHelper = 0.0f;
	}
	
	public void playSound(AudioClip clip) {
		AudioSource audioSource = gameObject.GetComponent<AudioSource>();
		if (audioSource.isPlaying) {
			// neue Soundsource dazu
			foreach (AudioSource audioIterator in GetComponents<AudioSource>()) {
				if (!audioIterator.isPlaying) // entferne nicht mehr laufende
					Destroy(audioIterator);
			}
			audioSource = gameObject.AddComponent<AudioSource>();
		}
		audioSource.clip = clip;
		audioSource.Play();
	}
	
<<<<<<< HEAD
	public Vector3 ToLocalWorld(NET_ActorState.Message otherState)
    {
		Vector3 fp = otherState.position;
        float fva = otherState.vertAng;
        float fha = otherState.horzAng;
			
		// turn the player to our zero
		fp = new Vector3(
			Mathf.Cos(-fha) * fp.x - Mathf.Sin(-fha) * fp.y,
			Mathf.Sin(-fha) * fp.x + Mathf.Cos(-fha) * fp.y,
			fp.z
		);
		// now turn him up/down
		fp = new Vector3(
			fp.x,
			Mathf.Cos(fva-verticalAngle) * fp.y - Mathf.Sin(fva-verticalAngle) * fp.z,
			Mathf.Sin(fva-verticalAngle) * fp.y + Mathf.Cos(fva-verticalAngle) * fp.z
		);
		// and back
		return new Vector3(
			Mathf.Cos(fha) * fp.x - Mathf.Sin(fha) * fp.y,
			Mathf.Sin(fha) * fp.x + Mathf.Cos(fha) * fp.y,
			fp.z
		);	
||||||| merged common ancestors
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if (stream.isWriting)
		{
			// calculate my position on sphere
			/*Vector3 p = new Vector3(
				transform.position.x,
				Mathf.Cos(verticalAngle) * transform.position.y - Mathf.Sin(verticalAngle) * transform.position.z,
				Mathf.Sin(verticalAngle) * transform.position.y + Mathf.Cos(verticalAngle) * transform.position.z
			);*/
			Vector3 p = transform.position;
			stream.Serialize(ref p);
			
			float va = verticalAngle;
			stream.Serialize(ref va);
			float ha = horizontalAngle;
			stream.Serialize(ref ha);
			
			Quaternion r = transform.rotation;
			stream.Serialize(ref r);
		}
		else
		{
			Vector3 fp = Vector3.zero;
			stream.Serialize(ref fp);
			
			float fva = 0f;
			stream.Serialize(ref fva);
			
			float fha = 0f;
			stream.Serialize(ref fha);
			
			// turn the player to our zero
			fp = new Vector3(
				Mathf.Cos(-fha) * fp.x - Mathf.Sin(-fha) * fp.y,
				Mathf.Sin(-fha) * fp.x + Mathf.Cos(-fha) * fp.y,
				fp.z
			);
			// now turn him up/down
			fp = new Vector3(
				fp.x,
				Mathf.Cos(fva-verticalAngle) * fp.y - Mathf.Sin(fva-verticalAngle) * fp.z,
				Mathf.Sin(fva-verticalAngle) * fp.y + Mathf.Cos(fva-verticalAngle) * fp.z
			);
			// and back
			transform.position = new Vector3(
				Mathf.Cos(fha) * fp.x - Mathf.Sin(fha) * fp.y,
				Mathf.Sin(fha) * fp.x + Mathf.Cos(fha) * fp.y,
				fp.z
			);
			
			
			Quaternion fr = Quaternion.Euler(new Vector3(0, 0, 0));
			stream.Serialize(ref fr);
			transform.rotation = fr;
		}
	
=======
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if (stream.isWriting) {
			Vector3 p = transform.position;
			stream.Serialize(ref p);
			
			float va = verticalAngle;
			stream.Serialize(ref va);
			
			Quaternion r = transform.rotation;
			stream.Serialize(ref r);
			
			bool ir = running;
			stream.Serialize(ref ir);
		}
		else {
			Vector3 fp = Vector3.zero;
			stream.Serialize(ref fp);
			
			float fva = 0f;
			stream.Serialize(ref fva);
			
			Quaternion fr = Quaternion.Euler(new Vector3(0, 0, 0));
			stream.Serialize(ref fr);
			transform.rotation = fr;
			
			transform.position = fp; // Spieler ist auf Äquator
			Vector3 axis = Vector3.Cross(Vector3.forward, transform.position).normalized;
			transform.RotateAround(Vector3.zero, axis, (verticalAngle * Mathf.Rad2Deg) + (-fva * Mathf.Rad2Deg));
			
			// Animate Player
			bool fir = false;
			stream.Serialize(ref fir);
			if (fir)
				GetComponentInChildren<Animation>().CrossFade("runforward");
			else
				GetComponentInChildren<Animation>().CrossFade("idle");
		}
	
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
	}
	
	IEnumerator deadPlayer() {
		float createTime = Time.time;
		float elapsedTime = 0.0f;
		GetComponentInChildren<Animation>().CrossFade("die");
		while (elapsedTime < 10f) {
			float multiplicator = elapsedTime + 10f; // 10 <= multiplicator <= 20
			float x = transform.position.x;
			float y = transform.position.y;
			float z = transform.position.z;
			yield return new WaitForSeconds(Random.value);
			Vector3 position = new Vector3(Random.Range(x-0.1f, x+0.1f), Random.Range(y-0.1f, y+0.1f), Random.Range(z-0.1f, z+0.1f));
			GameObject explosion = GameObject.Instantiate(Static.explosionPrefab, position, Quaternion.identity) as GameObject;
			Detonator detonator = explosion.GetComponent<Detonator>();
			detonator.setSize(Random.Range(50f * multiplicator, 100f * multiplicator));
			detonator.setDuration(5f);
			float distance = Vector3.Distance (GameObject.FindGameObjectWithTag("Player").transform.position, position);
			detonator.GetComponent<AudioSource>().volume /= distance * multiplicator;
			detonator.GetComponent<AudioSource>().Play();
			detonator.Explode();
			//float scale = 1f - ((multiplicator - 10f) / 10f); // Range: 1 - 0
			//Debug.Log ("---> " + scale);
			//transform.localScale *= scale;
			transform.position -= 0.7f * transform.position.normalized * Time.deltaTime;
			elapsedTime = Time.time - createTime;
		}
<<<<<<< HEAD
		playerHandler.transform.localScale = Vector3.zero;
		playerHandler.GetComponent<CapsuleCollider>().enabled = false;
		while (Static.player.isDead()) {
||||||| merged common ancestors
		playerHandler.transform.localScale = Vector3.zero;
		playerHandler.GetComponent<CapsuleCollider>().enabled = false;
		while (Player.isDead()) {
=======
		transform.localScale = Vector3.zero;
		GetComponent<CapsuleCollider>().enabled = false;
		while (Static.player.isDead()) {
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
			do {			
				switch (Random.Range(0, 2)) {
				case 0:
					if (verticalMovement != 0f) {
						verticalMovement = 0f;
					} else {
						verticalMovement = (Random.Range(0, 2) == 0 ? 0.1f : -0.1f);
					}
					break;
				case 1:
					if (horizontalMovement != 0f) {
						horizontalMovement = 0f;
					} else {
						horizontalMovement = (Random.Range(0, 2) == 0 ? 0.1f : -0.1f);
					}
					break;
				}
			} while (verticalMovement == 0f && horizontalMovement == 0f);
			yield return new WaitForSeconds(Random.value*5 + 5f);
		}
	}

    public Vector3 UpdateFoe(NET_ActorState.Message actorState)
    {
        float verticalMovement = Input.GetAxis("Vertical");
        float vm = Static.player.getSpeed() * verticalMovement * Time.deltaTime;
        vm = determineVerticalParcelPosition(verticalMovement, vm);
        // verticalAngle += vm;
        vm = verticalAngle + actorState.vertAng;

        horizontalMovement = Input.GetAxis("Horizontal") * Static.player.getSpeed();
        float m = horizontalMovement * Time.deltaTime * Static.player.getSpeed() * (-2);
        m = determineHorizontalParcelPosition(horizontalMovement, m);
        //horizontalAngle += m;

        if (vertAngle != 0)
        {
            // turn the player to our zero
            Vector3 tmp = new Vector3(
                Mathf.Cos(-horizontalAngle) * actorState.position.x - Mathf.Sin(-horizontalAngle) * actorState.position.y,
                Mathf.Sin(-horizontalAngle) * actorState.position.x + Mathf.Cos(-horizontalAngle) * actorState.position.y,
                actorState.position.z
            );
            // now turn him up/down
            tmp = new Vector3(
                tmp.x,
                Mathf.Cos(-vm) * tmp.y - Mathf.Sin(-vm) * tmp.z,
                Mathf.Sin(-vm) * tmp.y + Mathf.Cos(-vm) * tmp.z
            );
            // and back
            return new Vector3(
                Mathf.Cos(horizontalAngle) * tmp.x - Mathf.Sin(horizontalAngle) * tmp.y,
                Mathf.Sin(horizontalAngle) * tmp.x + Mathf.Cos(horizontalAngle) * tmp.y,
                tmp.z
            );
        }

        return actorState.position;
    }
	
<<<<<<< HEAD

	void Update () {

        if (!isActive) return;

        // UpdateFoe();
||||||| merged common ancestors
	void Update () {
		
		// Gegner drehen mit dem Planeten..!
		if (!networkView.isMine) {
			
			float verticalMovement = Input.GetAxis("Vertical");
			float vm = Player.getSpeed() * verticalMovement * Time.deltaTime;
			vm = determineVerticalParcelPosition( verticalMovement, vm);
			verticalAngle += vm;
			
			horizontalMovement = Input.GetAxis("Horizontal") * Player.getSpeed();
			float m = horizontalMovement*Time.deltaTime*Player.getSpeed()*(-2);
			m = determineHorizontalParcelPosition( horizontalMovement, m);
			horizontalAngle += m;
			
			if (vertAngle != 0) {
				// turn the player to our zero
				Vector3 tmp = new Vector3(
					Mathf.Cos(-horizontalAngle) * transform.position.x - Mathf.Sin(-horizontalAngle) * transform.position.y,
					Mathf.Sin(-horizontalAngle) * transform.position.x + Mathf.Cos(-horizontalAngle) * transform.position.y,
					transform.position.z
				);
				// now turn him up/down
				tmp = new Vector3(
					tmp.x,
					Mathf.Cos(-vm) * tmp.y - Mathf.Sin(-vm) * tmp.z,
					Mathf.Sin(-vm) * tmp.y + Mathf.Cos(-vm) * tmp.z
				);
				// and back
				transform.position = new Vector3(
					Mathf.Cos(horizontalAngle) * tmp.x - Mathf.Sin(horizontalAngle) * tmp.y,
					Mathf.Sin(horizontalAngle) * tmp.x + Mathf.Cos(horizontalAngle) * tmp.y,
					tmp.z
				);
			}
			
			return;
		}
=======
	void Update () {
		
		// Gegner drehen mit dem Planeten..!
		if (!networkView.isMine && Static.rink != null) {
			
			if (vertAngleM != 0) { // an Wänden hängen bleiben..
				float vm;
				if (Static.player.isDead()) {
					vm = vertAngleM;
					vertAngleM = 0;
				} else {
					vm = Static.player.getSpeed() * Input.GetAxis("Vertical") * Time.deltaTime;
					vm = determineVerticalParcelPosition(Input.GetAxis("Vertical"), vm);
				}
				verticalAngle += vm;
				//verticalAngle = verticalAngle % (Mathf.PI*2);

				Vector3 axis = Vector3.Cross(Vector3.forward, transform.position);
				transform.RotateAround(Vector3.zero, axis, vm * Mathf.Rad2Deg);
			}
			
			return;
		}

>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
		
<<<<<<< HEAD
		if (!Static.player.isDead()) {
||||||| merged common ancestors
		if (!Player.isDead()) {
=======
		if (Static.rink != null && !Static.player.isDead()) {
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
			
			// -----------------------------------------------------------
			// Bewegung und Bestimmung einer möglichen neuen currentParcel
			// -----------------------------------------------------------
			moveCharacter();
			currCell = Static.rink.gameArea[lpos][bpos];
<<<<<<< HEAD
			if (currCell.hasContactMine()) {
				currCell.getExplosion().startExplosion();
			}
				
            /*
			if (currCell.isExploding()) {
                player.setDead(true);
||||||| merged common ancestors
			if (currCell.isExploding()) {
				Player.setDead(true);
=======
			//currCell.colorCell(Color.cyan);
			
			if (currCell.hasContactMine()) {
				networkView.RPC("startEvent", RPCMode.All, currCell.getLpos(), currCell.getBpos(), 3);
			}
				
			if (currCell.isExploding()) {
				Static.player.setDead(true, networkView);
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
				renderer.material.color = Color.black;
				StartCoroutine(deadPlayer());
				networkView.RPC("removePlayer", RPCMode.OthersBuffered, Network.player);
			}
<<<<<<< HEAD
            */
||||||| merged common ancestors
			
			
			// Falls die Zelle ein Powerup enthält -> aufsammeln
			if (currCell.hasPowerup()) {
				Player.powerupCollected(currCell.destroyPowerup(false));
			}
=======
			
			
			// Falls die Zelle ein Powerup enthält -> aufsammeln
			if (currCell.hasPowerup()) {
				networkView.RPC("startEvent", RPCMode.Others, currCell.getLpos(), currCell.getBpos(), 0);
				Static.player.powerupCollected(currCell.destroyPowerup(false));
			}
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
			
			// Leertaste -> Bombe legen
<<<<<<< HEAD
			if (Input.GetButtonDown("Fire1") ||  Input.GetKeyDown(KeyCode.Space)){
                NET_MSG_PlantBomb plantBombMsg = new NET_MSG_PlantBomb();
                plantBombMsg.pid = scr_netClient.GetLocalPID();
                plantBombMsg.time = 0.0f; // ignored
                plantBombMsg.rpos = player.GetPosition();
                scr_netClient.Send(plantBombMsg);
||||||| merged common ancestors
			if ( Input.GetKeyDown(KeyCode.Space)){
				if ( !currCell.hasBomb()) {
					
					if (Player.addBomb()) {
						Explosion.createExplosionOnCell(currCell, Player.getFlamePower(), Player.getDelay(), true, true);
						// Um eine Bombe eines anderen Spielers auf einer Zelle zu spawnen:
						// Explosion.createExplosionOnCell(Parcel, flamePower, true);
						// Powerup-ToDos: flameMight, flameSpeed
						if (Player.getTriggerbomb()) {
							Player.addTriggerBomb(currCell);
						}
					}
				}
=======
			if (Input.GetKeyDown(KeyCode.Space)) {
				if (!currCell.hasBomb() && !currCell.hasContactMine()) {
					int extra = Static.player.addBomb();
					if (extra > -1) {
						GameObject ex = Network.Instantiate(Resources.Load("Prefabs/Bombe"), currCell.getCenterPos(), Quaternion.identity, 0) as GameObject;
						ex.networkView.RPC("createExplosionOnCell", RPCMode.All, currCell.getLpos(), currCell.getBpos(), 
					    	               Static.player.getFlamePower(), Static.player.getDelay(), Static.player.getSuperbomb(), extra, true, true);
						if (extra == 1)
							Static.player.addTriggerBomb(ex, currCell);
						playSound(Static.bombDropSoundEffect);
					}
				}
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
			}
			
			if ((Input.GetKeyDown(KeyCode.LeftShift)) || (Input.GetKeyDown(KeyCode.RightShift))) {
				if (!currCell.hasBomb() && !currCell.hasContactMine()) {
					if (Static.player.addContactMine()) {
						GameObject ex = Network.Instantiate(Resources.Load("Prefabs/Bombe"), currCell.getCenterPos(), Quaternion.identity, 0) as GameObject;
						ex.networkView.RPC("createExplosionOnCell", RPCMode.All, currCell.getLpos(), currCell.getBpos(), 
					     	              Static.player.getFlamePower(), Static.player.getDelay(), Static.player.getSuperbomb(), 2, true, true);
						playSound(Static.contactMineDropSoundEffect);
					}
				}
				Static.player.releaseTriggerBombs();
			}

			
			if ((Time.time - createTime) > 1.0f) {
<<<<<<< HEAD
				createTime = Time.time;
                player.increaseHP();
||||||| merged common ancestors
				createTime = Time.time;
				Player.increaseHP();
=======
				createTime = Time.time;
				Static.player.increaseHP();
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
			}
		} else {
			moveCharacter();
		}
	}
	
	[RPC]
	public void startEvent(int lpos, int bpos, int mode) {
		Parcel cell = Static.rink.gameArea[lpos][bpos];
		switch (mode) {
			case 0: // destroy powerup
				cell.destroyPowerup(false);
				break;
			case 1: // destroy powerup
				cell.destroyPowerup(true);
				break;
			case 3: // explding contact-mine
				cell.getExplosion().startExplosion();
				break;
		}
	}
	[RPC]
	public void addPowerup(int lpos, int bpos, int type) {
		Parcel cell = Static.rink.gameArea[lpos][bpos];
		cell.addPowerup(new Powerup((PowerupType) type));
	}
	[RPC]
	public void removePlayer(NetworkPlayer p) {
		if (networkView.owner == p) {
			transform.localScale = Vector3.zero;
		}
	}
	
	private void moveCharacter() {
		
<<<<<<< HEAD
		float verticalMovement;
		if (Static.player.isDead()) {
||||||| merged common ancestors
		float verticalMovement;
		if (Player.isDead()) {
=======
		float verticalMovement, vm=0, m=0;
		if (Static.player.isDead()) {
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
			verticalMovement = this.verticalMovement;
		} else {
			verticalMovement = Input.GetAxis("Vertical");
		}
<<<<<<< HEAD
		if ( verticalMovement != 0) {
			float m = Static.player.getSpeed() * verticalMovement * Time.deltaTime;
			if ( vDirection == 0) {
||||||| merged common ancestors
		if ( verticalMovement != 0) {
			float m = Player.getSpeed() * verticalMovement * Time.deltaTime;
			if ( vDirection == 0) {
=======
		if (verticalMovement != 0) {
			vm = Static.player.getSpeed() * verticalMovement * Time.deltaTime;
			if (vDirection == 0) {
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
				
				vDirection = (int)Mathf.Sign(vm);
				
				if (vDirection == 1) {
					verticalHelper -= 	Mathf.PI/(2*(n_L-1));
				} else {
					verticalHelper += 	Mathf.PI/(2*(n_L-1));
				}
			}
		
<<<<<<< HEAD
			float vAngle = verticalAngle;
			verticalAngle += m;

			if (!Static.player.isDead()) {
				m = determineVerticalParcelPosition( verticalMovement, m);
||||||| merged common ancestors
			float vAngle = verticalAngle;
			verticalAngle += m;

			if (!Player.isDead()) {
				m = determineVerticalParcelPosition( verticalMovement, m);
=======
			if (!Static.player.isDead()) {
				vm = determineVerticalParcelPosition(verticalMovement, vm);
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
			}
			verticalAngle += vm;
			//verticalAngle = verticalAngle % (Mathf.PI*2);
			vertAngle = verticalAngle;
			
			Static.sphereHandler.move(vm);
			vertAngleM = vm;
		}
		
		float horizontalMovement;
		if (Static.player.isDead()) {
			horizontalMovement = this.horizontalMovement;
		} else {
			horizontalMovement = Input.GetAxis("Horizontal") * Static.player.getSpeed();
		}
<<<<<<< HEAD
		if ( horizontalMovement != 0){
			float m = horizontalMovement*Time.deltaTime*Static.player.getSpeed()*(-2);
			if ( hDirection == 0) {
||||||| merged common ancestors
		if ( horizontalMovement != 0){
			float m = horizontalMovement*Time.deltaTime*Player.getSpeed()*(-2);
			if ( hDirection == 0) {
=======
		if (horizontalMovement != 0) {
			m = horizontalMovement*Time.deltaTime*Static.player.getSpeed()*(-2);
			if (hDirection == 0) {
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
				
				hDirection = (int)Mathf.Sign(m);
				
				if (hDirection == 1) {
					horizontalHelper += 	Mathf.PI/(n_B);
				} else {
					horizontalHelper -= 	Mathf.PI/(n_B);
				}
				horizontalHelper += m;
			}
		 
<<<<<<< HEAD
			float hAngle = horizontalAngle;
			horizontalAngle += m;
			
			if (!Static.player.isDead()) {
				m = determineHorizontalParcelPosition( horizontalMovement, m);
||||||| merged common ancestors
			float hAngle = horizontalAngle;
			horizontalAngle += m;
			
			if (!Player.isDead()) {
				m = determineHorizontalParcelPosition( horizontalMovement, m);
=======
			if (!Static.player.isDead()) {
				m = determineHorizontalParcelPosition(horizontalMovement, m);
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
			}
			horizontalAngle += m;
			//horizontalAngle = horizontalAngle % (Mathf.PI*2);
			
			moveAlongEquator(m);
		}
		
		// Spielerrotation
		int newAngle = angle;
		if (verticalMovement > 0) {
			// nach oben schauen
			if (horizontalMovement < 0) {
				// nach links oben schauen
				newAngle = 315;
			} else if (horizontalMovement > 0) {
				// nach rechts oben schauen
				newAngle = 45;
			} else {
				// nur nach oben schauen
				newAngle = 0;
			}
		} else if (verticalMovement < 0) {
			// nach unten schauen
			if (horizontalMovement < 0) {
				// nach links unten schauen
				newAngle = 225;
			} else if (horizontalMovement > 0) {
				// nach rechts unten schauen
				newAngle = 135;
			} else {
				// nur nach unten schauen
				newAngle = 180;
			}
		} else {
			if (horizontalMovement < 0) {
				// nur nach links schauen
				newAngle = 270;
			} else if (horizontalMovement > 0) {
				// nur nach rechts schauen
				newAngle = 90;
			}
		}
		
		if (Mathf.Abs(angle - newAngle) > 180)
			angle = (angle + newAngle) / 2 + 180;
		else
			angle = (angle + newAngle) / 2;
		
		transform.up = transform.position;
		transform.Rotate(0f, angle, 0f, Space.Self);
		
		// Animate Player
		if (!Static.player.isDead()) {
			running = Mathf.Abs(m) > 0.002 || Mathf.Abs(vm) > 0.002;
			if (running)
				GetComponentInChildren<Animation>().CrossFade("runforward");
			else
				GetComponentInChildren<Animation>().CrossFade("idle");
		}
	}
	
	// <summary>
	// Bestimme, ob die Spielfigur einen neuen Würfel berührt. Wenn dem so ist, ändere currParcel auf
	// die Position des neuen Würfels.
	// </summary>
	private float determineVerticalParcelPosition(float verticalMovement, float m) {
		
		if (vDirection == 1 && Mathf.Sign(verticalMovement) == 1) {	// Bewegungsrichtung blieb gleich				
				if (DEBUGPLAYERPOSITION)
					Debug.Log("#V1");
			
				// Setting position of player in current cell
<<<<<<< HEAD
				float newPlayerPosition =  1-Mathf.Abs(verticalAngle-verticalHelper)/(Mathf.PI/(n_L-1)); //bs( verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
                player.setXPos(newPlayerPosition);
||||||| merged common ancestors
				float newPlayerPosition =  1-Mathf.Abs(verticalAngle-verticalHelper)/(Mathf.PI/(n_L-1)); //bs( verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Player.setXPos( newPlayerPosition);
=======
				float newPlayerPosition =  1-Mathf.Abs(verticalAngle-verticalHelper)/(Mathf.PI/(n_L-1)); //bs(verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Static.player.setXPos(newPlayerPosition);
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
				if (DEBUGPLAYERPOSITION)
					Debug.Log(newPlayerPosition);
			
				if (Mathf.Abs(verticalAngle - verticalHelper + playerRadius) > Mathf.PI/(n_L-1)) {
				
<<<<<<< HEAD
					Parcel newCell;

					if ( lpos < n_L-2){
						if ( Static.rink.gameArea[lpos+1][bpos].getType() != 0 || Static.rink.gameArea[lpos+1][bpos].hasBomb()){
							return 0.0f;
						}
						newCell = Static.rink.gameArea[++lpos][bpos];
                        player.setXPos(0);

					} else{
						if ( Static.rink.gameArea[0][bpos].getType() != 0 || Static.rink.gameArea[0][bpos].hasBomb()){
							return 0.0f;
						}
						lpos = 0;
						newCell = Static.rink.gameArea[lpos][bpos];
                        player.setXPos(0);
||||||| merged common ancestors
					Parcel newCell;

					if ( lpos < n_L-2){
						if ( Static.rink.gameArea[lpos+1][bpos].getType() != 0 || Static.rink.gameArea[lpos+1][bpos].hasBomb()){
							return 0.0f;
						}
						newCell = Static.rink.gameArea[++lpos][bpos];
						Player.setXPos( 0);

					} else{
						if ( Static.rink.gameArea[0][bpos].getType() != 0 || Static.rink.gameArea[0][bpos].hasBomb()){
							return 0.0f;
						}
						lpos = 0;
						newCell = Static.rink.gameArea[lpos][bpos];
						Player.setXPos( 0);
=======
					Parcel newCell = Static.rink.gameArea[lpos < n_L-2 ? lpos+1 : 0][bpos];
					if (newCell.getType() != 0 || newCell.hasBomb()) {
						return 0.0f;
					}
					if (Mathf.Abs(verticalAngle - verticalHelper) > Mathf.PI/(n_L-1)) {
						//Static.player.setXPos(0);
						verticalHelper += Mathf.PI/(n_L-1);
						lpos = newCell.getLpos();
						Static.player.setCurrentParcel(newCell);
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
					}
<<<<<<< HEAD
					verticalHelper += Mathf.PI/(n_L-1);
                    player.setCurrentParcel(newCell);	
||||||| merged common ancestors
					verticalHelper += Mathf.PI/(n_L-1);
					Player.setCurrentParcel(newCell);	
=======
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
				}
			} else if (vDirection == 1 && Mathf.Sign(verticalMovement) == -1) {	// Bewegungsrichtung ändert sich
				
<<<<<<< HEAD
				//Debug.Log("#2");
				float newPlayerPosition =  Mathf.Abs(verticalAngle-verticalHelper)/(Mathf.PI/(n_L-1)); //bs( verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
                player.setXPos(newPlayerPosition);
||||||| merged common ancestors
				//Debug.Log("#2");
				float newPlayerPosition =  Mathf.Abs(verticalAngle-verticalHelper)/(Mathf.PI/(n_L-1)); //bs( verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Player.setXPos( newPlayerPosition);
=======
				if (DEBUGPLAYERPOSITION)
					Debug.Log("#V2");
				float newPlayerPosition = Mathf.Abs(verticalAngle-verticalHelper)/(Mathf.PI/(n_L-1)); //bs(verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Static.player.setXPos(newPlayerPosition);
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
				if (DEBUGPLAYERPOSITION)
					Debug.Log(newPlayerPosition);
			
				vDirection = -1;
				verticalHelper += Mathf.PI/((n_L-1));
					
				if (Mathf.Abs(verticalAngle - verticalHelper + playerRadius) > Mathf.PI/(n_L-1)) {
					
					Parcel newCell = Static.rink.gameArea[lpos > 0 ? lpos-1 : n_L-2][bpos];
					if (newCell.getType() != 0 || newCell.hasBomb()) {
						return 0.0f;
					}
					if (Mathf.Abs(verticalAngle - verticalHelper) > Mathf.PI/(n_L-1)) {
						verticalHelper -= Mathf.PI/(n_L-1);
						Static.player.setCurrentParcel(newCell);
						lpos = newCell.getLpos();
					}
<<<<<<< HEAD
					verticalHelper -= Mathf.PI/(n_L-1);
                    player.setCurrentParcel(newCell);	
||||||| merged common ancestors
					verticalHelper -= Mathf.PI/(n_L-1);
					Player.setCurrentParcel(newCell);	
=======
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
				}
			} else if (vDirection == -1 && Mathf.Sign(verticalMovement) == -1) {	// Bewegungsrichtung blieb gleich
				
				if (DEBUGPLAYERPOSITION)
					Debug.Log("#V3");
				// Setting position of player in current cell
<<<<<<< HEAD
				float newPlayerPosition =  Mathf.Abs( verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
                player.setXPos(newPlayerPosition);
||||||| merged common ancestors
				float newPlayerPosition =  Mathf.Abs( verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Player.setXPos( newPlayerPosition);
=======
				float newPlayerPosition =  Mathf.Abs(verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Static.player.setXPos(newPlayerPosition);
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
				if (DEBUGPLAYERPOSITION)
					Debug.Log(newPlayerPosition);

				if (Mathf.Abs(verticalAngle - verticalHelper - playerRadius) > Mathf.PI/(n_L-1)) {
					
					Parcel newCell = Static.rink.gameArea[lpos > 0 ? lpos-1 : n_L-2][bpos];
					if (newCell.getType() != 0 || newCell.hasBomb()) {
						return 0.0f;
					}
					if (Mathf.Abs(verticalAngle - verticalHelper) > Mathf.PI/(n_L-1)) {
						verticalHelper -= Mathf.PI/(n_L-1);
						Static.player.setCurrentParcel(newCell);
						lpos = newCell.getLpos();
					}
<<<<<<< HEAD
					verticalHelper -= Mathf.PI/(n_L-1);
                    player.setCurrentParcel(newCell);	
||||||| merged common ancestors
					verticalHelper -= Mathf.PI/(n_L-1);
					Player.setCurrentParcel(newCell);	
=======
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
				}
			} else if (vDirection == -1 && Mathf.Sign(verticalMovement) == 1) {	// Bewegungsrichtung ändert sich
				
				if (DEBUGPLAYERPOSITION)
<<<<<<< HEAD
					Debug.Log("#4");
				float newPlayerPosition =  1-Mathf.Abs(verticalAngle-verticalHelper)/(Mathf.PI/(n_L-1)); //bs( verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
                player.setXPos(newPlayerPosition);
||||||| merged common ancestors
					Debug.Log("#4");
				float newPlayerPosition =  1-Mathf.Abs(verticalAngle-verticalHelper)/(Mathf.PI/(n_L-1)); //bs( verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Player.setXPos( newPlayerPosition);
=======
					Debug.Log("#V4");
				float newPlayerPosition =  1-Mathf.Abs(verticalAngle-verticalHelper)/(Mathf.PI/(n_L-1)); //bs(verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Static.player.setXPos(newPlayerPosition);
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
				if (DEBUGPLAYERPOSITION)
					Debug.Log(newPlayerPosition);
			
				vDirection = 1;
				verticalHelper -=	Mathf.PI/((n_L-1));
				
				if (Mathf.Abs(verticalAngle - verticalHelper + playerRadius) > Mathf.PI/(n_L-1)) {
					
					Parcel newCell = Static.rink.gameArea[lpos < n_L-2 ? lpos+1 : 0][bpos];
					if (newCell.getType() != 0 || newCell.hasBomb()) {
						return 0.0f;
					}
					if (Mathf.Abs(verticalAngle - verticalHelper) > Mathf.PI/(n_L-1)) {
						verticalHelper += Mathf.PI/(n_L-1);
						Static.player.setCurrentParcel(newCell);
						lpos = newCell.getLpos();
					}
<<<<<<< HEAD
					verticalHelper += Mathf.PI/(n_L-1);
                    player.setCurrentParcel(newCell);	
||||||| merged common ancestors
					verticalHelper += Mathf.PI/(n_L-1);
					Player.setCurrentParcel(newCell);	
=======
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
				}
			}
		
		return m;
	}
	
	private float determineHorizontalParcelPosition(float horizontalMovement, float m) {
		
		if (hDirection == -1 && Mathf.Sign(horizontalMovement) == -1) {	// Bewegungsrichtung blieb gleich				
				if (DEBUGPLAYERPOSITION)
					Debug.Log("#H1");
			
<<<<<<< HEAD
				float newPlayerPosition =  1-Mathf.Abs(horizontalAngle-horizontalHelper)/(2*Mathf.PI/n_B); //bs( verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
                player.setZPos(newPlayerPosition);
||||||| merged common ancestors
				float newPlayerPosition =  1-Mathf.Abs(horizontalAngle-horizontalHelper)/(2*Mathf.PI/n_B); //bs( verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Player.setZPos( newPlayerPosition);
=======
				float newPlayerPosition =  1-Mathf.Abs(horizontalAngle-horizontalHelper)/(2*Mathf.PI/n_B); //bs(verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Static.player.setZPos(newPlayerPosition);
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
				if (DEBUGPLAYERPOSITION)
					Debug.Log(newPlayerPosition);
			
			
				if (Mathf.Abs(horizontalAngle - horizontalHelper + playerRadius) > 2*Mathf.PI/n_B) {
					
					Parcel newCell;
					
					if (bpos < n_B-1) {
						if (Static.rink.gameArea[lpos][bpos+1].getType() != 0 || Static.rink.gameArea[lpos][bpos+1].hasBomb()) {
							return 0.0f;
						}
						newCell = Static.rink.gameArea[lpos][bpos+1];
					} else {
						if (Static.rink.gameArea[lpos][0].getType() != 0 || Static.rink.gameArea[lpos][0].hasBomb()) {
							return 0.0f;
						}
						newCell = Static.rink.gameArea[lpos][0];
					}
					if (Mathf.Abs(horizontalAngle - horizontalHelper) > 2*Mathf.PI/n_B) {
						horizontalHelper += 2*Mathf.PI/n_B;
						Static.player.setCurrentParcel(newCell);
						bpos = newCell.getBpos();
					}
<<<<<<< HEAD
					horizontalHelper += 2*Mathf.PI/n_B;
                    player.setCurrentParcel(newCell);	
||||||| merged common ancestors
					horizontalHelper += 2*Mathf.PI/n_B;
					Player.setCurrentParcel(newCell);	
=======
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
				}
			} else if (hDirection == -1 && Mathf.Sign(horizontalMovement) == 1) {	// Bewegungsrichtung ändert sich
				
				if (DEBUGPLAYERPOSITION)
					Debug.Log("#H2");
				
			
				hDirection = 1;
				horizontalHelper +=	2*Mathf.PI/(n_B);
			
<<<<<<< HEAD
				float newPlayerPosition =  Mathf.Abs(horizontalAngle-horizontalHelper)/(2*Mathf.PI/n_B); //bs( verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
                player.setZPos(newPlayerPosition);
||||||| merged common ancestors
				float newPlayerPosition =  Mathf.Abs(horizontalAngle-horizontalHelper)/(2*Mathf.PI/n_B); //bs( verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Player.setZPos( newPlayerPosition);
=======
				float newPlayerPosition =  Mathf.Abs(horizontalAngle-horizontalHelper)/(2*Mathf.PI/n_B); //bs(verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Static.player.setZPos(newPlayerPosition);
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
				if (DEBUGPLAYERPOSITION)
					Debug.Log(newPlayerPosition);
					
				if (Mathf.Abs(horizontalAngle - horizontalHelper - playerRadius) > 2*Mathf.PI/n_B) {
				
					Parcel newCell;
					
					if (bpos > 0) {
						if (Static.rink.gameArea[lpos][bpos-1].getType() != 0 || Static.rink.gameArea[lpos][bpos-1].hasBomb()) {
							return 0.0f;
						}
						newCell = Static.rink.gameArea[lpos][bpos-1];
					} else {
						if (Static.rink.gameArea[lpos][n_B-1].getType() != 0 || Static.rink.gameArea[lpos][n_B-1].hasBomb()) {
							return 0.0f;
						}
						newCell = Static.rink.gameArea[lpos][n_B-1];
					}
					if (Mathf.Abs(horizontalAngle - horizontalHelper) > 2*Mathf.PI/n_B) {
						horizontalHelper -= 2*Mathf.PI/n_B;
						Static.player.setCurrentParcel(newCell);
						bpos = newCell.getBpos();
					}
<<<<<<< HEAD
					horizontalHelper -= 2*Mathf.PI/n_B;
                    player.setCurrentParcel(newCell);	
||||||| merged common ancestors
					horizontalHelper -= 2*Mathf.PI/n_B;
					Player.setCurrentParcel(newCell);	
=======
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
				}
			} else if (hDirection == 1 && Mathf.Sign(horizontalMovement) == 1) {	// Bewegungsrichtung blieb gleich
				
				if (DEBUGPLAYERPOSITION)
					Debug.Log("#H3");
<<<<<<< HEAD
				float newPlayerPosition =  Mathf.Abs(horizontalAngle-horizontalHelper)/(2*Mathf.PI/n_B); //bs( verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
                player.setZPos(newPlayerPosition);
||||||| merged common ancestors
				float newPlayerPosition =  Mathf.Abs(horizontalAngle-horizontalHelper)/(2*Mathf.PI/n_B); //bs( verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Player.setZPos( newPlayerPosition);
=======
			
				float newPlayerPosition =  Mathf.Abs(horizontalAngle-horizontalHelper)/(2*Mathf.PI/n_B); //bs(verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Static.player.setZPos(newPlayerPosition);
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
				if (DEBUGPLAYERPOSITION)
					Debug.Log(newPlayerPosition);
			
				if (Mathf.Abs(horizontalAngle - horizontalHelper - playerRadius) > 2*Mathf.PI/n_B) {
				
					Parcel newCell;
						
					if (bpos > 0) {
						if (Static.rink.gameArea[lpos][bpos-1].getType() != 0 || Static.rink.gameArea[lpos][bpos-1].hasBomb()) {
							return 0.0f;
						}
						newCell = Static.rink.gameArea[lpos][bpos-1];
					} else {
						newCell = Static.rink.gameArea[lpos][n_B-1];
					}
					if (Mathf.Abs(horizontalAngle - horizontalHelper) > 2*Mathf.PI/n_B) {
						horizontalHelper -= 2*Mathf.PI/n_B;
						Static.player.setCurrentParcel(newCell);
						bpos = newCell.getBpos();
					}
<<<<<<< HEAD
					horizontalHelper -= 2*Mathf.PI/n_B;
                    player.setCurrentParcel(newCell);	
||||||| merged common ancestors
					horizontalHelper -= 2*Mathf.PI/n_B;
					Player.setCurrentParcel(newCell);	
=======
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
				}
			} else if (hDirection == 1 && Mathf.Sign(horizontalMovement) == -1) {	// Bewegungsrichtung ändert sich
				
				if (DEBUGPLAYERPOSITION)
					Debug.Log("#H4");
				
				hDirection = -1;
				horizontalHelper -=	2*Mathf.PI/(n_B);
			
<<<<<<< HEAD
				float newPlayerPosition =  1-Mathf.Abs(horizontalAngle-horizontalHelper)/(2*Mathf.PI/n_B); //bs( verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
                player.setZPos(newPlayerPosition);
||||||| merged common ancestors
				float newPlayerPosition =  1-Mathf.Abs(horizontalAngle-horizontalHelper)/(2*Mathf.PI/n_B); //bs( verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Player.setZPos( newPlayerPosition);
=======
				float newPlayerPosition =  1-Mathf.Abs(horizontalAngle-horizontalHelper)/(2*Mathf.PI/n_B); //bs(verticalAngle - verticalHelper) / (Mathf.PI/(n_L-1));
				Static.player.setZPos(newPlayerPosition);
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
				if (DEBUGPLAYERPOSITION)
					Debug.Log(newPlayerPosition);
			
				if (Mathf.Abs(horizontalAngle - horizontalHelper + playerRadius) > 2*Mathf.PI/n_B) {
					
					Parcel newCell;
					
					if (bpos < n_B-1) {
						if (Static.rink.gameArea[lpos][bpos+1].getType() != 0 || Static.rink.gameArea[lpos][bpos+1].hasBomb()) {
							return 0.0f;
						}
						newCell = Static.rink.gameArea[lpos][bpos+1];
					} else {
						if (Static.rink.gameArea[lpos][0].getType() != 0 || Static.rink.gameArea[lpos][0].hasBomb()) {
							return 0.0f;
						}
						newCell = Static.rink.gameArea[lpos][0];
					}
					if (Mathf.Abs(horizontalAngle - horizontalHelper) > 2*Mathf.PI/n_B) {
						horizontalHelper += 2*Mathf.PI/n_B;
						Static.player.setCurrentParcel(newCell);
						bpos = newCell.getBpos();
					}
<<<<<<< HEAD
					horizontalHelper += 2*Mathf.PI/n_B;
                    player.setCurrentParcel(newCell);	
||||||| merged common ancestors
					horizontalHelper += 2*Mathf.PI/n_B;
					Player.setCurrentParcel(newCell);	
=======
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
				}
			} 
		
		return m;

	}
	
	private void moveAlongEquator(float movement) {
		
		movement = movement * Mathf.Rad2Deg;
		
		// Spieler drehen
		transform.RotateAround(Vector3.zero, Vector3.forward, movement);
		
		// Kamera drehen
		cam.transform.RotateAround(Vector3.zero, Vector3.forward, movement);
		cam.transform.LookAt(Vector3.zero, Vector3.forward);
		
<<<<<<< HEAD
		camera.transform.position = new Vector3(Mathf.Cos(movement)* camera.transform.position.x - Mathf.Sin(movement) * camera.transform.position.y,
										Mathf.Sin(movement) * camera.transform.position.x + Mathf.Cos(movement) * camera.transform.position.y,
										camera.transform.position.z);
		camera.transform.LookAt(Vector3.zero, Vector3.forward);
    }
||||||| merged common ancestors
		camera.transform.position = new Vector3(Mathf.Cos(movement)* camera.transform.position.x - Mathf.Sin(movement) * camera.transform.position.y,
										Mathf.Sin(movement) * camera.transform.position.x + Mathf.Cos(movement) * camera.transform.position.y,
										camera.transform.position.z);
		camera.transform.LookAt(Vector3.zero, Vector3.forward);
	}
=======
		// Licht mitdrehen..
		sun.transform.RotateAround(Vector3.zero, Vector3.forward, movement);
		sun.transform.eulerAngles = new Vector3(0,90,0);
	}
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
	
<<<<<<< HEAD
	void OnParticleCollision(GameObject explosion) {
        player.decreaseHP();
        if (player.getHP() == 0)
        {
||||||| merged common ancestors
	void OnParticleCollision(GameObject explosion) {
		Player.decreaseHP();
		if (Player.getHP() == 0) {
=======
	/*
	void OnParticleCollision(GameObject explosion) {
		Static.player.decreaseHP();
		if (Static.player.getHP() == 0) {
>>>>>>> b2aadccf061629298696c53aaaaec5470f597779
			renderer.material.color = Color.black;
			//moveDirection = new Vector3(0, 0, 0);
			GameObject deadPlayer = GameObject.Instantiate(Static.explosionPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity) as GameObject; 
		}
	}
	*/

}
