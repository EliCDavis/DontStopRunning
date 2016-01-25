using System.Collections.Generic;
using EliDavis.Characters;
using EliDavis.Characters.Enemy;
using EliDavis.Characters.PlayerInGameControl;
using UnityEngine;

namespace EliDavis.GameManagement.TargetChallengeMode {

	public class AncientChallengeStage : TargetChallengeGame {

		protected override PlayerBehavior spawnPlayer ()
		{
			
			GameObject playerObject = PlayerFactory.CreatePlayer (new Vector3(-6.5f,52,96.6f), 
				EliDavis.Characters.PlayerInGameControl.Weaponry.PrebuiltWeaponType.Chaingun, 
				EliDavis.Characters.PlayerInGameControl.Weaponry.PrebuiltWeaponType.MissileLauncher
			);

			return playerObject.GetComponent<PlayerBehavior> ();

		}

		protected override void onBeforeGameStart ()
		{
			timeForGameModeInSeconds = 120f;
		}

	}

}