using UnityEngine;
using System.Collections;

public class WeaponInstance : MonoBehaviour {

	// Notes:
	// The 0 point of models should be exactly where the eye/camera would be if one were aiming down the sights properly.

	public AudioSource AS;
	
	// The weapon template accessed to find prefabs and variables.
	public WeaponTemplate template;

	public int magazine;
	public int ammoReserve;
	
	// Takes the Nth fire mode. Allows for select firing of weapons.
	public int fireSelect = 0;

	/*
		On remainingBurst:
		This controls how many bullets should be fired before the next trigger pull is accepted.
		If it is 0, then no bullets should be fired. If it is >0, that means that that many rounds should be fired before the next trigger pull is accepted.
		If the gun is in automatic, then the variable should be held at 1 by external input. Thus, the gun should keep firing until the trigger is released.
			This is, effectively, the same as semi automatic, but needs external resetting after each shot.
		This should be handled in the CombatantEntity class, where a check between automatic and burst weapons must be made before attempting to fire.
			For Players, this should be done by distinguishing onMousePress and onMouse, or whatever those procedures are called.
	*/
	
	public int remainingBurst = 0;
	public int firedBurst = 0;

	public float lastStateChange = 0f;


	public WeaponState state = WeaponState.None;
	/*	set {
			state = value;
			lastStateChange = Time.time;
		}
		get { return state;}
	}*/

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


	public void setHoldPos (HoldPos Val) {
		lastHoldPos = transform.localPosition;
		holdPos = Val;
		holdChangeTime = Time.time;
	}


	// TODO: Add a function that allows for LERPing between the positions ONLY WHEN NECESSARY.
	public AnimState animState = new AnimState();

	public Animator animCont;
	
	// To be called continually for automatic, or intermittently for a semi or burst weapon.
	public bool trigger (CombatantEntity shooter) {
		holder = shooter;
		if (remainingBurst == 0 && canFire()) {
			// Potential source of failure!
			remainingBurst = (template.fireModes[fireSelect] == 0 ? 1 : template.fireModes[fireSelect]);
			return true;
		}
		return false;
	}
	
	void Update() {
		if (state == WeaponState.Arming && lastStateChange + template.rearmTime < Time.time) setState(WeaponState.None);

		if (state == WeaponState.None && canFire () && remainingBurst > 0) {
			fire ();
		}

		transform.localPosition = Vector3.Lerp (lastHoldPos, holdPos == HoldPos.scope ? template.scopePos : template.holdPos, (Time.time - holdChangeTime)/template.scopeTime);

		//transform.Find ("M4A1/Laser").gameObject.GetComponent<LineRenderer> ().useWorldSpace = true;
		//transform.Find ("M4A1/Laser").gameObject.GetComponent<LineRenderer> ().SetPosition(1,transform.TransformPoint (template.bulletSource));
		//transform.Find ("M4A1/Laser").gameObject.GetComponent<LineRenderer> ().SetPosition(0,transform.position);
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
		//print ("Shooting weapon.");
		RaycastHit hit = new RaycastHit();

		//animCont.SetInteger ("State",1);



		// TODO: Charles: Add inaccuracy.

		Vector3 Innacc = Vector3.zero;
		
		if (Physics.Raycast (transform.TransformPoint (template.bulletSource), transform.parent.forward + Innacc, out hit)) {
			BulletData b = new BulletData(holder, template.damage);
			//hit.transform.gameObject.SendMessage ("ReceiveShot",b);  				// Correct but sketchy feeling way of doing it.

			Debug.DrawLine(transform.TransformPoint (template.bulletSource), hit.point, Color.green, 100);
			Debug.DrawLine(transform.TransformPoint (template.bulletSource), transform.position, Color.red, 100);

			BulletReceiver h = hit.transform.gameObject.GetComponent<BulletReceiver>();
			if (h != null) {
				h.ReceiveShot(b);
			}

			
			//print("Hit " + hit.transform.name + " with " + template.name);
		}
		 
		holder.recoil(template.YRecoil, firedBurst+1);	// Burst+1 since it gets incremented in setState.
		
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

