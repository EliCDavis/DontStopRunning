using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace PlayerInGameControl{

	public class WeaponBehavoir : MonoBehaviour {

		// TODO: Learn Unity's animation system :(

		private Slider leftHeat;

		private Slider rightHeat;

		private Weapon leftArm;

		private Weapon rightArm;


		private float getLeftArmHeat(){

			if (leftArm == null) {
				return 0f;
			}

			return leftArm.getCurrentHeat();
		}

		private float getRightArmHeat(){

			if (rightArm == null) {
				return 0f;
			}

			return rightArm.getCurrentHeat();
		}

		// Use this for initialization
		void Start () {

			// Create a basic machine gun configuration
			WeaponConfiguration chaingun = new WeaponConfiguration (2f, .04f,  .15f, 0.05f, .8f);
			chaingun.impactEffect = Resources.Load ("Particle/Bullet Impact") as GameObject;
			leftArm = new Weapon (transform.FindChild ("MainCamera").FindChild ("Turret Cannons").FindChild ("Left Arm").gameObject, chaingun);


			// Create a basic rocket Launcher configuration
			WeaponConfiguration rocketLauncher = new WeaponConfiguration (50f, .2f, .05f, 1f, .95f, new ProjectileConfiguration(Resources.Load<GameObject>("Projectiles/Missile")));
			rightArm = new Weapon (transform.FindChild ("MainCamera").FindChild ("Turret Cannons").FindChild ("Right Arm").gameObject, rocketLauncher);
			

			// Get reference to the UI
			Transform playerUI = transform.FindChild ("PlayerUI");

			// Set slider references for displaying heat
			leftHeat = playerUI.FindChild ("Left Arm Heat").GetComponent<Slider> ();
			rightHeat = playerUI.FindChild ("Right Arm Heat").GetComponent<Slider> ();

		}
		
		// Update is called once per frame
		void Update () {

			if (Input.GetAxisRaw ("Left Cannon Fire") != 0) {
				leftArm.fire();
			}

			if (Input.GetAxisRaw ("Right Cannon Fire") != 0) {
				rightArm.fire();
			}

			// Update heat values
			leftHeat.value = getLeftArmHeat ();
			rightHeat.value = getRightArmHeat ();

			// Display whether or not the gun is overheated
			if (leftArm.isOverheated ()) {
				leftHeat.transform.GetComponentInChildren<Text> ().text = "OVERHEATED";
			} else {
				leftHeat.transform.GetComponentInChildren<Text> ().text = "";
			}
			
			if (rightArm.isOverheated ()) {
				rightHeat.transform.GetComponentInChildren<Text> ().text = "OVERHEATED";
			} else {
				rightHeat.transform.GetComponentInChildren<Text> ().text = "";
			}

		}

	}

}