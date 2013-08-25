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
		
		private const float MAXSPEED = 0.8f;
		private const float MINDELAY = 0.04f;
		private const float MAXDELAY = 0.28f;
		private const int MAXFLAMEPOWER = 10;
		private bool SUPERBOMB = false;
		
		private int bombs = 1;
		private int bombsActive = 0;

		private int triggerbombs = 0;
		private int triggerbombsActive = 0;

		private int contactmines = 0;
		private int contactminesActive = 0;

		private int flamePower = 1;
		private float speed = 0.4f;
		private float delay = 0.2f;
		
		private bool dead = false;
		
		private UnityEngine.Object[] icons = new UnityEngine.Object[6];
		private string[] iconText = new string[4];
		
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

			icons[0] = Static.bombIconPrefab;
			icons[1] = Static.playerSpeedIconPrefab;
			icons[2] = Static.flameIconPrefab;
			icons[3] = Static.delaySpeedIconPrefab;
			icons[4] = Static.superBombIconPrefab;
			icons[5] = Static.extraIconPrefab;
			
			//int extra = TRIGGERBOMB ? 1 : 0;
			//String[] stats = {bombs.ToString(), ((int) speed*1000).ToString(), flamePower.ToString(), ((int) delay*1000).ToString(), SUPERBOMB ? 1 : 0, extra};
			string[] iconText = {
				bombs.ToString(),
				((int) speed*1000).ToString() + " ms",
				flamePower.ToString(),
				((int) delay*1000).ToString() + " ms"
			};
			
			// UPDATE MENU BAR
			// SomeStrangeMenu.updateStats(stats);
			// SomeStrangeMenu.updatePrefabs(prefabs);
		}
		
		public string[] getIconText() {
			return iconText;
		}

		public UnityEngine.Object[] getIcons() {
			return icons;
		}

		public void addTriggerBomb(GameObject bomb, Parcel cell) {
			triggerbombsActive++;
			triggerbombList.Add(cell, bomb);
		}
		
		public void powerupCollected(PowerupType type)
		{
			if (type == PowerupType.BOMB_UP) {
				bombs++;
			} else if (type == PowerupType.BOMB_DOWN) {
				if (bombs > 1) {
					bombs--;
				}
			} else if (type == PowerupType.FLAME_UP) {
				if (flamePower < MAXFLAMEPOWER) {
					flamePower++;
				} else {
					Static.setGoldenFlame(true);
					updateMenuStats();
				}
			} else if (type == PowerupType.FLAME_DOWN) {
				if (flamePower == MAXFLAMEPOWER) {
					Static.setGoldenFlame(false);
					updateMenuStats();
				}
				if (flamePower > 1) {
					flamePower--;
				}
			} else if (type == PowerupType.PLAYER_SPEED_UP) {
				if (speed < MAXSPEED) {
					speed += 0.05f;
				}
			} else if (type == PowerupType.PLAYER_SPEED_DOWN) {
				if (speed > 1.0f) {
					speed -= 0.05f;
				}
			} else if (type == PowerupType.DELAY_SPEED_UP) {
				if (delay > MINDELAY) {
					delay -= 0.02f;
				}
			} else if (type == PowerupType.DELAY_SPEED_DOWN) {
				if (delay < MAXDELAY) {
					speed += 0.02f;
				}
			} else if (type == PowerupType.GOLDEN_FLAME) {
				flamePower = MAXFLAMEPOWER;
				Static.setGoldenFlame(true);
				updateMenuStats();
			} else if (type == PowerupType.SUPERBOMB) {
				SUPERBOMB = true;
				Static.setSuperbomb(true);
				updateMenuStats();
			} else if (type == PowerupType.TRIGGERBOMB) {
				if (triggerbombs < 3) {
					triggerbombs++;
					contactmines = 0;
					Static.setExtra(1);
					updateMenuStats();
				}
			} else if (type == PowerupType.CONTACTMINE) {
				if (contactmines < 3) {
					contactmines++;
					triggerbombs = 0;
					Static.setExtra(2);
					updateMenuStats();
				}
			}
			Debug.Log("bombs: " + bombs + ", flamePower: " + flamePower + ", speed: " + speed*1000 + " ms, delay: " + delay*1000 + " ms");
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
		
		/*
		public void addBomb() {
			if (triggerbombs > triggerbombsActive) {
				triggerbombsActive++;
				addTriggerBomb(currentCell);
				Explosion.createExplosionOnCell(currentCell, Static.player.getFlamePower(), Static.player.getDelay(), Static.player.getSuperbomb(), 1, true, true);
			} else if (bombsActive < bombs) {
				bombsActive++;
				Explosion.createExplosionOnCell(currentCell, Static.player.getFlamePower(), Static.player.getDelay(), Static.player.getSuperbomb(), 0, true, true);
			}
		}
		*/
		
		/*public void addContactMine() {
			if (contactmines > contactminesActive) {
				contactminesActive++;
				Explosion.createExplosionOnCell(currentCell, Static.player.getFlamePower(), Static.player.getDelay(), Static.player.getSuperbomb(), 2, true, true);
			}
		}*/
		
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
			//Debug.Log ("Bombs: " + bombsActive + "/" + bombs);
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
						if (gameArea[i][j].getType() == 0) {
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
				while (speed > 0.4f && parcelPool.Count > 0) {
					nv.RPC("addPowerup", RPCMode.Others, parcelPool[0].getLpos(), parcelPool[0].getBpos(), (int)PowerupType.PLAYER_SPEED_UP);
					parcelPool[0].addPowerup(new Powerup(PowerupType.PLAYER_SPEED_UP));
					parcelPool.RemoveAt(0);
					speed -= 0.05f;
				}
				while (delay < 0.2f && parcelPool.Count > 0) {
					nv.RPC("addPowerup", RPCMode.Others, parcelPool[0].getLpos(), parcelPool[0].getBpos(), (int)PowerupType.DELAY_SPEED_UP);
					parcelPool[0].addPowerup(new Powerup(PowerupType.DELAY_SPEED_UP));
					parcelPool.RemoveAt(0);
					delay += 0.02f;
				}
			}
		}
		
		public bool isDead() {
			return dead;
		}
		
		public void setCurrentParcel(Parcel parcel){
			
			/*
			if (currentCell != null){
				currentCell.hightlightColor(false);	
			}
			*/
			
			currentCell = parcel;	
			
			//currentCell.setColor(Color.cyan);
			//currentCell.hightlightColor(true);
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
			speed = 0.4f;
			delay = 0.2f;
			dead = false;
		}
		
		public void setPlayers(List<NetworkPlayer> players) {
			playerAlive = players;
			if (playerWins.Count == 0)
				foreach (NetworkPlayer p in players)
					playerWins.Add(p,0);
		}
		
		public void imOut(NetworkPlayer p) {
			playerAlive.Remove(p);
			// round ended
			if (playerAlive.Count <= 1) {
				if (playerAlive.Count == 1) {
					playerWins[playerAlive[0]]++;
					Static.menuHandler.incomingChatMessage(Menu.getPlayerNick(playerAlive[0]) + " has won this round!");
				}
				Static.menuHandler.Invoke("startRound", 5);
			}
		}
		
		public List<NetworkPlayer> getPlayersAlive() {
			return playerAlive;
		}
		
		public List<string> getWins() {
			List<KeyValuePair<NetworkPlayer,int>> pl = playerWins.ToList();
			pl.Sort((firstPair,nextPair) => {
        		return nextPair.Value.CompareTo(firstPair.Value);
    		});
			List<string> re = new List<string>();
			foreach (var it in pl)
				re.Add(Menu.getPlayerNick(it.Key) + ": " + it.Value);
			return re;
		}
		
		public void resetGame() {
			resetStats();
		}
	}
}

