using UnityEngine;
using System.Collections;

public class TestEnemy : CombatantEntity {
	
	public string[] animNames = {
		"idle",
		"idleCrouch",
		"idleAiming",
		"jump",
		"melee",
		"forward",
		"forwardLeft",
		"forwardRight",
		"left",
		"right",
		"turnLeft",
		"turnRight",
		"backward",
		"backwardLeft",
		"backwardRight",
		"dance"
	};
	
	public float moveSpeedFwd;
	public float moveSpeedBck;
	public float moveSpeedStf;
	
	bool triggered = false;
	
	// Use this for initialization
	void Start () {
		//GetComponent<Animator>().SetTrigger("IDLE");
		weapons[currentWeapon].enableWeapon(this);
		//GetComponent<Animator>().StartPlayback();
	}
	
	// Update is called once per frame
	void Update () {
		//if (Time.time > 5) GetComponent<Animator>().Play (animNames[(int)Anims.idleAiming]);
		if (Time.time > 5 && !triggered) {
			GetComponent<Animator>().SetTrigger("IDLE_AIMING");
			triggered = true;
			print ("Triggering IDLE_AIMING");
		}
	}
}
