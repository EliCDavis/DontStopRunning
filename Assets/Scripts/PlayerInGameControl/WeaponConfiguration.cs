using UnityEngine;
using System.Collections;

namespace PlayerInGameControl{


	/// <summary>
	/// Weapon configuration that stores data of how we want a certain weapon to run.
	/// </summary>
	public struct WeaponConfiguration {


		// TODO: Create projectile's own configuration struct
		/// <summary>
		/// If there is a projectile to launch, then raycasting will be ignored, and 
		/// the impact effect object will never be used.
		/// </summary>
		private ProjectileConfiguration projectile;


		/// <summary>
		/// Used when raycast hits something while firing the weapon, this effect is 
		/// instantiated at the hit point.
		/// </summary>
		public GameObject impactEffect;


		/// <summary>
		/// The damage this weapon deals every time it hits a target.
		/// </summary>
		public float damagePerFire;


		/// <summary>
		/// How much heat is added to the weapon when it is used
		/// </summary>
		public float heatPerFireIncrement;


		/// <summary>
		/// The percentage of cooldown we receive per second.
		/// Ex. If the rate is .25, in 4 seconds we will completely
		/// cool down from overheating.
		/// </summary>
		public float cooldownRate;


		/// <summary>
		/// The fire rate of the weapon.
		/// Cooldown time before next fire can happen
		/// </summary>
		public float fireRate;


		/// <summary>
		/// The accuracy of the weapon clamped between values 0 and 1
		/// </summary>
		private float accuracy;


		/// <summary>
		/// Determines whether or not the weapon is projectile based.
		/// </summary>
		private bool projectileBased;


		public WeaponConfiguration(float damagePerFire, float heatPerFireIncrement, float cooldownRate, float fireRate, float accuracy){

			this.damagePerFire = damagePerFire;
			this.heatPerFireIncrement = heatPerFireIncrement;
			this.cooldownRate = cooldownRate;
			this.fireRate = fireRate;
			this.Accuracy = accuracy;
			this.impactEffect = null;
			this.projectile = new ProjectileConfiguration();
			this.projectileBased = false;

		}


		public WeaponConfiguration(float damagePerFire, float heatPerFireIncrement, float cooldownRate, float fireRate, float accuracy, GameObject impactEffect){
			
			this.damagePerFire = damagePerFire;
			this.heatPerFireIncrement = heatPerFireIncrement;
			this.cooldownRate = cooldownRate;
			this.fireRate = fireRate;
			this.Accuracy = accuracy;
			this.impactEffect = impactEffect;
			this.projectile = new ProjectileConfiguration();
			this.projectileBased = false;
			
		}


		public WeaponConfiguration(float damagePerFire, float heatPerFireIncrement, float cooldownRate, float fireRate, float accuracy, ProjectileConfiguration projectile){
			
			this.damagePerFire = damagePerFire;
			this.heatPerFireIncrement = heatPerFireIncrement;
			this.cooldownRate = cooldownRate;
			this.fireRate = fireRate;
			this.Accuracy = accuracy;
			this.impactEffect = null;
			this.projectile = projectile;
			this.projectileBased = true;
			
		}



		// TODO: Test this!
		/// <summary>
		/// Based on the current configurations, determines whether or not the gun
		/// can ever overheat
		/// </summary>
		/// <returns><c>true</c>, if the gun could ever overheat, <c>false</c> otherwise.</returns>
		public bool canOverheat(){

			if( (1f/fireRate) * heatPerFireIncrement >= cooldownRate){
				return true;
			}

			return false;

		}

		public float Accuracy {

			get {
				return accuracy;
			}
			set {
				accuracy = Mathf.Clamp01(value);
			}

		}

		public bool IsProjectileBased {
			get {
				return projectileBased;
			}
		}

		public ProjectileConfiguration Projectile {
			get {
				return projectile;
			}
			set {
				projectile = value;
				this.projectileBased = true;
			}
		}
	}

}