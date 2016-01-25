using System.Collections.Generic;
using EliDavis.Characters;
using EliDavis.Characters.Enemy;
using EliDavis.Characters.PlayerInGameControl;
using UnityEngine;

namespace EliDavis.GameManagement.TargetChallengeMode {


	/// <summary>
	/// Contains basic logic for a target challenge game that should be overwritten
	/// for the scene at hand.
	/// 
	/// TODO overwritting unessisary for simple games of player and targets
	/// could be done by setting up a player spawn object in the scene and then
	/// just looking at previously configured mech option to spawn player there
	/// </summary>
	public abstract class TargetChallengeGame : MonoBehaviour {


		/// <summary>
		/// A list of all targets currentely not destroyed in the scene.
		/// </summary>
		private List<TargetBehavior> targetsInScene = null;


		/// <summary>
		/// How much time the player has to kill all targets for the game to be 
		/// considered a 'win'.
		/// </summary>
		protected float timeForGameModeInSeconds = 0f;


		/// <summary>
		/// The time in seconds when the game started for the player.
		/// </summary>
		private float timeGameStarted = 0f;


		/// <summary>
		/// The current state the game is in.
		/// </summary>
		private GameState currentState = GameState.HaventBegun;


		/// <summary>
		/// The number of targets destroyed since the start of the scene.
		/// </summary>
		private int targetsDestroyed = 0;


		/// <summary>
		/// The current player in the scene that's playing the game
		/// </summary>
		private PlayerBehavior player = null;


		/// <summary>
		/// When a player destroys a target that target calls this function to notify it
		/// has been destroyed.  When the targets are 0 then the game is finnished
		/// </summary>
		/// <param name="target">Target.</param>
		public void notifyDestructionOfTarget(TargetBehavior target){

			if (!targetsInScene.Contains (target)) {
				Debug.LogError ("Trying to notify destruction of a target that does not exist " +
					"in our list of targets build from the start of the start of the game");
				return;
			}

			targetsInScene.Remove (target);
			targetsDestroyed++;

			if (targetsInScene.Count == 0) {
				endGame ();
			}

		}


		/// <summary>
		/// Returns how much time is left in the game,  clamped bewteen 0 and timeForGameModeInSeconds.
		/// </summary>
		/// <returns>The time left in game.</returns>
		public float getTimeLeftInGame(){
			return Mathf.Clamp((timeGameStarted + timeForGameModeInSeconds) - Time.time, 0, timeForGameModeInSeconds); 
		}


		/// <summary>
		/// Method to be overwritten by child classes to spawn the player how they'd like to.
		/// </summary>
		protected abstract PlayerBehavior spawnPlayer();


		/// <summary>
		/// Called before when game start has been called but logic has not executed.  
		/// Intended for configuration purposes
		/// </summary>
		protected abstract void onBeforeGameStart ();


		void Start(){
			startGame ();
		}

		void Update(){

			// End the game if we're currentely playing and time ran out
			if (currentState == GameState.BeingPlayed && getTimeLeftInGame() == 0) {
				endGame ();
			}

		}

		/// <summary>
		/// Does nessesary set up in order for the game to be played.
		/// </summary>
		private void startGame(){

			onBeforeGameStart ();

			currentState = GameState.BeingPlayed;

			// Find all targets set up in the scene
			targetsInScene = new List<TargetBehavior>(GameObject.FindObjectsOfType<TargetBehavior> ()) ;

			player = spawnPlayer ();

			// Keep up with the time we started the game
			timeGameStarted = Time.time;

		}


		/// <summary>
		/// Ends the game being played and displays a screen 
		/// </summary>
		private void endGame(){
			
			currentState = GameState.Ended;
		
			if (targetsInScene.Count == 0) {

				// TODO make a winning screen displaying your time + accuracy

			} else {

				// TODO make a failure screen displaying how many targets you managed to destroy + accuracy

			}

		}


	}


}
