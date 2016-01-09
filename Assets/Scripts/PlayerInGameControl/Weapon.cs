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


		/// <summary>
		/// Initializes a new instance of the <see cref="PlayerInGameControl.Weapon"/> class.
		/// </summary>
		/// <param name="weaponObject">Weapon object, 3D model with a bullet spawn object as a child</param>
		/// <param name="weaponConfig">Weapon config, different settings we want the weapon to operate under</param>
		public Weapon(GameObject weaponObject, WeaponConfiguration weaponConfig){

			// Set the weapon configurations
			weaponConfiguration = weaponConfig;
			
			// Don't bother trying to get a bullet spawn from a null object
			if (weaponObject == null) {
				return;
			}

			// Hook up bullet spawn and model 
			this.weaponModel = weaponObject;
			this.bulletSpawn = weaponModel.transform.FindChild ("BulletSpawn");

		}


		/// <summary>
		/// Gets the current heat that the weapon has accumulated
		/// </summary>
		/// <returns>The current heat.</returns>
		public float getCurrentHeat(){
			return Mathf.Clamp01(currentHeat - ((Time.time - timeOfLastFire) * weaponConfiguration.cooldownRate));
		}


		/// <summary>
		/// Fires the weapon if able to.
		/// </summary>
		public void fire(){

			// Make sure we can fire gun //

			// Make sure we're in the correct state
			if (isOverheated ()) {
				return;
			}

			// Make sure enough time has elapsed since last fire
			if(Time.time - timeOfLastFire < weaponConfiguration.fireRate){
				return;
			}

			// Update heat since last fire
			currentHeat = getCurrentHeat();

			// Hurt whatever got in our way
			if (weaponConfiguration.IsProjectileBased) {
				launchProjectile();
			} else {
				castPain ();
			}


			// Add heat to the gun
			currentHeat = Mathf.Clamp01(currentHeat + weaponConfiguration.heatPerFireIncrement);

			// Go into overheated state if we've become overheated
			if(currentHeat == 1f){
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
		public bool isOverheated(){


			// If we're not overheated 
			if(currentState != WeaponState.Overheated) return false;
			

			// Check if enough time has passed to consider completely cooled down.
			if(getCurrentHeat() == 0f){

				// Change the state back to normal because it's cooled down now
				currentState = WeaponState.Normal;

				return false;

			}

			// Nothing is saying we're not overheated so we must be
			return true;

		}


		/// <summary>
		/// Performs a ray cast and instantiates effects and deals damage appropriately
		/// if a cast hits an enemy
		/// </summary>
		private void castPain ()
		{
			
			// If we don't even have a bullet spawn to animate don't bother
			if (bulletSpawn == null) {
				Debug.LogError("The weapon has no bullet spawn child to properly fire weapon");
				return;
			}
			
			// Shoot from perspective of main camera
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			// Change up the ray cast direction based on accuracy of the weapon
			ray.direction = new Vector3(
									Random.Range(weaponConfiguration.Accuracy, 1f) * ray.direction.x,
									Random.Range(weaponConfiguration.Accuracy, 1f) * ray.direction.y,
									Random.Range(weaponConfiguration.Accuracy, 1f) * ray.direction.z
	                            );

			// Store the hit info
			RaycastHit hit;
			
			// If our raycast hit something
			if (Physics.Raycast (ray, out hit, 300)) {

				// If we have a impact effect...
				if(weaponConfiguration.impactEffect != null){

					// Create the impact effect whereever our raycast hit
					Object.Instantiate(weaponConfiguration.impactEffect, hit.point, Quaternion.identity);
					
				}
				
				// Try grabbing the enemy instance
				TurretBehavoir enemyHit = hit.collider.gameObject.GetComponent<TurretBehavoir>();
				
				// If our impact actually was an enemy
				if(enemyHit != null){

					// Damage the enemy
					enemyHit.damage(weaponConfiguration.damagePerFire);
					
				}

				// Try grabbing instance of rigidbody
				Rigidbody rBody = hit.collider.gameObject.GetComponent<Rigidbody>();

				// If our object had a rigid body
				if(rBody != null){

					// Add force because that shit just got hit with a bullet. Knocks shit back man
					rBody.AddForce(Vector3.Normalize(ray.direction)*2,ForceMode.Impulse);

				}
				
			}
			
		}


		private void launchProjectile(){

			// What we want to instantiate
			GameObject projectile = weaponConfiguration.Projectile.projectileModel;

			// If we don't actually have a projectile to launch then don't bother.
			if(projectile == null){
				Debug.LogError("The weapon is labeled as projectile based but has no projectile set to launch");
				return;
			}

			if (bulletSpawn == null) {
				Debug.LogError("The weapon has no bullet spawn child to properly fire weapon");
				return;
			}

			GameObject instance =  Object.Instantiate (projectile, bulletSpawn.position, bulletSpawn.rotation) as GameObject;

			Vector3 launchForce = weaponConfiguration.Projectile.LaunchForce;

			if(launchForce != Vector3.zero){

				if(instance.GetComponent<Rigidbody>() != null){

					instance.GetComponent<Rigidbody>().AddForce(launchForce);
				
				}

			}

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
