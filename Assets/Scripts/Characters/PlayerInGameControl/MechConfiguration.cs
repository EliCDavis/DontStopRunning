using UnityEngine;
using System.Collections;
using EliDavis.Characters.PlayerInGameControl.Weaponry;

namespace EliDavis.Characters.PlayerInGameControl {

	/// <summary>
	/// A single configuration that will be used to build a mech, 
	/// such as weapons and armour types
	/// </summary>
	public class MechConfiguration {

		WeaponConfiguration leftWeapon = WeaponFactory.getWeaponConfiguration (PrebuiltWeaponType.Chaingun);

		WeaponConfiguration rightWeapon = WeaponFactory.getWeaponConfiguration (PrebuiltWeaponType.MissileLauncher);

		public void setLeftWeapon(WeaponConfiguration leftWeapon){
			this.leftWeapon = leftWeapon;
		}

		public void setRightWeapon(WeaponConfiguration rightWeapon){
			this.rightWeapon = rightWeapon;
		}

		public WeaponConfiguration getLeftWeapon(){
			return this.leftWeapon;
		}

		public WeaponConfiguration getRightWeapon(){
			return this.rightWeapon;
		}

	}

}