using UnityEngine;
using System.Collections;
using Enemy;

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
		/// The weapon configuration that keeps up with info
		/// such as fire rate, damage, projectiles, ect.
		/// </summary>
		private WeaponConfiguration weaponConfiguration;


		/// <summary>
		/// The current heat of the weapon.
		/// At 1 the gun becomes overheated and must cool down
		/// </summary>
		private float currentHeat = 0f;


		/// <summary>
		/// The time of when we last called the fire method.
		/// </summary>
		private float timeOfLastFire = 0f;


		/// <summary>
		/// Reference to the weapon model.
		/// </summary>
		private GameObject weaponModel = null;


		/// <summary>
		/// The bullet spawn of the weapon, where we should animate 
		/// effects and raycast from.
		/// </summary>
		private Transform bulletSpawn = null;



		public Weapon(GameObject weaponObject, WeaponConfiguration weaponConfig){

			if (weaponObject == null) {
				return;
			}

			weaponConfiguration = weaponConfig;

			this.weaponModel = weaponObject;

			this.bulletSpawn = weaponModel.transform.FindChild ("BulletSpawn");

		}



		/// <summary>
		/// Performs a ray cast and instantiates effects and deals damage appropriately
		/// if a cast hits an enemy
		/// </summary>
		void castPain ()
		{

			// If we don't even have a bullet spawn to animate don't bother
			if (bulletSpawn == null) return;

			// Shoot from perspective of main camera
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			// Store the hit info
			RaycastHit hit;


			// If our raycast hit something
			if (Physics.Raycast (ray, out hit, 100)) {

				// Create a cute impact effect if we have one
				if(weaponConfiguration.impactEffect != null){

					Object.Instantiate(weaponConfiguration.impactEffect, hit.point, Quaternion.identity);

				}

				// Try grabbing the enemy instance
				TurretBehavoir enemyHit = hit.collider.gameObject.GetComponent<TurretBehavoir>();

				// Damage the enemy if there is one
				if(enemyHit != null){

					enemyHit.damage(weaponConfiguration.damagePerFire);
				
				}


			}

		}



		/// <summary>
		/// Fires the weapon if able to.
		/// </summary>
		public void fire(){


			// Update heat since last fire
			currentHeat = Mathf.Clamp( currentHeat - (Time.time - timeOfLastFire) * weaponConfiguration.cooldownRate, 0f, 1f);

			// Make sure we can fire gun //

			// Make sure we're in the correct state
			if (isOverheated ()) {
				return;
			}

			// Make sure enough time has elapsed since last fire
			if(Time.time - timeOfLastFire < weaponConfiguration.fireRate){
				return;
			}


			// Hurt whatever got in our way
			castPain ();


			// Add heat to the gun
			currentHeat += weaponConfiguration.heatPerFireIncrement;

			// Go into overheated state if we've become overheated
			if(currentHeat >= 1f){
				currentState = WeaponState.Overheated;
			}

			// Update our last firing time
			timeOfLastFire = Time.time;

			// Create Effects
			animateFire ();

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
			if(timeSinceLastFire * weaponConfiguration.cooldownRate >= 1f){

				// Change the state back to normal because it's cooled down now
				currentState = WeaponState.Normal;
				currentHeat = 0f;


				return false;

			}

			// Nothing is saying we're not overheated so we must be
			return true;

		}


		/// <summary>
		/// Adds some flare to our fire.
		/// Plays a sound and a particle emiter
		/// </summary>
		private void animateFire(){


			// If we don't even have a bullet spawn to animate don't bother
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
