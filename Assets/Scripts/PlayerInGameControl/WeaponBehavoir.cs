using UnityEngine;
using System.Collections;

namespace PlayerInGameControl{

	public class WeaponBehavoir : MonoBehaviour {

		// TODO: Learn Unity's animation system :(

		private Weapon leftArm;

		private Weapon rightArm;


		// Use this for initialization
		void Start () {

			leftArm = new Weapon (transform.FindChild("MainCamera").FindChild("Turret Cannons").FindChild("Left Arm").gameObject);

			rightArm = new Weapon (transform.FindChild("MainCamera").FindChild("Turret Cannons").FindChild("Right Arm").gameObject);

		}
		
		// Update is called once per frame
		void Update () {

			if (Input.GetAxisRaw ("Left Cannon Fire") != 0) {
				leftArm.fire();
			}

			if (Input.GetAxisRaw ("Right Cannon Fire") != 0) {
				rightArm.fire();
			}

		}

	}

}