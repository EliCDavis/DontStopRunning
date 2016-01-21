using UnityEngine;
using System.Collections;

namespace EliDavis.GameManagement.TargetChallengeMode {

	public class GameInit : MonoBehaviour {

		// Use this for initialization
		void Start () {
			TargetChallengeGame game = new TargetChallengeGame ();
			game.startGame ();
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}

}