using UnityEngine;
using System.Collections;
using Audio;

namespace PlayerInGameControl{


	/// <summary>
	/// Weapon class representing a player's mech's weapon such as a missle launcher
	/// or chain gun that has cooldown times, fire rates, and animations
	/// </summary>
	public class Weapon  {


		enum WeaponState {

			/// <summary>
			/// Normal State is where the weapon is most free.
			/// It can fire and operate normally
			/// </summary>
			Normal,

			/// <summary>
			/// The weapon is overheated, we can not fire it
			/// until it has cooled off some.
			/// </summary>
			Overheated
		
		}


		/// <summary>
		/// The current state of the weapon which dictates some 
		/// of the actions is can perform.
		/// </summary>
		private WeaponState currentState = WeaponState.Normal;


		/// <summary>
		/// The current heat of the weapon.
		/// At 1 the gun becomes overheated and must cool down
		/// </summary>
		private float currentHeat = 0f;


		/// <summary>
		/// How much heat is added to the weapon when it is used
		/// </summary>
		private float heatPerFireIncrement = 0.05f;


		/// <summary>
		/// The percentage of cooldown we receive per second.
		/// Ex. If the rate is .25, in 4 seconds we will completely
		/// cool down from overheating.
		/// </summary>
		private float cooldownRate = 0.25f;


		/// <summary>
		/// The fire rate of the weapon.
		/// Cooldown time before next fire can happen
		/// </summary>
		private float fireRate = 0.05f;


		/// <summary>
		/// The time of when we last called the fire method.
		/// </summary>
		private float timeOfLastFire = 0f;


		/// <summary>
		/// Reference to the weapon model.
		/// </summary>
		private GameObject weaponModel = null;


		public Weapon(GameObject weaponModel){

			this.weaponModel = weaponModel;

		}


		public void fire(){


			// Update heat since last fire
			currentHeat = Mathf.Clamp( currentHeat - (Time.time - timeOfLastFire) * cooldownRate, 0f, 1f);

			// Make sure we can fire gun //

			// Make sure we're in the correct state
			if (isOverheated ()) {
				return;
			}

			// Make sure enough time has elapsed since last fire
			if(Time.time - timeOfLastFire < fireRate){
				return;
			}


			// Hurt whatever got in our way


			// Add heat to the gun
			currentHeat += heatPerFireIncrement;

			// Go into overheated state if we've become overheated
			if(currentHeat >= 1f){
				currentState = WeaponState.Overheated;
			}

			// Update our last firing time
			timeOfLastFire = Time.time;

			// Create Effects
			animateFire ();

			// Create hit effect at raycasted collision

		}


		/// <summary>
		/// Checks to see if the weapon is actually overheated or
		/// if it has cooled down 
		/// </summary>
		/// <returns><c>true</c>, if overheated <c>false</c> otherwise.</returns>
		private bool isOverheated(){


			// If we're not overheated 
			if(currentState != WeaponState.Overheated) return false;
			

			// If we are overheated let's check if we're still overheated
			float timeSinceLastFire = Time.time - timeOfLastFire;


			// Check if enough time has passed to consider completely cooled down.
			if(timeSinceLastFire * cooldownRate >= 1f){

				// Change the state back to normal because it's cooled down now
				currentState = WeaponState.Normal;
				currentHeat = 0f;


				return false;

			}

			// Nothing is saying we're not overheated so we must be
			return true;

		}



		private void animateFire(){


			// If we don't even have a weapon to animate don't bother
			if (weaponModel == null) return;

			// Get a reference to where the bullet spawn
			Transform bulletSpawn = weaponModel.transform.FindChild ("BulletSpawn");

			// If there is none something is wrong so don't bother
			if (bulletSpawn == null) return;


			// Get a reference to the particle system
			ParticleSystem particleSystem = bulletSpawn.GetComponent<ParticleSystem> ();

			// Only play flash if able to find a particle system
			if (particleSystem != null) {

				particleSystem.Play ();
			
			}


			// Grab a reference to the audio source
			AudioSource audioSource = bulletSpawn.GetComponent<AudioSource>();

			// Only play sound effect if theres an audio source
			if(audioSource != null){
				audioSource.Play();
			}

		}


	}

}
