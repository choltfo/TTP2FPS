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
	
	public Animator anim;
	public NavMeshAgent navMeshAgent;
	public Vector3 currentTarget;
	Vector3 lastNMDirection;
	
	bool isIdle;
	
	const float sin22p5 = 0.382683;
	
	// Use this for initialization
	void Start () {
		//anim.SetTrigger("IDLE");
		weapons[currentWeapon].enableWeapon(this);
		//anim.StartPlayback();
	}
	
	/*
		NOTE:
		Animated sub-object should always face (anim.transform.forward) towards the enemy.
		Movement is handled exclusively by the NavMeshAgent, while animation is handled by this script and
			the sub-object, which is the root of the gun. This way, it will backpedal while firing when applicable.
	*/
	void Update () {
		navMeshAgent.SetDestination(currentTarget);
		
		Vector3 animLocalDirection = anim.transform.InverseTransformDirection(navMeshAgent.desiredVelocity).normalized;
		
		print (animLocalDirection);
		
		anim.transform.forward = Vector3.forward;	// NOTE: Change this when targetting is a thing! Should point to either direction of travel or target.
		
		Debug.DrawLine(transform.position, transform.position+navMeshAgent.desiredVelocity, Color.cyan);
		Debug.DrawLine(transform.position, transform.position+animLocalDirection, Color.red);
		
		int XTravel = 0;
		int ZTravel = 0;
		//sin22p5
		if (Mathf.Abs(animLocalDirection.x) > sin22p5) XTravel = animLocalDirection.x/Mathf.Abs(animLocalDirection.x);
		if (Mathf.Abs(animLocalDirection.z) > sin22p5) ZTravel = animLocalDirection.z/Mathf.Abs(animLocalDirection.z);
		
		if ((animLocalDirection - lastNMDirection).sqrMagnitude > 0.0001f) { 
			anim.SetTrigger("MOVE_" + (ZTravel == 0 ? "" : (ZTravel > 0 ? "FORWARD" : "BACKWARD")) + (XTravel == 0 ? "" : (XTravel > 0 ? "_RIGHT" : "_LEFT")));
			
			/*if (animLocalDirection.z > 0) {
				if (animLocalDirection.x > 0) {
					anim.SetTrigger("MOVE_FORWARD_RIGHT");
					print ("Turning Right");
				} else if (animLocalDirection.x < 0) {
					anim.SetTrigger("MOVE_FORWARD_LEFT");
					print ("Turning Left");
				} else {
					anim.SetTrigger("MOVE_FORWARD");
					print ("Moving Forward");
				}
			}
			if (animLocalDirection.z < 0) {
				if (animLocalDirection.x > 0) {
					anim.SetTrigger("MOVE_BACKWARD_RIGHT");
					print ("Turning Right");
				} else if (animLocalDirection.x < 0) {
					anim.SetTrigger("MOVE_BACKWARD_LEFT");
					print ("Turning Left");
				} else {
					anim.SetTrigger("MOVE_BACKWARD");
					print ("Moving Backward");
				}
			}*/
			isIdle = false;
		}
		
		if (animLocalDirection.sqrMagnitude < 0.1) {
			if (!isIdle) anim.SetTrigger("IDLE_AIMING");
			weapons[currentWeapon].trigger(this);
			isIdle = true;
			print ("Idling");
		}
		lastNMDirection = animLocalDirection;
		
		
		//if (Time.time > 5) anim.Play (animNames[(int)Anims.idleAiming]);
		/*if (Time.time > 5 && !triggered) {
			anim.SetTrigger("IDLE_AIMING");
			triggered = true;
			print ("Triggering IDLE_AIMING");
		}*/
	}
}
