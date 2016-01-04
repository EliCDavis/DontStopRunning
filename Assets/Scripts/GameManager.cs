using UnityEngine;
using System.Collections;

public class GameManager {

	/// <summary>
	/// Initializes a new instance of the <see cref="GameManager"/> class.
	/// Made private so classes are forced to use getInstance()
	/// </summary>
	private GameManager(){
		
	}

	static GameManager instance = null;

	/// <summary>
	/// Singleton method for ensuring theres only one game manager instance.
	/// </summary>
	/// <returns>The instance.</returns>
	public GameManager getInstance(){

		if (instance == null) {
			instance = new GameManager();
		}

		return instance;

	}

	private LevelSetting currentLoadedLevelSettings = null;

	public void loadLevel(LevelSetting settings){

		currentLoadedLevelSettings = settings;

	}

}
