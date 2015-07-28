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
	
	public NavMeshAgent navMeshAgent;
	public Vector3 currentTarget;
	Vector3 lastNMDirection;
	
	
	// Use this for initialization
	void Start () {
		//GetComponent<Animator>().SetTrigger("IDLE");
		weapons[currentWeapon].enableWeapon(this);
		//GetComponent<Animator>().StartPlayback();
	}
	
	// Update is called once per frame
	void Update () {
		navMeshAgent.SetDestination(currentTarget);
		
		Vector3 localNMDirection = transform.InverseTransformDirection(navMeshAgent.desiredVelocity);
		
		//print (localNMDirection);
		
		if ((localNMDirection - lastNMDirection).sqrMagnitude > 0.2) { 
			if (localNMDirection.x > 0) {
				GetComponent<Animator>().SetTrigger("MOVE_FORWARD_RIGHT");
				print ("Turning Right");
			} else if (localNMDirection.x < 0) {
				GetComponent<Animator>().SetTrigger("MOVE_FORWARD_LEFT");
				print ("Turning Left");
			}
			if (localNMDirection.z > 0) {
				GetComponent<Animator>().SetTrigger("MOVE_FORWARD");
				print ("Moving Forward");
			} else if (localNMDirection.z < 0) {
				GetComponent<Animator>().SetTrigger("MOVE_BACKWARD");
				print ("Moving Backward");
			}
		}
		
		if (localNMDirection.sqrMagnitude < 0.1) GetComponent<Animator>().SetTrigger("IDLE_AIMING"); 
		
		lastNMDirection = localNMDirection;
		
		
		//if (Time.time > 5) GetComponent<Animator>().Play (animNames[(int)Anims.idleAiming]);
		/*if (Time.time > 5 && !triggered) {
			GetComponent<Animator>().SetTrigger("IDLE_AIMING");
			triggered = true;
			print ("Triggering IDLE_AIMING");
		}*/
	}
}
