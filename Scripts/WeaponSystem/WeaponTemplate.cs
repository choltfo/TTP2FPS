using UnityEngine;
using UnityEditor;

public class WeaponTemplate : ScriptableObject {
	public string Name;
	public GameObject MainWeapon;	// Prefab reference.
	public string AS = "Sound";

	public Vector3 bulletSource;
	public float range;

	public Vector3 scopePos;
	public Vector3 holdPos;

	/*
	public float zRecoil;	// Force applied backwards to the shooter.
	public float xRecoil;	// Maximum sway from a shot. Applied on Y-axis.
	public float yRecoil;	// Maximum vertical sway. Applied on X-axis.
	*/

	public int magSize;

	public float damage;
	
	// Reminder: Recoil tolerance.
	
	// Delay between shots.
	public float rearmTime = 0.2f;
	
	// Time to reload.
	public float reloadTime = 1f;

	public float scopeTime = 0.25f;

	public bool canAim = true;
	
	// Sear type.
	// 0 = Infinite shots per trigger pull.
	// 1 = Single shot per trigger pull.
	// >1 = N-shot burst per trigger pull.
	//
	// 0,1,3 is a standard assault rifle, with auto, semi, and 3-round burst.
	public int[] fireModes = {0,1,3};

	// Animations go beneath here.

	public CustomAnim cameraShake;
	public CustomAnim reload;
	public CustomAnim gunShake;	// Must reset in rearmTime or shorter! Camera shake handles offsetting.

	public WeaponInstance create (GameObject parent, int mags, HoldPos hp, Vector3 position = default(Vector3)) {
		GameObject go = (GameObject)Instantiate (MainWeapon, parent.transform.position, parent.transform.rotation);
		go.transform.parent = parent.transform;
		go.transform.localEulerAngles = Vector3.zero;
		WeaponInstance w = go.AddComponent<WeaponInstance> ();
		//w.AS = go.transform.Find (AS).gameObject.audio;
		w.template = this;
		w.magazine = magSize;
		w.ammoReserve = mags * magSize;
		w.holdPos = hp;
		w.state = WeaponState.None;

		w.animCont = go.GetComponent<Animator>();
		//w.animCont.

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

