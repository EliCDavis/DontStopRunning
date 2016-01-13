using UnityEngine;
using System.Collections;
using EliDavis.GameManagement;

namespace EliDavis.Characters {

	/// <summary>
	/// Basic Character class that anything with traditional health should probably extend from
	/// </summary>
	public class Character: MonoBehaviour {

		/// <summary>
		/// The max health a player can have at one time.
		/// </summary>
		protected float MAX_HEALTH = 100;
		
		
		/// <summary>
		/// The current health of the player.
		/// </summary>
		protected float currentHealth = 0;


		/// <summary>
		/// Sets the health parameters that the character will operate with
		/// </summary>
		/// <param name="maxhealth">Max health the character will have</param>
		/// <param name="currenthealth">The current health of the character.</param>
		public void setHealthParameters(float maxhealth, float currenthealth){

			this.MAX_HEALTH = maxhealth;
			this.currentHealth = currenthealth;

		}


		/// <summary>
		/// Inflicts a certain amount of damage to the player and
		/// performs a death check.
		/// </summary>
		/// <param name="value">Value.</param>
		public void damage(float amount){

			if (amount == 0f) {
				return;
			}
			
			currentHealth = Mathf.Clamp (currentHealth - Mathf.Abs (amount), 0, MAX_HEALTH);

			// If we're dead
			if (currentHealth == 0f) {
				GameManager.getInstance().reportKilledCharacter(this);
				OnDie ();
			} else {
				OnDamage ();
			}

		}


		/// <summary>
		/// Raises the damage event.
		/// </summary>
		protected virtual void OnDamage(){

		}


		/// <summary>
		/// If the character has died let's remove them from the scene.
		/// </summary>
		protected virtual void OnDie(){

			Destroy (gameObject);

		}

	}

}