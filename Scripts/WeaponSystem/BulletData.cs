using UnityEngine;
using System.Collections;

public class BulletData {
	public CombatantEntity shooter;
	public float damage;
	
	public BulletData(CombatantEntity s, float d) {
		shooter = s;
		damage = d;
	}
}
