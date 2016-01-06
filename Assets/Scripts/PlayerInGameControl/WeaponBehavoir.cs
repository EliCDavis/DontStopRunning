using UnityEngine;
using System.Collections;

namespace PlayerInGameControl{

	public class WeaponBehavoir : MonoBehaviour {

		// TODO: Learn Unity's animation system :(

		private Weapon leftArm;

		private Weapon rightArm;


		public float getLeftArmHeat(){

			if (leftArm == null) {
				return 0f;
			}

			return 0;
		}

		public float getRightArmHeat(){

			if (rightArm == null) {
				return 0f;
			}

			return 0;
		}

		// Use this for initialization
		void Start () {

			WeaponConfiguration chaingun = new WeaponConfiguration (3f, .05f, .25f, 0.05f);
			chaingun.impactEffect = Resources.Load ("Particle/Bullet Impact") as GameObject;

			leftArm = new Weapon (transform.FindChild ("MainCamera").FindChild ("Turret Cannons").FindChild ("Left Arm").gameObject, chaingun);

			rightArm = new Weapon (transform.FindChild ("MainCamera").FindChild ("Turret Cannons").FindChild ("Right Arm").gameObject, chaingun);

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