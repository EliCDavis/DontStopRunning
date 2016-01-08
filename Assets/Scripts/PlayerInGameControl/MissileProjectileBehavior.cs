using UnityEngine;
using System.Collections;

namespace PlayerInGameControl{

	public class MissileProjectileBehavior : MonoBehaviour {

		[SerializeField]
		private GameObject explosionEffect = null;

		[SerializeField]
		private float damageToDeal = 50f;

		[SerializeField]
		private float maxDistance = 10f;

		void OnCollisionEnter(){

			// We've collided, it's time to blow up
			dealDamageToSurroundings ();

			// If we have an effect for the projectile
			if (explosionEffect != null) {
			
				// Create the effect
				Instantiate(explosionEffect, transform.position, Quaternion.identity);
			
			}

			// Destroy the projectile
			Destroy (gameObject);

		}


		/// <summary>
		/// Deals the damage to surrounding things such as turrets
		/// </summary>
		private void dealDamageToSurroundings (){

			// Get all turrets in the scene
			Enemy.TurretBehavoir[] turrets = GameObject.FindObjectsOfType(typeof(Enemy.TurretBehavoir)) as Enemy.TurretBehavoir[];

			// For every turret in the scene
			for (int i = 0; i < turrets.Length; i ++) {

				// Get distance between it and us
				float distance = Vector3.Distance(turrets[i].transform.position, transform.position);

				// If we're within distance
				if(distance <= maxDistance){

					// Deal damage appropriataly based on the proximity of the explosion
					turrets[i].damage( (1-(distance/maxDistance)) * damageToDeal);
				
				}

			}

			PlayerBehavior player = GameObject.FindObjectOfType<PlayerBehavior>();

			if (player != null) {

				// Get distance between it and us
				float distance = Vector3.Distance(player.transform.position, transform.position);

				// If we're within distance
				if(distance <= maxDistance){
					
					// Deal damage appropriataly based on the proximity of the explosion
					player.damage( (1-(distance/maxDistance)) * damageToDeal);
					
				}

			}

		}

	}

}