using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

namespace PlayerInGameControl {

	public class PlayerBehavior : MonoBehaviour {

		/// <summary>
		/// The max health a player can have at one time.
		/// </summary>
		private float MAX_HEALTH = 100;


		/// <summary>
		/// The current health of the player.
		/// </summary>
		private float currentHealth = 0;


		/// <summary>
		/// The max boost a player can have.
		/// </summary>
		private float MAX_BOOST = 200;


		/// <summary>
		/// The current boost the player has.
		/// </summary>
		private float currentBoost = 0;


		/// <summary>
		/// The time that we last utilized the boost power.
		/// </summary>
		private float lastTimeUsingBoost = 0f;


		/// <summary>
		/// The canvas that keeps up the player's UI
		/// </summary>
		private GameObject playerUI = null;


		/// <summary>
		/// Inflicts a certain amount of damage to the player and
		/// performs a death check.
		/// </summary>
		/// <param name="value">Value.</param>
		public void damage(float value){
			
			if (value == 0f) {
				return;
			}
			
			currentHealth = Mathf.Clamp (currentHealth - Mathf.Abs (value), 0, MAX_HEALTH);

			// If we're dead
			if(currentHealth == 0f){

				// Disable the UI that shows things such as health
				playerUI.GetComponent<Canvas>().enabled = false;

				// Display the on death UI
				transform.FindChild("OnDeathScreen").GetComponent<Canvas>().enabled = true;

				// Disable the player from moving around
				setPlayerControl(false);
			
			} 
			
		}


		/// <summary>
		/// Things such as powers requires boost power to be executed.
		/// When the player utilizes these powers it requests boost power
		/// If succesful the player can executed requested powers.
		/// </summary>
		/// <returns><c>true</c>There is enough boost and it has been subtracted<c>false</c> otherwise.</returns>
		/// <param name="amount">Amount of boost power you'd like to utilize</param>
		public bool requestBoostPower(float amount){

			if(currentBoost - amount > 0){

				currentBoost -= amount;
				lastTimeUsingBoost = Time.time;

				return true;

			}


			return false;

		}


		/// <summary>
		/// Modifies whether or not the player can give input to the character and have
		/// it respond.  This means moving the character around the map and executing powers.
		/// </summary>
		/// <param name="canBeControlled">If set to <c>true</c> can be controlled.</param>
		private void setPlayerControl(bool canBeControlled){

			gameObject.GetComponent<PlayerPowerBehavior> ().enabled = canBeControlled;
			gameObject.GetComponent<RigidbodyFirstPersonController> ().enabled = canBeControlled;
			gameObject.GetComponent<WeaponBehavoir> ().enabled = canBeControlled;

			// Lock the mouse to the center of the screen if the player is going to play
			if (canBeControlled) {
				Cursor.lockState = CursorLockMode.Locked;
			} 

			// Unlock the mouse so the player can move it freely around the screen since they can't control the character
			else 
			{
				Cursor.lockState = CursorLockMode.None;
			}


		}


		/// <summary>
		/// Updates the UI to make sure that it's in sync with the internal values
		/// such as health that need to be displayed
		/// </summary>
		private void updateUI(){
			
			playerUI.transform.FindChild ("Health").GetComponent<Slider>().value = currentHealth / MAX_HEALTH;
			playerUI.transform.FindChild ("Booster").GetComponent<Slider>().value = currentBoost / MAX_BOOST;
			
		}


		/// <summary>
		/// The player's boost power is constantly being replinished
		/// A certain period of time must have no use of boost before it can 
		/// begin replenishing.
		/// </summary>
		private void boostRecoveryUpdate(){
			
			float timeAfterUseBeforeRecovery = 1f;

			// If enough time hasn't elapsed then let's not bother increasing our boost meter
			if(Time.time - lastTimeUsingBoost < timeAfterUseBeforeRecovery){
				return;
			}

			float boostRecoveryRate = 25f;
			
			currentBoost = Mathf.Clamp (currentBoost + (boostRecoveryRate*Time.deltaTime), 0, MAX_BOOST);
			
		}


		// Use this for initialization
		void Start () {

			// Set the stats the be maxed out
			currentHealth = MAX_HEALTH;
			currentBoost = MAX_BOOST;

			playerUI = transform.FindChild ("PlayerUI").gameObject;

		}


		// Update is called once per frame
		void Update () {

			boostRecoveryUpdate ();
			updateUI ();

		}

	}

}
