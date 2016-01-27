using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EliDavis.Characters.PlayerInGameControl;
using EliDavis.Characters.PlayerInGameControl.Weaponry;

namespace EliDavis.GameManagement {


	public class MainMenu : MonoBehaviour {


		string lastArmSelected = "left";


		[SerializeField]
		private GameObject[] panelsToHideOnStart = null;


		[SerializeField]
		private GameObject[] panelsToDisplayOnStart = null;


		[SerializeField]
		private GameObject weaponInfoPanel = null;


		/// <summary>
		/// The mech configuration panel in the UI
		/// </summary>
		[SerializeField]
		private GameObject mechConfigurationPanel = null;


		/// <summary>
		/// The current mech configuration we're modifying for the player to play the game in 
		/// </summary>
		private MechConfiguration mechConfiguration = null;


		/// <summary>
		/// Set's a weapon configuration to the mech configuration
		/// </summary>
		/// <param name="weaponChoice">Weapon choice.</param>
		public void setWeaponChoice(string weaponChoice){

			PrebuiltWeaponType weapon = (PrebuiltWeaponType) (System.Enum.Parse(typeof(PrebuiltWeaponType), weaponChoice, true));

			if (lastArmSelected == "left") {
				mechConfiguration.setLeftWeapon (WeaponFactory.getWeaponConfiguration (weapon));
			} else {
				mechConfiguration.setRightWeapon (WeaponFactory.getWeaponConfiguration (weapon));
			}

			updateMechConfigPanel ();

		}


		/// <summary>
		/// Displays a weapon configuration's description in the weapon info panel
		/// </summary>
		/// <param name="weaponChoice">Weapon choice.</param>
		public void viewWeaponConfig(string weaponChoice){

			PrebuiltWeaponType weapon = (PrebuiltWeaponType) (System.Enum.Parse(typeof(PrebuiltWeaponType), weaponChoice, true));

			weaponInfoPanel.GetComponentInChildren<Text>().text = getDescriptionOfWeaponConfiguration(WeaponFactory.getWeaponConfiguration(weapon));

		}


		/// <summary>
		/// Sets the arm selected, so that when a weapon is selected, that 
		/// weapon is set for the specific arm
		/// </summary>
		/// <param name="arm">Arm.</param>
		public void setArmSelected(string arm){

			lastArmSelected = arm;

		}


		/// <summary>
		/// Starts up the assault game mode.
		/// </summary>
		public void playAssault(){

			GameManager.getInstance ().setPlayerMechConfiguration (mechConfiguration);
			Application.LoadLevel ("AreaGen");

		}


		/// <summary>
		/// Ensures the mech congiruation panel is in sync with what is in the 
		/// current internal mech configuration object
		/// </summary>
		private void updateMechConfigPanel(){

			if (mechConfigurationPanel == null) {
				Debug.LogError("We don't have a mech configuration panel to update!");
				return;
			}

			mechConfigurationPanel.transform.FindChild ("Left Weapon Panel").GetComponentInChildren<Text>().text = getDescriptionOfWeaponConfiguration(mechConfiguration.getLeftWeapon());
			mechConfigurationPanel.transform.FindChild ("Right Weapon Panel").GetComponentInChildren<Text>().text = getDescriptionOfWeaponConfiguration(mechConfiguration.getRightWeapon());

		}


		/// <summary>
		/// Genereates a description from the given configuration
		/// </summary>
		/// <returns>The description of weapon configuration.</returns>
		/// <param name="config">Config.</param>
		private string getDescriptionOfWeaponConfiguration(WeaponConfiguration config){

			string configText = config.Name+"\n\n"; 
			
			if (config.IsProjectileBased) {
				
				configText += "Projectile Based\n";
				
			} else {
				
				configText += "Damage: " + config.damagePerFire + "\n";
				
			}
			
			configText += "Accuracy: " + (int)(config.Accuracy*100) + "%\n";
			
			configText += "Fire rate: " + 1f/config.fireRate + " rps\n";
			
			configText += "Heat Index: " + (int)((config.heatPerFireIncrement / config.cooldownRate) * 100);
			
			return configText;

		}


		void Start(){

			// TODO Load mech configuratoin from previous save
			mechConfiguration = new MechConfiguration ();
			mechConfiguration.setLeftWeapon (WeaponFactory.getWeaponConfiguration(PrebuiltWeaponType.Chaingun));
			mechConfiguration.setRightWeapon (WeaponFactory.getWeaponConfiguration(PrebuiltWeaponType.MissileLauncher));
			updateMechConfigPanel ();

			// Hide any panels that might currentely be rendered
			if (panelsToHideOnStart != null) {
				for(int i = 0; i < panelsToHideOnStart.Length; i ++){
					panelsToHideOnStart[i].SetActive(false);
				}
			}

			// Show any panels that might not be currentely rendered
			if (panelsToDisplayOnStart != null) {
				for(int i = 0; i < panelsToDisplayOnStart.Length; i ++){
					panelsToDisplayOnStart[i].SetActive(true);
				}
			}

		}



	}

}