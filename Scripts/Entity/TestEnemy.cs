using UnityEngine;
using System.Collections;

public class TestEnemy : CombatantEntity {
	
	public float moveSpeedFwd;
	public float moveSpeedBck;
	public float moveSpeedStf;
	
	public GameObject target;
	
	public NavMeshAgent navMeshAgent;
	public Vector3 currentTarget;
	Vector3 lastNMDirection;
	
	bool isIdle;
	bool tracking = false;
	MovementStates state = MovementStates.MovingToTarget;
	
	const float sin22p5 = 0.382683f;	// Used for animation triggering.
	
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
		
		Debug.DrawRay(transform.position,transform.forward, Color.gray);
		
		Vector3 animLocalDirection = anim.transform.InverseTransformDirection(navMeshAgent.desiredVelocity).normalized;
		
		//print (animLocalDirection);
		
		//print (navMeshAgent.path.corners.Length);
		
		if (navMeshAgent.path.corners.Length <= 2) {
			RaycastHit testForLOS;
			Debug.DrawLine(anim.transform.position+new Vector3(0,1.75f)+(anim.transform.forward*1f), target.transform.position+new Vector3(0,0.625f)-(anim.transform.forward*0.5f), Color.blue);
			if (!Physics.Linecast(anim.transform.position+new Vector3(0,1.75f)+(anim.transform.forward*1f), target.transform.position+new Vector3(0,0.625f)-(anim.transform.forward*0.5f), out testForLOS)) {
				state = MovementStates.HasLOS;
				weapons[currentWeapon].transform.eulerAngles.Set(weapons[currentWeapon].transform.eulerAngles.x,transform.forward.y,weapons[currentWeapon].transform.eulerAngles.z);
			} else {
				state = MovementStates.MovingToTarget;
				navMeshAgent.Resume();
			}
		} else {
			state = MovementStates.MovingToTarget;
			navMeshAgent.Resume();
		}
		
		// Entity has line of sight to target. This does not take into account distance, yet....
		// It should now start aiming.
		
		if (state == MovementStates.HasLOS) {
			
			// Point gun to right elevation.
			
			float xRot = (Mathf.Atan(
				(weapons[currentWeapon].transform.position.y - target.transform.position.y)
				/ Mathf.Sqrt(
					Mathf.Pow (weapons[currentWeapon].transform.position.x - target.transform.position.x,2)+
					Mathf.Pow (weapons[currentWeapon].transform.position.z - target.transform.position.z,2)
				)
				) / (2*Mathf.PI)) * 360;
				
			float yRot = anim.transform.eulerAngles.y;//Mathf.Clamp(xRot,weapons[currentWeapon].transform.eulerAngles.x-(1*Time.deltaTime),weapons[currentWeapon].transform.eulerAngles.x+(1*Time.deltaTime));
			
			// anim.transform.eulerAngles.y
			
			weapons[currentWeapon].transform.eulerAngles = new Vector3(xRot,yRot,0);
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
			
			// TODO: Needs changing for covering and advanced PF.
			
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
	
	CoverNode getNearestCover() {
		float prox = float.MaxValue;
		int nearest = 0;
		
		for (int i = 0; i < CoverList.allNodes.Count; i++) {
			float dist = (CoverList.allNodes[i].transform.position-transform.position).sqrMagnitude;
			if (dist < prox) {
				nearest = i;
				prox = dist;
			}
		}
		
		return CoverList.allNodes[nearest];
	}
	
}



