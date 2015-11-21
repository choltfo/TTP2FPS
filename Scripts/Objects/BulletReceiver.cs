using UnityEngine;
using System.Collections;

public class BulletReceiver : MonoBehaviour {
	public GameObject bulletHole;

    public BulletData recentHit = null;
	
	public virtual void ReceiveShot(BulletData bd) {
		print ("Was shot by " + bd.shooter.name);

        recentHit = bd;

        bd.shooter.shotNotify(this);
		// This needs excessive extension.
	}
}



