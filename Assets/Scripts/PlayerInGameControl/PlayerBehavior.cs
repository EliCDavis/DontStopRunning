using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
		private float MAX_BOOST = 100;


		/// <summary>
		/// The current boost the player has.
		/// </summary>
		public float currentBoost = 0;


		/// <summary>
		/// The canvas that keeps up the player's UI
		/// </summary>
		private GameObject playerUI = null;


		public void damage(float value){
			
			if (value == 0f) {
				return;
			}
			
			currentHealth = Mathf.Clamp (currentHealth - Mathf.Abs (value), 0, MAX_HEALTH);
			updateUI ();
			
		}
		
		
		public void updateUI(){
			
			playerUI.transform.FindChild ("Health").GetComponent<Slider>().value = currentHealth / MAX_HEALTH;
			playerUI.transform.FindChild ("Booster").GetComponent<Slider>().value = currentBoost / MAX_BOOST;
			
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
				updateUI();

				return true;

			}

			return false;

		}


		private void boostRecoveryUpdate(){
			
			float boostRecoveryRate = 20f;
			
			currentBoost = Mathf.Clamp (currentBoost + (boostRecoveryRate*Time.deltaTime), 0, MAX_BOOST);
			updateUI ();
			
		}


		// Use this for initialization
		void Start () {
			currentHealth = MAX_HEALTH;
			//currentBoost = MAX_BOOST;
			playerUI = transform.FindChild ("PlayerUI").gameObject;
			updateUI ();
		}


		// Update is called once per frame
		void Update () {

			boostRecoveryUpdate ();

		}

	}

}
