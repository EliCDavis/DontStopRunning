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
		public GameObject projectile;


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


		public WeaponConfiguration(float damagePerFire, float heatPerFireIncrement, float cooldownRate, float fireRate){

			this.damagePerFire = damagePerFire;
			this.heatPerFireIncrement = heatPerFireIncrement;
			this.cooldownRate = cooldownRate;
			this.fireRate = fireRate;
			this.impactEffect = null;
			this.projectile = null;

		}

		public WeaponConfiguration(float damagePerFire, float heatPerFireIncrement, float cooldownRate, float fireRate, GameObject impactEffect){
			
			this.damagePerFire = damagePerFire;
			this.heatPerFireIncrement = heatPerFireIncrement;
			this.cooldownRate = cooldownRate;
			this.fireRate = fireRate;
			this.impactEffect = impactEffect;
			this.projectile = null;
			
		}


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

	}

}