using UnityEngine;
using System.Collections;

namespace EliDavis.Characters.PlayerInGameControl.Weaponry {

	public struct ProjectileConfiguration {

		public GameObject projectileModel;

		private Vector3 launchForce;

		public Vector3 LaunchForce {
			get {
				return launchForce;
			}
			set {

				launchForce = value;

				if(launchForce != Vector3.zero){
				
					useALaunchForce = true;
				
				}

			}
		}

		private bool useALaunchForce;

		public bool UseALaunchForce {
			get {
				return useALaunchForce;
			}
			set {

				useALaunchForce = value;

				if(useALaunchForce == false){
					this.LaunchForce = Vector3.zero;
				}

			}
		}

		public ProjectileConfiguration(GameObject projectileModel, Vector3 launchForce){

			this.projectileModel = projectileModel;
			this.launchForce = launchForce;
			this.useALaunchForce = true;

		}

		public ProjectileConfiguration(GameObject projectileModel){
			
			this.projectileModel = projectileModel;
			this.launchForce = Vector3.zero;
			this.useALaunchForce = false;
			
		}

	}

}
