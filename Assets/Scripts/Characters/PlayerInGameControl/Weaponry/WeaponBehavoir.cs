using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace EliDavis.Characters.PlayerInGameControl.Weaponry {

	public class WeaponBehavoir : MonoBehaviour {

		// TODO: Learn Unity's animation system :(

		private Slider leftHeat;

		private Slider rightHeat;

		private Weapon leftArm;

		private Weapon rightArm;


		/// <summary>
		/// Sets the left arm that the player fires from
		/// </summary>
		/// <param name="arm">Arm.</param>
		public void setLeftArm (Weapon arm) {
			leftArm = arm;
		}


		/// <summary>
		/// Sets the right arm that the player fires from
		/// </summary>
		/// <param name="arm">Arm.</param>
		public void setRightArm(Weapon arm) {
			rightArm = arm;
		}


		/// <summary>
		/// Returns the current percentage of heat that the weapon has accumulated
		/// </summary>
		/// <returns>The left arm heat.</returns>
		private float getLeftArmHeat(){

			if (leftArm == null) {
				return 0f;
			}

			return leftArm.getCurrentHeat();
		
		}

		/// <summary>
		/// Returns the current percentage of heat that the weapon has accumulated.
		/// </summary>
		/// <returns>The right arm heat.</returns>
		private float getRightArmHeat(){

			if (rightArm == null) {
				return 0f;
			}

			return rightArm.getCurrentHeat();
		}

		// Use this for initialization
		void Start () {

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