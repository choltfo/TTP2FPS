using UnityEngine;
using System.Collections;

public class WeaponInstance : MonoBehaviour {

	// Notes:
	// The 0 point of models should be exactly where the eye/camera would be if one were aiming down the sights properly.

	public WeaponTemplate template;

	public int magazine;
	public int ammoReserve;

	public WeaponState state = WeaponState.None;
	public float lastStateChange = 0f;

	public void shoot (CombatantEntity shooter) {
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast (transform.position + transform.TransformPoint (template.bulletSource), transform.eulerAngles, out hit)) {
			BulletData b = new BulletData(shooter, template.damage);
			hit.transform.gameObject.SendMessage ("ReceiveShot",b);
		}
	}
}
