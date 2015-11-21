using UnityEngine;
using System.Collections;

// Like a  MobileEntity, but carrying a gun and ducking into cover.
public class CombatantEntity : MobileEntity {
	
	// Vars needed:
	public Animator anim;
	public WeaponInstance[] weapons;
	public int currentWeapon = 0;

    public Team team = Team.FREE_FOR_ALL;
	
	// Cover. Object reference.
	
	
	// Tries to shoot a gun.
	public void fireWeapon() {
		weapons[currentWeapon].trigger(this);
	}

	public virtual void recoil (float powerCoef) {
		return;
	}
	
	// Attempts to enter the cover passed
	public virtual int enterCover() {
		return 3;
	}
	
	// Called when a BulletReceiver is shot by this entity.
	// Mostly for flashing UI, but could have some other use, such as taunts.
	public virtual void shotNotify (BulletReceiver BR) {}

	// Called when an associated hitbox is shot. Deals with non-damage information.
	public virtual void receiveShot(BulletData BD) {}
}

