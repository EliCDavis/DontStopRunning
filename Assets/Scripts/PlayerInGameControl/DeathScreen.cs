using UnityEngine;
using System.Collections;

namespace PlayerInGameControl {

	public class DeathScreen : MonoBehaviour {

		public void playAgain(){
			Application.LoadLevel(Application.loadedLevel);
		}

		public void goToMainMenu(){

		}

	}

}