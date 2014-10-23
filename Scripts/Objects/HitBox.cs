using UnityEngine;
using System.Collections;

// The script attached to objects that need to pass damage onto players and mobs when hit by a bullet.
public class HitBox : BulletReceiver {
	public override void ReceiveShot(BulletData bd) {
		print ("Was shot by " + bd.shooter.name + " for " + bd.damage + " damage.");
	}
}

