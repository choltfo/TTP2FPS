using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class HitBox : BulletReceiver {
	public MobileEntity entity;
	public float muliplier = 1f;
	
	public override void ReceiveShot(BulletData b) {
		entity.takeDamage(b.damage*muliplier);
		print ("Received hit on " + name + " from " + b.shooter.name + ", passing to " + entity.name + '.');
		b.shooter.shotNotify(this);
		if (entity is CombatantEntity) {
			((CombatantEntity)entity).receiveShot(b);
		}
	}
}
