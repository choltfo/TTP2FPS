using UnityEngine;
using System.Collections;

// Like a  MobileEntity, but carrying a gun and ducking into cover.
public class CombatantEntity : MobileEntity {
	
	// Vars needed:
	
	public WeaponInstance[] weapons;
	public int currentWeapon = 0;
	
	// Cover. Object reference.
	
	
	// Tries to shoot a gun.
	public void fireWeapon() {
		weapons[currentWeapon].trigger(this);
	}
	
	
	// Attempts to enter the cover passed
	public virtual int enterCover() {
		return 3;
	}

}

