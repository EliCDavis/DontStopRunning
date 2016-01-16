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


	}

}