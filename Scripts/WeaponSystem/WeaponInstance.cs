﻿using UnityEngine;
using System.Collections;

public class WeaponInstance : MonoBehaviour {

	// Notes:
	// The 0 point of models should be exactly where the eye/camera would be if one were aiming down the sights properly.
	
	public GameObject mainObject;
	public AudioSource AS;
	
	// The weapon template accessed to find prefabs and variables.
	public WeaponTemplate template;
	
	public int magazine;
	public int ammoReserve;
	
	// Takes the Nth fire mode. Allows for select firing of weapons.
	public int fireSelect = 0;
	
	int remainingBurst = 0;

	public WeaponState state = WeaponState.None;
	public float lastStateChange = 0f;
	
	public CombatantEntity holder;

	/*
		On remainingBurst:
		This controls how many bullets should be fired before the next trigger pull is accepted.
		If it is 0, then no bullets should be fired. If it is >0, that means that that many rounds should be fired before the next trigger pull is accepted.
		If the gun is in automatic, then the variable should be held at 1 by external input. Thus, the gun should keep firing until the trigger is released.
			This is, effectively, the same as semi automatic, but needs external resetting after each shot.
		This should be handled in the CombatantEntity class, where a check between automatic and burst weapons must be made before attempting to fire.
			For Players, this should be done by distinguishing onMousePress and onMouse, or whatever those procedures are called.
	*/
	
	// To be called continually for automatic, or intermittently for a semi or burst weapon.
	public bool trigger (CombatantEntity shooter) {
		holder = shooter;
		if (remainingBurst == 0 && canFire()) {
			// Potential source of failure!
			remainingBurst = (template.fireModes[fireSelect] == 0 ? 1 : template.fireModes[fireSelect]);
		}
	}
	
	void update() {
		if (state == WeaponState.arming && lastStateChange + template.rearmTime > Time.time) setState(WeaponState.None);
		
		if (state == WeaponState.None && canFire() && remainingBurst > 0) {
			fire();
		}
		
	}
	
	// Whether the gun CAN shoot, not whether it will.
	public bool canFire () {
		return state == WeaponState.None && magazine > 0;
	}
	
	// A setter for State, just to prevent stupidity at a later date.
	public void setState (WeaponState s) {
		lastStateChange = Time.time;
		state = s;
	}
	
	// For each bullet leaving the gun, do this.
	public void fire () {
		RaycastHit hit = new RaycastHit();
		// TODO: Charles: Add inaccuracy.
		if (Physics.Raycast (transform.position + transform.TransformPoint (template.bulletSource), transform.eulerAngles, out hit)) {
			BulletData b = new BulletData(holder, template.damage);
			hit.transform.gameObject.SendMessage ("ReceiveShot",b);
		}
		
		// A bullet has been fired, so remove it from the queue, and get the action moving backwards.
		remainingBurst --;
		magazine --;					// Spend one bullet. Possibly subject to change?
		setState(WeaponState.Arming);
		
		if (magazine == 0) {
			state = WeaponState.Empty;
			remainingBurst = 0;
		}
	}
}

