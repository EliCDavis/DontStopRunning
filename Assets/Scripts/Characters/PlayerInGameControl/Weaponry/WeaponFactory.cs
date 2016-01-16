using UnityEngine;
using System.Collections;

namespace EliDavis.Characters.PlayerInGameControl.Weaponry {


	/// <summary>
	/// Weapon factory that is meant for instantiating prebuilt weapons and the 
	/// configurations that go with them.
	/// </summary>
	public static class WeaponFactory  {


		/// <summary>
		/// Builds a weapon object based on what prebuilt weapon was passed in
		/// </summary>
		/// <returns>The weapon.</returns>
		/// <param name="position">Position.</param>
		/// <param name="weapon">Weapon.</param>
		public static Weapon createWeapon(Vector3 position, PrebuiltWeaponType weapon){
			return new Weapon (createWeaponGameObject (position, weapon), getWeaponConfiguration (weapon));
		}


		/// <summary>
		/// Creates the weapon's 3D model in the scene
		/// </summary>
		/// <returns>The weapon game object.</returns>
		/// <param name="position">Position.</param>
		/// <param name="weapon">Weapon.</param>
		public static GameObject createWeaponGameObject(Vector3 position, PrebuiltWeaponType weapon){

			GameObject reference = null;
			Vector3 rotation = Vector3.zero;
			 
			switch (weapon) {
				
			case PrebuiltWeaponType.Chaingun:
				reference =  Resources.Load<GameObject>("Weaponry/Gatling Gun");
				rotation = new Vector3(-90,0,0);
				break;

			case PrebuiltWeaponType.MissileLauncher:
				reference =  Resources.Load<GameObject>("Weaponry/Rocket Launcher");
				break;

			default:
				reference =  Resources.Load<GameObject>("Weaponry/Gatling Gun");
				rotation = new Vector3(-90,0,0);
				break;

			}

			GameObject instance = null;

			if (reference != null) {
				instance = GameObject.Instantiate (reference, position, Quaternion.Euler(rotation)) as GameObject;
			}

			return instance;

		}


		/// <summary>
		/// Creates a weapon configuration based on the weapon type passed in. 
		/// ( A chaingun fires faster than a rocket launcher )
		/// </summary>
		/// <returns>The weapon configuration.</returns>
		/// <param name="weapon">Weapon.</param>
		public static WeaponConfiguration getWeaponConfiguration(PrebuiltWeaponType weapon){

			switch (weapon){

			case PrebuiltWeaponType.Chaingun:
				return new WeaponConfiguration(4f, .02f,  .15f, 0.05f, .8f, Resources.Load<GameObject> ("Particle/Bullet Impact"));

			case PrebuiltWeaponType.MissileLauncher:
				return new WeaponConfiguration(50f, .2f, .05f, 1f, .95f, new ProjectileConfiguration(Resources.Load<GameObject>("Projectiles/Missile")));


			}

			return new WeaponConfiguration();

		}


		/// <summary>
		/// A weapon when parented to the player has a certain offset from them,
		/// and that offset changes based on the 3D model
		/// </summary>
		/// <returns>The set from player.</returns>
		/// <param name="weapon">Weapon.</param>
		public static Vector3 offSetFromPlayer(PrebuiltWeaponType weapon){

			switch (weapon){
				
			case PrebuiltWeaponType.Chaingun:
				return new Vector3 (1.63f, 0, 0);
				
			case PrebuiltWeaponType.MissileLauncher:
				return new Vector3 (1.649f, 0.033f, 0.239f);
				
				
			}

			return new Vector3 (1.63f, 0, 0);

		}

	}

}
