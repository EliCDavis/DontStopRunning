using UnityEngine;
using System.Collections;
using EliDavis.Characters.PlayerInGameControl.Weaponry;

namespace EliDavis.Characters.PlayerInGameControl {

	public static class PlayerFactory {


		public static GameObject CreatePlayer(Vector3 position, PrebuiltWeaponType leftWeapon, PrebuiltWeaponType rightWeapon){

			// Grab reference to the player to load
			GameObject playerReference = Resources.Load<GameObject> ("Player");

			// Create instance of the player
			GameObject instance = Object.Instantiate (playerReference, position, Quaternion.identity) as GameObject;

			// Try grabing instance of player behavior
			PlayerBehavior playerBehavior = instance.GetComponent<PlayerBehavior> ();
			GameManagement.GameManager.getInstance ().addFriendly (playerBehavior);

			// If we where succesful in getting the player behavior
			if (playerBehavior != null) {

				// Configure player settings
				playerBehavior.setBoosterSettings (200f, 200f);
				playerBehavior.setHealthParameters (100f, 100f);

			}

			// Create the Weapons //

			// get instance of the player's weapon behavior
			WeaponBehavoir weaponBehavior = instance.GetComponent<WeaponBehavoir>();

			if (weaponBehavior != null) {

				// Create the players left weapon if one was passed in
				if (leftWeapon != PrebuiltWeaponType.None) {

					Weapon leftWeaponObject = WeaponFactory.createWeapon (Vector3.zero, leftWeapon);
					weaponBehavior.setLeftArm (leftWeaponObject);

					leftWeaponObject.getWeaponModel().transform.parent = instance.transform.FindChild("MainCamera").FindChild("Turret Cannons");

					// Invert the X axis because it's on the players left side
					Vector3 pos = WeaponFactory.offSetFromPlayer(leftWeapon);
					pos.x *= -1;
					leftWeaponObject.getWeaponModel().transform.localPosition = pos;

				}


				// Create the players right weapon if one was passed in
				if (rightWeapon != PrebuiltWeaponType.None) {

					Weapon rightWeaponObject = WeaponFactory.createWeapon (Vector3.zero, rightWeapon);
					weaponBehavior.setRightArm (rightWeaponObject);

					rightWeaponObject.getWeaponModel().transform.parent = instance.transform.FindChild("MainCamera").FindChild("Turret Cannons");
					rightWeaponObject.getWeaponModel().transform.localPosition = WeaponFactory.offSetFromPlayer(rightWeapon);

				}

			}



			return instance;

		}


	}

}