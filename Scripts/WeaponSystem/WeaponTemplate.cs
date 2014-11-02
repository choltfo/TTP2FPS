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
	
	// Sear type.
	// 0 = Infinite shots per trigger pull.
	// 1 = Single shot per trigger pull.
	// >1 = N-shot burst per trigger pull.
	//
	// 0,1,3 is a standard assault rifle, with auto, semi, and 3-round burst.
	public int[] fireModes = {0,1,3};

	public WeaponInstance create (GameObject parent, int mags) {
		GameObject go = (GameObject)Instantiate (MainWeapon, parent.transform.position, parent.transform.rotation);
		go.transform.parent = parent.transform;
		go.transform.localPosition = holdPos;
		go.transform.localEulerAngles = Vector3.zero;
		WeaponInstance w = go.AddComponent<WeaponInstance> ();
		w.AS = go.transform.Find (AS).gameObject.audio;
		w.template = this;
		w.magazine = magSize;
		w.ammoReserve = mags * magSize;
		//MonoBehaviour.print ("Is this even getting called?!");
		return w;
	}

}

