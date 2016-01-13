using UnityEngine;
using System.Collections;

namespace EliDavis.GameManagement{

	public class LevelSetting {

		Vector2 seed = Vector2.zero;

		public Vector2 Seed {
			get {
				return seed;
			}
		}

		float difficulty = 0f;

		public float Difficulty {
			get {
				return difficulty;
			}
		}

		public LevelSetting(Vector2 seed, float difficulty){

			this.seed = seed;
			this.difficulty = difficulty;
		
		}

	}

}