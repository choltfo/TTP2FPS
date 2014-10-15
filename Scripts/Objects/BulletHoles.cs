using UnityEngine;
using System.Collections;

public class BulletHoles : MonoBehaviour {

	public GameObject bulletHole;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}



	public void ReceiveShot(BulletData bd) {
		print ("Was shot by " + bd.shooter.name);
	}
}
