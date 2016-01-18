using UnityEngine;
using System.Collections;
using EliDavis.GameManagement;

namespace EliDavis.Characters.Enemy {

	/// <summary>
	/// Enemy Character class that enemies should inherit from to give them some
	/// additional methods that would make coding behavior quicker.
	/// </summary>
	public class Enemy: Character {


		protected Character getClosestTarget(){

			Character[] friendlysInScene = GameManager.getInstance ().getFriendlyCharacters ();

			Character clostest = null;
			float closestDistance = 1000000f;

			for (int i = 0; i < friendlysInScene.Length; i ++) {

				float distance = Vector3.Distance(friendlysInScene[i].gameObject.transform.position, this.transform.position);

				if(distance < closestDistance){

					closestDistance = distance;
					clostest = friendlysInScene[i];

				}

			}

			return clostest;

		}


		/// <summary>
		/// Given a target a raycast is performed to see if we can hit the target, 
		/// if not that means it hit another object in the way to the target and there
		/// for the target is obscured by something
		/// </summary>
		/// <returns><c>true</c>, if is obscured, <c>false</c> otherwise.</returns>
		/// <param name="target">Target.</param>
		protected bool objectIsObscured(GameObject target){

			// Variable for storing hit results of our raycast
			RaycastHit hit;
			
			// Direction we want the raycast to go in.
			Vector3 toTarget = target.transform.position - transform.position;
			
			// Shoot a ray cast aimed at the player to see if it hits.
			if (Physics.Raycast(transform.position, toTarget, out hit, 10000))
			{
				// If what we hit is our target
				if (hit.transform.gameObject == target)
				{
					return false;
				}
			}

			return true;

		}


	}

}