using UnityEngine;
using System.Collections;

public class BulletReceiver : MonoBehaviour {
	public GameObject bulletHole;
	
	public virtual void ReceiveShot(BulletData bd) {
		print ("Was shot by " + bd.shooter.name);
		bd.shooter.shotNotify(this);
		// This needs excessive extension.
	}
}



