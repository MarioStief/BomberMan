using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AssemblyCSharp
{
	public class Player
	{
		private Parcel currentCell;	// current Parcel
		private float xpos, zpos;	// Player's position in the current Parcel
		
		private const float MAXSPEED = 0.7f;
		private const float MINSPEED = 0.3f;
		private const float MINDELAY = 0.04f;
		private const float MAXDELAY = 0.28f;
		private const int MAXBOMBS = 10;
		private const int MAXFLAMEPOWER = 10;
		private bool SUPERBOMB = false;
		
		private int bombs = 1;
		private int bombsActive = 0;

		private int triggerbombs = 0;
		private int triggerbombsActive = 0;

		private int contactmines = 0;
		private int contactminesActive = 0;

		private int flamePower = 1;
		private float speed = 0.38f; // Startet auf Stufe 3 von 11
		private float delay = 0.2f; // Startet auf Stufe 4 von 12
		
		private bool dead = false;
		
		private Texture2D[] icons = new Texture2D[6];
		private string[] iconText = new string[6];
		
		private Dictionary<Parcel,GameObject> triggerbombList = new Dictionary<Parcel,GameObject>();
		private Dictionary<NetworkPlayer,int> playerWins = new Dictionary<NetworkPlayer, int>();
		private List<NetworkPlayer> playerAlive = new List<NetworkPlayer>();
		
		public void updateMenuStats() {
			
			/* returns integer array of player attributes
			 * 
			 * bombs = maximum Bombs, the player pocesses
			 *         related icon: Static.bombIconPrefab
			 * speed = player movement speed in ms
			 * 	       related icon: Static.playerSpeedIconPrefab
			 * flamepower = the size of the explosion
			 *         related icon: Static.flameIconPrefab
			 *         icon changes automatically if flame power reaches max and backwards
			 * delay = the shorter the delay the faster the explosion spreads in ms
			 *         related icon: Static.delaySpeedIconPrefab
			 * superbomb = 1 for true, 0 for false
			 *         related icon: Static.superBombIconPrefab
			 *         icon automatically when superbomb is collected and backwards
			 * extras = collected extra which uses the SHIFT key
			 *         related icon: Static.extraIconPrefab
			 *         icon is empty transparent at start and adapts
			 * 
			 * NOTE: icons are 32x32 and transparent
			 */

			icons = new Texture2D[] {
				Static.dynamiteIconPrefab,
				Static.playerSpeedIconPrefab,
				Static.flameIconPrefab,
				Static.delaySpeedIconPrefab,
				Static.extraIconPrefab,
				SUPERBOMB ? Static.superBombIconPrefab : null
			};
			
			string extra = "";
			if (triggerbombs > 0)
				extra = triggerbombs.ToString() + "x";
			if (contactmines > 0)
				extra = contactmines.ToString() + "x";
			
			iconText = new string[]{
				bombs + "x",
				(int) Mathf.Round(speed*1000) + " m/h",
				flamePower.ToString(),
				(int) Mathf.Round(delay*1000) + " ms",
				extra, ""
			};
		}
		
		public string[] getIconText() {
			return iconText;
		}

		public Texture2D[] getIcons() {
			return icons;
		}

		public void addTriggerBomb(GameObject bomb, Parcel cell) {
			triggerbombsActive++;
			triggerbombList.Add(cell, bomb);
		}
		
		public void powerupCollected(PowerupType type)
		{
			if (type == PowerupType.BOMB_UP) {
				if (bombs < MAXBOMBS) {
					bombs++;
				}
			} else if (type == PowerupType.BOMB_DOWN) {
				if (bombs > 1) {
					bombs--;
				}
			} else if (type == PowerupType.FLAME_UP) {
				if (flamePower < MAXFLAMEPOWER) {
					flamePower++;
				}
				if (flamePower < MAXFLAMEPOWER) {
					Static.setGoldenFlame(true);
				}
			} else if (type == PowerupType.FLAME_DOWN) {
				if (flamePower == MAXFLAMEPOWER) {
					Static.setGoldenFlame(false);
				}
				if (flamePower > 1) {
					flamePower--;
				}
			} else if (type == PowerupType.PLAYER_SPEED_UP) {
				if (speed < MAXSPEED) {
					speed += 0.04f;
				}
			} else if (type == PowerupType.PLAYER_SPEED_DOWN) {
				if (speed > MINSPEED) {
					speed -= 0.04f;
				}
			} else if (type == PowerupType.DELAY_SPEED_UP) {
				if (delay > MINDELAY) {
					delay -= 0.02f;
				}
			} else if (type == PowerupType.DELAY_SPEED_DOWN) {
				if (delay < MAXDELAY) {
					delay += 0.02f;
				}
			} else if (type == PowerupType.GOLDEN_FLAME) {
				flamePower = MAXFLAMEPOWER;
				Static.setGoldenFlame(true);
			} else if (type == PowerupType.SUPERBOMB) {
				SUPERBOMB = true;
				Static.setSuperbomb(true);
			} else if (type == PowerupType.TRIGGERBOMB) {
				if (triggerbombs < 3) {
					triggerbombs++;
					contactmines = 0;
					Static.setExtra(1);
				}
			} else if (type == PowerupType.CONTACTMINE) {
				if (contactmines < 3) {
					contactmines++;
					triggerbombs = 0;
					Static.setExtra(2);
				}
			}
			Debug.Log("dynamite: " + bombs + ", flamePower: " + flamePower + ", speed: " + speed*1000 + " ms, delay: " + delay*1000 + " ms");
			updateMenuStats();
		}
		
		// return "extra"
		public int addBomb() {
			if (triggerbombs > triggerbombsActive) {
				return 1;
			} else if (bombsActive < bombs) {
				bombsActive++;
				return 0;
			}
			return -1;
		}
		
		public bool addContactMine() {
			if (contactmines > contactminesActive) {
				contactminesActive++;
				return true;
			}
			return false;
		}
		
		public void removeContactMine() {
			contactminesActive--;
		}
		
		public void releaseTriggerBombs() {
			Dictionary<Parcel,GameObject> triggerBombs = new Dictionary<Parcel,GameObject>(triggerbombList);
			foreach (var bomb in triggerBombs) {
				bomb.Value.networkView.RPC("startExplosion", RPCMode.All, true);
				removeTriggerBomb(bomb.Key);
			}
		}
		
		public void removeTriggerBomb(Parcel cell) {
			triggerbombsActive--;
			triggerbombList.Remove(cell);
		}
		
		public Dictionary<Parcel,GameObject> getTriggerBombs() {
			return triggerbombList;
		}
		
		public void setXPos(float x){
			if (x > 1) xpos = 1;
			xpos = x;	
		}
		
		public float getXPos(){
			return xpos;	
		}
		
		public void setZPos(float z){
			if (zpos > 1) zpos = 1;
			zpos = z;	
		}
		
		public float getZPos(){
			return zpos;	
		}
		
		public void removeBomb() {
			bombsActive--;
		}

		public int getFlamePower() {
			return flamePower;
		}

		public int getBombs() {
			return bombs;
		}

		public float getSpeed() {
			return speed;
		}
		
		public void setDead(bool d, NetworkView nv) {
			dead = d;
			if (d) {
				Static.menuHandler.playSound(Static.playerDeadSoundEffect, false);
				// Verteile Powerups über das Spielfeld
				List<Parcel> parcelPool = new List<Parcel>();
				Parcel[][] gameArea = Static.gameArea;
				for (int i = 0; i < gameArea.Length; i++) {
					for (int j = 0; j < gameArea[i].Length; j++) {
						if (gameArea[i][j].getType() == 0 && !gameArea[i][j].hasExplosion()) {
							parcelPool.Add(gameArea[i][j]);
						}
					}
				}
				parcelPool = shuffleList(parcelPool);
				if (SUPERBOMB) {
					nv.RPC("addPowerup", RPCMode.Others, parcelPool[0].getLpos(), parcelPool[0].getBpos(), (int)PowerupType.SUPERBOMB);
					parcelPool[0].addPowerup(new Powerup(PowerupType.SUPERBOMB));
					parcelPool.RemoveAt(0);
					SUPERBOMB = false;
					Static.setSuperbomb(false);
					updateMenuStats();
				}
				if (flamePower == MAXFLAMEPOWER && parcelPool.Count > 0) {
					nv.RPC("addPowerup", RPCMode.Others, parcelPool[0].getLpos(), parcelPool[0].getBpos(), (int)PowerupType.GOLDEN_FLAME);
					parcelPool[0].addPowerup(new Powerup(PowerupType.GOLDEN_FLAME));
					parcelPool.RemoveAt(0);
					bombs = 1;
					Static.setGoldenFlame(false);
					updateMenuStats();
				}
				while (triggerbombs > 0) {
					nv.RPC("addPowerup", RPCMode.Others, parcelPool[0].getLpos(), parcelPool[0].getBpos(), (int)PowerupType.TRIGGERBOMB);
					parcelPool[0].addPowerup(new Powerup(PowerupType.TRIGGERBOMB));
					parcelPool.RemoveAt(0);
					triggerbombs--;
					Static.setExtra(0);
					updateMenuStats();
				}
				while (contactmines > 0) {
					nv.RPC("addPowerup", RPCMode.Others, parcelPool[0].getLpos(), parcelPool[0].getBpos(), (int)PowerupType.CONTACTMINE);
					parcelPool[0].addPowerup(new Powerup(PowerupType.CONTACTMINE));
					parcelPool.RemoveAt(0);
					contactmines--;
					Static.setExtra(0);
					updateMenuStats();
				}
				while (bombs > 1 && parcelPool.Count > 0) {
					nv.RPC("addPowerup", RPCMode.Others, parcelPool[0].getLpos(), parcelPool[0].getBpos(), (int)PowerupType.BOMB_UP);
					parcelPool[0].addPowerup(new Powerup(PowerupType.BOMB_UP));
					parcelPool.RemoveAt(0);
					bombs--;
				}
				while (flamePower > 1 && parcelPool.Count > 0) {
					nv.RPC("addPowerup", RPCMode.Others, parcelPool[0].getLpos(), parcelPool[0].getBpos(), (int)PowerupType.FLAME_UP);
					parcelPool[0].addPowerup(new Powerup(PowerupType.FLAME_UP));
					parcelPool.RemoveAt(0);
					flamePower--;
				}
				while (speed > 0.38f && parcelPool.Count > 0) {
					nv.RPC("addPowerup", RPCMode.Others, parcelPool[0].getLpos(), parcelPool[0].getBpos(), (int)PowerupType.PLAYER_SPEED_UP);
					parcelPool[0].addPowerup(new Powerup(PowerupType.PLAYER_SPEED_UP));
					parcelPool.RemoveAt(0);
					speed -= 0.04f;
				}
				while (delay < 0.2f && parcelPool.Count > 0) {
					nv.RPC("addPowerup", RPCMode.Others, parcelPool[0].getLpos(), parcelPool[0].getBpos(), (int)PowerupType.DELAY_SPEED_UP);
					parcelPool[0].addPowerup(new Powerup(PowerupType.DELAY_SPEED_UP));
					parcelPool.RemoveAt(0);
					delay += 0.02f;
				}
				// update Menu
				updateMenuStats();
			}
		}
		
		public bool isDead() {
			return dead;
		}
		
		public void setCurrentParcel(Parcel parcel){
			currentCell = parcel;	
		}
		
		public Parcel getCurrentParcel(){
			return currentCell;	
		}
		
		private List<Parcel> shuffleList(List<Parcel> sortedList)
		{
			List<Parcel> randomList = new List<Parcel>();
			
		    int randomIndex = 0;
		    while (sortedList.Count > 0)
		    {
		    	randomIndex = Random.Range(0, sortedList.Count);
		    	randomList.Add(sortedList[randomIndex]);
				sortedList.RemoveAt(randomIndex);
			}
			return randomList;
		}
		
		public bool getSuperbomb() {
			return SUPERBOMB;
		}

		public bool hasTriggerbomb() {
			return (triggerbombs > 0);
		}

		public bool hasContactMine() {
			return (contactmines > 0);
		}

		public float getDelay() {
			return delay;
		}
		
		public int getMaxFlamePower() {
			return MAXFLAMEPOWER;
		}
		
		public void resetStats() {
			SUPERBOMB = false;
			bombs = 1;
			bombsActive = 0;
			triggerbombs = 0;
			triggerbombsActive = 0;
			contactmines = 0;
			contactminesActive = 0;
			flamePower = 1;
			speed = 0.38f;
			delay = 0.2f;
			dead = false;
		}
		
		public void setPlayers(List<NetworkPlayer> players) {
			playerAlive = players;
			foreach (NetworkPlayer p in players)
				if (!playerWins.ContainsKey(p))
					playerWins.Add(p,0);
		}
		public void resetPlayers() {
			playerWins.Clear();
		}
		
		public void imOut(NetworkPlayer p) {
			if (playerAlive.Contains(p))
				playerAlive.Remove(p);
			// round ended
			if (Network.isServer && playerAlive.Count <= 1) {
				Static.menuHandler.Invoke("startRound", 0.3f);
			}
		}
		public void setWinner(NetworkPlayer p) {
			playerWins[p]++;
		}
		
		public List<NetworkPlayer> getPlayersAlive() {
			return playerAlive;
		}
		
		public int getWinAmount(NetworkPlayer p) {
			return playerWins[p];
		}
		
		public List<string> getWins() {
			List<KeyValuePair<NetworkPlayer,int>> pl = playerWins.ToList();
			pl.Sort((firstPair,nextPair) => {
        		return nextPair.Value.CompareTo(firstPair.Value);
    		});
			List<string> re = new List<string>();
			foreach (var it in pl)
				if (Menu.getPlayerNick(it.Key) != "")
					re.Add(Menu.getPlayerNick(it.Key) + ": " + it.Value);
			return re;
		}
	}
}

