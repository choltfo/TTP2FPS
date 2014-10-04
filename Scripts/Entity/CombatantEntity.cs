using UnityEngine;
using System.Collections;

// Like an ME, but carrying a gun and ducking into cover.
public class CombatantEntity : MobileEntity {

	// Vars needed:
	
	public WeaponInstance[] weapons;
	public int currentWeapon;
	
	// Cover. Object reference.

	// Tries to shoot a gun.
	// 0 -> Fired.
	// 1 -> Out of ammo.
	// 2 -> Action resetting.
	// 3 -> Internal badness.
	public int fireWeapon() {
		return 3;
	}
	
	
	// Attempts to enter the cover passed
	public virtual int enterCover() {
		return 3;
	}

}

