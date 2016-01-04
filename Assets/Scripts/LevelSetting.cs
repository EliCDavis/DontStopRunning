using UnityEngine;
using System.Collections;


public class LevelSetting {


	Vector2 seed = Vector2.zero;


	float difficulty = 0f;


	public LevelSetting(Vector2 seed, float difficulty){

		this.seed = seed;
		this.difficulty = difficulty;
	
	}

}
