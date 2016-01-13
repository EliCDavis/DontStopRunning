using UnityEngine;
using System.Collections;
using EliDavis.Characters;
using EliDavis.GameManagement;

namespace EliDavis.Characters.Enemy {


	/// <summary>
	/// Enemy factory used for building enemies and putting them in the scene
	/// </summary>
	public static class EnemyFactory {


		/// <summary>
		/// Creates a turret.
		/// </summary>
		/// <returns>The turret instance</returns>
		/// <param name="position">Position to spawn the turret</param>
		public static GameObject CreateTurret(Vector3 position){

			// Get a reference to the turret
			GameObject turretReference = Resources.Load<GameObject> ("Enemies/Turret");

			// Create an instance of the turret
			GameObject turretInstance = Object.Instantiate (turretReference, position, Quaternion.identity) as GameObject;

			// Get an instance of the turret's character
			Character character = turretInstance.transform.FindChild("Head").GetComponent<Character>();

			// Set the paramters the turret will operate under
			character.setHealthParameters (100f,100f);

			// Add the turret to the game manager
			GameManager.getInstance ().addEnemy (character);

			// return the instance we have just created
			return turretInstance;

		}


		/// <summary>
		/// Creates a drone.
		/// </summary>
		/// <returns>The drone instance in the scene</returns>
		/// <param name="position">Position of the drone</param>
		/// <param name="patrolArea">Patrol area the drone will survey</param>
		/// <param name="altitudeToPatrol">Altitude to patrol.</param>
		public static GameObject CreateDrone(Vector3 position, Rect patrolArea, float altitudeToPatrol){

			// Get reference of drone
			GameObject droneReference = Resources.Load<GameObject> ("Enemies/Drone");

			// Create an instance of the drone
			GameObject droneInstance = Object.Instantiate (droneReference, position, Quaternion.identity) as GameObject;

			// Get the behavior of the drone
			DroneBehavior drone = droneInstance.GetComponent<DroneBehavior>();

			// Set the parameters the drone will operate under
			drone.setHealthParameters (100f,100f);
			drone.setPatrolAirspace (patrolArea, altitudeToPatrol);

			// Add the drone to the game manager
			GameManager.getInstance ().addEnemy (drone);

			// return the instance we have just created
			return droneInstance;

		}


	}


}