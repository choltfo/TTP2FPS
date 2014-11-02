using UnityEngine;
using System.Collections;

public class BulletReceiver : MonoBehaviour {
	public GameObject bulletHole;

	// Use this for initialization
	/*void Start () {
	
	}*/
	
	// Update is called once per frame
	/*void Update () {
	
	}*/
	
	public virtual void ReceiveShot(BulletData bd) {
		print ("Was shot by " + bd.shooter.name);
		// This needs excessive extension.
	}
}



