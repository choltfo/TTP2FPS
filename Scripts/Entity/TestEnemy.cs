using UnityEngine;
using System.Collections;

public class TestEnemy : CombatantEntity {
	
	public float moveSpeedFwd;
	public float moveSpeedBck;
	public float moveSpeedStf;
	
	public GameObject target;
	
	
	public Animator anim;
	public NavMeshAgent navMeshAgent;
	public Vector3 currentTarget;
	Vector3 lastNMDirection;
	
	bool isIdle;
	bool tracking = false;
	MovementStates state = MovementStates.MovingToTarget;
	
	const float sin22p5 = 0.382683f;
	
	// Use this for initialization
	void Start () {
		//anim.SetTrigger("IDLE");
		weapons[currentWeapon].enableWeapon(this);
		//anim.StartPlayback();
	}
	
	public override void Die () {
		if (alive){
			tracking = false;
			navMeshAgent.Stop();
			anim.SetTrigger("DIE");
			//weapons[currentWeapon].disableWeapon();
			weapons[currentWeapon].drop();
			navMeshAgent.enabled = false;
			GetComponent<CharacterController>().enabled = false;
		}
	}
	
	/*
		NOTE:
		Animated sub-object should always face (anim.transform.forward) towards the enemy.
		Movement is handled exclusively by the NavMeshAgent, while animation is handled by this script and
			the sub-object, which is the root of the gun. This way, it will backpedal while firing when applicable.
	*/
	public override void childUpdate () {
		if (Input.GetKeyDown(KeyCode.Return)) tracking = true;
		if (tracking) navMeshAgent.SetDestination(target.transform.position);
		
		Vector3 animLocalDirection = anim.transform.InverseTransformDirection(navMeshAgent.desiredVelocity).normalized;
		
		//print (animLocalDirection);
		
		//print (navMeshAgent.path.corners.Length);
		
		if (navMeshAgent.path.corners.Length <= 2) {
			RaycastHit testForLOS;
			Debug.DrawLine(anim.transform.position+new Vector3(0,1.75f)+(anim.transform.forward*1f), target.transform.position+new Vector3(0,0.625f)-(anim.transform.forward*0.5f), Color.blue);
			if (!Physics.Linecast(anim.transform.position+new Vector3(0,1.75f)+(anim.transform.forward*1f), target.transform.position+new Vector3(0,0.625f)-(anim.transform.forward*0.5f), out testForLOS)) {
				state = MovementStates.HasLOS;
			} else {
				state = MovementStates.MovingToTarget;
				navMeshAgent.Resume();
			}
		} else {
			state = MovementStates.MovingToTarget;
			navMeshAgent.Resume();
		}
		
		if (state == MovementStates.MovingToTarget) {
		
			int XTravel = 0;
			int ZTravel = 0;
			//sin22p5
			if (Mathf.Abs(animLocalDirection.x) > sin22p5) XTravel = (int)(animLocalDirection.x/Mathf.Abs(animLocalDirection.x));
			if (Mathf.Abs(animLocalDirection.z) > sin22p5) ZTravel = (int)(animLocalDirection.z/Mathf.Abs(animLocalDirection.z));
			
			if ((animLocalDirection - lastNMDirection).sqrMagnitude > 0.0001f) {
				if (!(XTravel == 0 && ZTravel == 0)) anim.SetTrigger("MOVE" + (ZTravel == 0 ? "" : (ZTravel > 0 ? "_FORWARD" : "_BACKWARD")) + (XTravel == 0 ? "" : (XTravel > 0 ? "_RIGHT" : "_LEFT")));
				isIdle = false;
			}
			
			anim.transform.forward = transform.forward;
			
		} else {
			navMeshAgent.Stop();
			anim.transform.forward = (navMeshAgent.destination - transform.position).normalized;
		}
		
		if (animLocalDirection.sqrMagnitude < 0.1) {
			if (!isIdle) anim.SetTrigger("IDLE_AIMING");
			if (target != null && tracking) weapons[currentWeapon].trigger(this);
			isIdle = true;
			//print ("Idling");
		}
		lastNMDirection = animLocalDirection;
	}
}
