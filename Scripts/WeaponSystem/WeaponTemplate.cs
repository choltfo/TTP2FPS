using UnityEngine;

public class WeaponTemplate : ScriptableObject {
	public string Name;
	public GameObject MainWeapon;	// Prefab reference.
	public GameObject soundSource;
	public Vector3 soundSourcePos;

	public Vector3 bulletSource;
	public float range;

	public Vector3 scopePos;
	public Vector3 holdPos;
	
	public string reloadAnimName = "CHANGE THIS";
	public string sprintAnimName = "CHANGE THIS";
	public string resetAnimName  = "CHANGE THIS";
	
	public float YRecoil; // Recoil coeffecient for recoil() on CombatatEntity.

	public int magSize;

	public float damage;
	
	// Reminder: Recoil tolerance.
	
	// Delay between shots.
	public float rearmTime = 0.2f;
	
	// Time to reload.
	public float reloadTime = 1f;

	public float scopeTime = 0.25f;

	public bool canAim = true;
	
	public bool tracers = true;
	public Material tracerMat;
	
	// Sear type.
	// 0 = Infinite shots per trigger pull.
	// 1 = Single shot per trigger pull.
	// >1 = N-shot burst per trigger pull.
	//
	// 0,1,3 is a standard assault rifle, with auto, semi, and 3-round burst.
	public int[] fireModes = {0,1,3};

	// Animations go beneath here.

	public float xRecoil;
	public float XRecoilAccel;

	public AudioClip soundFire;
	public AudioClip soundReload;
	public AudioClip soundFireSelect;
	public AudioClip soundDryFire;

	public WeaponInstance create (GameObject parent, int mags, HoldPos hp, CombatantEntity owner, Vector3 position = default(Vector3)) {
		GameObject go = (GameObject)Instantiate (MainWeapon, parent.transform.position, parent.transform.rotation);
		go.transform.parent = parent.transform;
		go.transform.localEulerAngles = Vector3.zero;

		WeaponInstance w = go.GetComponent<WeaponInstance> ();
		if (!w) w = go.AddComponent<WeaponInstance> ();

		//w.AS = go.transform.Find (AS).gameObject.audio;
		w.template = this;
		w.magazine = magSize;

		// TODO: This will be a problem if ever the player receives a gun with no mags....
		if (w.ammoReserve == 0) w.ammoReserve = mags * magSize;
		w.holdPos = hp;
		w.state = WeaponState.None;
		w.holder = owner;

		if (hp == HoldPos.hold) {
			go.transform.Translate(holdPos, Space.Self);
		} else if (hp == HoldPos.scope) {
			go.transform.Translate(scopePos, Space.Self);
		} else {
			go.transform.Translate(position, Space.Self);
		}

		//MonoBehaviour.print ("Is this even getting called?!");
		return w;
	}

}

