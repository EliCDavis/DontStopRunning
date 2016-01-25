using UnityEngine;
using System.Collections;

namespace EliDavis.GameManagement.TargetChallengeMode {

	public class TargetBehavior : EliDavis.Characters.Character {

		TargetChallengeGame gameBeingPlayed = null;

		public void setGameBeingPlayed(TargetChallengeGame game){
			this.gameBeingPlayed = game;
		}

		protected override void OnDamage ()
		{

		}

		protected override void OnDie ()
		{
			gameBeingPlayed.notifyDestructionOfTarget (this);
		}

	}

}