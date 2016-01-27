using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EliDavis.Characters;
using EliDavis.Characters.PlayerInGameControl;

namespace EliDavis.GameManagement {

	/// <summary>
	/// The ultimate game manager multiple different systems in the codebase will
	/// call for reference to what's going on currentely inside of the scene.
	/// This includes information such as what enemies are alive, or the status
	/// of the player.
	/// </summary>
	public class GameManager {

		/// <summary>
		/// Initializes a new instance of the <see cref="GameManager"/> class.
		/// Made private so classes are forced to use getInstance()
		/// </summary>
		private GameManager(){

			enemyList = new List<Character>();
			friendlyList = new List<Character> ();

		}


		/// <summary>
		/// The only instance that should every exhist of the GameManager.
		/// </summary>
		static GameManager instance = null;


		/// <summary>
		/// Singleton method for ensuring theres only one game manager instance.
		/// </summary>
		/// <returns>The instance.</returns>
		public static GameManager getInstance(){

			if (instance == null) {
				instance = new GameManager();
			}

			return instance;

		}


		private List<Character> enemyList;


		private List<Character> friendlyList;


		private MechConfiguration playerConfiguration;


		private int enemiesDestroyed = 0;


		private int friendlysDestroyed = 0;


		private LevelSetting currentLoadedLevelSettings = null;


		public void loadLevel(LevelSetting settings){

			clearEnemies ();
			currentLoadedLevelSettings = settings;

		}


		public LevelSetting getCurrentLoadedSettings(){

			return currentLoadedLevelSettings;

		}


		public void setPlayerMechConfiguration(MechConfiguration mechConfiguration){

			this.playerConfiguration = mechConfiguration;

		}


		/// <summary>
		/// Returns every instance of a character currentely loaded
		/// in the scene, friendly or enemy to the player.
		/// This also includes the player if their not dead
		/// </summary>
		/// <returns>The all characters in scene.</returns>
		public Character[] getAllCharactersInScene ()
		{
			List<Character> allCharacters = new List<Character> ();

			allCharacters.AddRange (enemyList);
			allCharacters.AddRange (friendlyList);

			return allCharacters.ToArray ();
		}


		/// <summary>
		/// Updates the gamemanger that the character is now dead
		/// </summary>
		/// <param name="character">Character.</param>
		public void reportKilledCharacter(Character character){

			if (enemyList.Contains (character)) {
				enemyList.Remove(character);
				enemiesDestroyed ++;
			}

			if (friendlyList.Contains (character)) {
				friendlyList.Remove(character);
				friendlysDestroyed ++;
			}

		}


		/// <summary>
		/// Returns the count of the number of reported killed 
		/// characters that turned out to be enemies since the level 
		/// was loaded (or last cleared)
		/// </summary>
		/// <returns>The enemies killed since level start.</returns>
		public int getEnemiesKilledSinceLevelStart(){

			return enemiesDestroyed;

		}


		/// <summary>
		/// Adds a enemy to the game manager to keep up with
		/// </summary>
		/// <param name="enemy">Enemy.</param>
		public void addEnemy(Character enemy){
		
			if (enemy == null) {
				return;
			}

			enemyList.Add (enemy);
		
		}


		/// <summary>
		/// This grabs all enemies that the game manager knows about 
		/// </summary>
		/// <returns>All enemies currentely alive in the scene.</returns>
		public Character[] getEnemies(){

			return enemyList.ToArray ();

		}


		/// <summary>
		/// Destroys all enemies from the scene
		/// </summary>
		public void destroyAllEnemies(){

			for(int i = 0; i < enemyList.Count; i ++){

				Object.Destroy(enemyList[i].gameObject);

			}

			clearEnemies ();

		}


		/// <summary>
		/// Clears the enemy list, this does not remove enemies
		/// from the scene.
		/// </summary>
		private void clearEnemies(){
			
			enemyList.Clear ();
			enemiesDestroyed = 0;
			
		}


		/// <summary>
		/// Adds a friendly character that will fight with the player
		/// </summary>
		/// <param name="friendly">Friendly.</param>
		public void addFriendly(Character friendly){

			if (friendly == null) {
				return;
			}

			friendlyList.Add (friendly);

		}


		public Character[] getFriendlyCharacters(){

			return friendlyList.ToArray ();

		}


	}

}