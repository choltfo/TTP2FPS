using UnityEngine;
using System.Collections;

public class BulletData {
	public CombatantEntity shooter;
	public float damage;
	// TODO: Add types of bullets, e.g hollow point.
	
	public BulletData(CombatantEntity s, float d) {
		shooter = s;
		damage = d;
	}
}
