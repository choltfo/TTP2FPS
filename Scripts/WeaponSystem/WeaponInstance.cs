using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Animation))]
[RequireComponent (typeof(Rigidbody))]
[RequireComponent (typeof(Collider))]
public class WeaponInstance : MonoBehaviour {

	// Notes:
	// The 0 point of models should be exactly where the eye/camera would be if one were aiming down the sights properly.

	// The weapon template accessed to find prefabs and variables.
	public WeaponTemplate template;

	public int magazine;
	public int ammoReserve;

	public float XRecoilVel;
	public float XRecoilRot;
	
	// Takes the Nth fire mode. Allows for select firing of weapons.
	public int fireSelect = 0;

	/*
		On remainingBurst:
		This controls how many bullets should be fired before the next trigger pull is accepted.
		If it is 0, then no bullets should be fired. If it is > 0, that means that that many rounds should be fired before the next trigger pull is accepted.
		If the gun is in automatic, then the variable should be held at 1 by external input. Thus, the gun should keep firing until the trigger is released.
			This is, effectively, the same as semi automatic, but needs external resetting after each shot.
		This should be handled in the CombatantEntity class, where a check between automatic and burst weapons must be made before attempting to fire.
			For Players, this should be done by distinguishing onMousePress and onMouse, or whatever those procedures are called.
	*/
	
	public int remainingBurst = 0;
	public int firedBurst = 0;

	public float lastStateChange = 0f;


	public WeaponState state = WeaponState.None;

	public CombatantEntity holder;

	/// <summary>
	/// Indicates where the gun should be.
	/// If it's 'scope', the gun should be at ScopePos.
	/// If it's 'hold', the gun should be at holdPos.
	/// If it's 'none', the gun is somwhere else.
	/// </summary>
	public HoldPos holdPos;
	float holdChangeTime = 0f;

	Vector3 lastHoldPos;

	bool hasDryFired = false;


	public void setHoldPos (HoldPos Val) {
		lastHoldPos = transform.localPosition;
		holdPos = Val;
		holdChangeTime = Time.time;
	}

	public void incrementFireSelect () {
		fireSelect = (fireSelect + 1) % template.fireModes.Length;
		GameObject audio = (GameObject)Instantiate (template.soundSource, transform.TransformPoint(template.soundSourcePos), new Quaternion());
		audio.transform.parent = transform;
		audio.GetComponent<GunshotAudio> ().soundClip = template.soundDryFire;
		holder.recoil(1);
		XRecoilVel += 0.5f;

	}

	// TODO: Add multiple segments of animations, i.e, remove mag, insert mag, bolt catch, charge, etc.
	// TODO: Add a function that allows for LERPing between the positions ONLY WHEN NECESSARY.
	public AnimState animState = new AnimState();
	
	// To be called continually for automatic, or intermittently for a semi or burst weapon.
	public bool trigger (CombatantEntity shooter) {
		holder = shooter;
		if (!GetComponent<Animation>().IsPlaying(template.sprintAnimName)) {
			if (remainingBurst == 0 && canFire ()) {
				// Potential source of failure!
				remainingBurst = (template.fireModes [fireSelect] == 0 ? 1 : template.fireModes [fireSelect]);
				return true;
			}
			if (magazine == 0 && !hasDryFired) {
				GameObject audio = (GameObject)Instantiate (template.soundSource, transform.TransformPoint(template.soundSourcePos), new Quaternion());
				audio.transform.parent = transform;
				audio.GetComponent<GunshotAudio> ().soundClip = template.soundDryFire;
				hasDryFired = true;
			}
		}
		return false;
	}

	float getVelTowardsCenter() {
		return Mathf.Clamp01 (-XRecoilRot) * template.XRecoilAccel;
	}
	
	public void drop() {
		GetComponent<Collider>().enabled = true;
		GetComponent<Rigidbody>().WakeUp();
		holder = null;
		transform.parent = null;
	}
	
	// Cramm initialization into here!
	// Called on pickup.
	public void init() {
		holdPos = HoldPos.hold;
	}
	
	public void enableWeapon() {
		if (state == WeaponState.Reloading) GetComponent<Animation>().Play(template.reloadAnimName);
		holdPos = HoldPos.hold;
		gameObject.SetActive(true);
	}
	
	public void disableWeapon() {
		GetComponent<Animation>().Stop();
		holdPos = HoldPos.hold;
		gameObject.SetActive(false);
	}
	
	public void setSprinting(bool sprinting) {
		if (sprinting) {
			holdPos = HoldPos.hold;
			GetComponent<Animation>().Stop();
			GetComponent<Animation>().Play(template.sprintAnimName);
			
			//GetComponent<Animation>().Play();
			//print("Animation " + template.sprintAnimName + " exists: " + (GetComponent<Animation>()[template.sprintAnimName] != null));
		} else {
			GetComponent<Animation>().Stop();
			GetComponent<Animation>().Play(template.resetAnimName);
			transform.localPosition = holdPos == HoldPos.hold ? template.holdPos : template.scopePos;
			//print("Ending sprint animation playback.");
		}
	}

	// TODO: Handling lag, i.e, the gun lags behind the player's perspective. Perhaps use a frame-delayed rotation change, or a hinge?
	// TODO: Make guns finish firing bursts when dropped. :D
	void Update() {
		
		//print (GetComponent<Animation>().isPlaying);
		
		if (holder == null) {
			GetComponent<Collider>().enabled = true;
			GetComponent<Rigidbody>().WakeUp();
			return;
		}
		GetComponent<Rigidbody>().Sleep();
		GetComponent<Collider>().enabled = false;
		
		// ANIM if (GetComponent<Animation>().IsPlaying(template.resetAnimName)) GetComponent<Animation>().Stop();

		if (state == WeaponState.Arming && lastStateChange + template.rearmTime < Time.time) setState(WeaponState.None);

		if (state == WeaponState.None && canFire () && remainingBurst > 0) {
			fire ();
		}

		if (state == WeaponState.Reloading && !GetComponent<Animation>().isPlaying) {
			
			state = WeaponState.None;
			int removedRounds = template.magSize - magazine;
			if (removedRounds > ammoReserve) removedRounds = ammoReserve;
			magazine += removedRounds;
			ammoReserve -= removedRounds;

			hasDryFired = false;
		}

		XRecoilVel = -XRecoilRot*10;

		XRecoilRot += XRecoilVel * Time.deltaTime;

		//transform.localEulerAngles = new Vector3(0, 0, 0);
		if (!GetComponent<Animation>().isPlaying) transform.localEulerAngles = new Vector3(0, XRecoilRot, 0);
		/*if (!GetComponent<Animation>().isPlaying)*/ transform.localPosition = Vector3.Lerp (lastHoldPos, holdPos == HoldPos.scope ? template.scopePos : template.holdPos, (Time.time - holdChangeTime)/template.scopeTime);
		//transform.Translate (0,0,-Mathf.Abs(XRecoilRot)/100,Space.Self);
		
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

	// Returns: 
	// 0 - Reloaded properly.
	// 1 - Last mag. Perhaps a shout of some sort?
	// 2 - Out of ammo. Perhaps a different shout?
	// 3 - Failed somehow else.
	public int reload() {
		//animation
		if (magazine < template.magSize && ammoReserve > 0) {
			GetComponent<Animation>().Play(template.reloadAnimName);
			GetComponent<Animation>().Play();
			state = WeaponState.Reloading;
			holdPos = HoldPos.hold;

			GameObject audio = (GameObject)Instantiate (template.soundSource, transform.TransformPoint(template.soundSourcePos), new Quaternion());
			audio.transform.parent = transform.parent;
			audio.GetComponent<GunshotAudio> ().soundClip = template.soundReload;
			
			
			
			return ammoReserve < template.magSize ? 1 : 0;
		}
		if (ammoReserve < 1) return 1;

		return 3;
	}
	
	// For each bullet leaving the gun, do this.
	public void fire () {

		//print ("Shooting weapon.");
		RaycastHit hit = new RaycastHit();

		//animCont.SetInteger ("State",1);

		GameObject audio = (GameObject)Instantiate (template.soundSource, transform.TransformPoint(template.soundSourcePos), new Quaternion());
		audio.GetComponent<GunshotAudio> ().soundClip = template.soundFire;

		// TODO: Charles: Add inaccuracy.

		Vector3 Innacc = Vector3.zero;
		
		if (Physics.Raycast (transform.TransformPoint (template.bulletSource), transform.forward + Innacc, out hit)) {
			BulletData b = new BulletData(holder, template.damage);
			//hit.transform.gameObject.SendMessage ("receiveShot",b);  				// Correct but sketchy feeling way of doing it.

			Debug.DrawLine(transform.TransformPoint (template.bulletSource), hit.point, Color.green, 100);
			Debug.DrawLine(transform.TransformPoint (template.bulletSource), transform.position, Color.red, 100);

			// Sketchy but correct feeling way of doing it.
			BulletReceiver h = hit.transform.gameObject.GetComponent<BulletReceiver>();
			if (h != null) {
				h.ReceiveShot(b);
			}

			
			//print("Hit " + hit.transform.name + " with " + template.name);
		}
		 
		holder.recoil(template.YRecoil);	// Burst+1 since it gets incremented in setState.

		XRecoilRot += ((Random.Range (-1, 1)*2)+1) * template.xRecoil;

		// A bullet has been fired, so remove it from the queue, and get the action moving backwards.
		setState(WeaponState.Arming);
		remainingBurst --;
		magazine --;					// Spend one bullet. Possibly subject to change?
		
		if (magazine == 0) {
 			state = WeaponState.Empty;
			remainingBurst = 0;
		}
	}
}



