using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EliDavis.Characters;

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

		}

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


		List<Character> enemyList;


		private int enemiesDestroyed = 0;


		private LevelSetting currentLoadedLevelSettings = null;


		public void loadLevel(LevelSetting settings){

			clearEnemies ();
			currentLoadedLevelSettings = settings;

		}


		public LevelSetting getCurrentLoadedSettings(){

			return currentLoadedLevelSettings;

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

		}


		public int getEnemiesKilledSinceLevelStart(){

			return enemiesDestroyed;

		}


		/// <summary>
		/// Adds a enemy to the game manager to keep up with
		/// </summary>
		/// <param name="enemy">Enemy.</param>
		public void addEnemy(Character enemy){
		
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


	}

}