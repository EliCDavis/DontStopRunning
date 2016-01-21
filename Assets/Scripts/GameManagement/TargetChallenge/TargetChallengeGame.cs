using System.Collections.Generic;
using EliDavis.Characters.Enemy;
using EliDavis.Characters.PlayerInGameControl;
using UnityEngine;

namespace EliDavis.GameManagement.TargetChallengeMode {

	public class TargetChallengeGame {

		public virtual void startGame(){
			PlayerFactory.CreatePlayer (new Vector3(-6.5f,52,96.6f), 
			                            EliDavis.Characters.PlayerInGameControl.Weaponry.PrebuiltWeaponType.Chaingun, 
			                            EliDavis.Characters.PlayerInGameControl.Weaponry.PrebuiltWeaponType.MissileLauncher
			                            );
		}

	}

}
