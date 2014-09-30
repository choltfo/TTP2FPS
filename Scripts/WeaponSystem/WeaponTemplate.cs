using UnityEngine;
using UnityEditor;

public class WeaponTemplate : ScriptableObject {

	// Assets.
	public GameObject MainWeapon;
	public string Name;

	public Vector3 bulletSource;
	public float range;

	public int magSize;

	public float damage;

	public WeaponInstance create (GameObject parent, int mags) {
		GameObject go = (GameObject)Instantiate (MainWeapon, parent.transform.position, parent.transform.rotation);
		WeaponInstance w = go.AddComponent<WeaponInstance>();
		w.magazine = magSize;
		w.ammoReserve = mags * magSize;
		return w;
	}
	
}