using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class MobileEntity : MonoBehaviour {

	// Remember: Use as few as possible functions. Stick everything in a variable ASAP!

	//[HideInInspector]
	public CharacterController c;


	// Health related publics.

	public float health = 100;
	public float maxHealth = 100;
	public bool alive = true;

	// Use this for initialization
	void Start () {
		c = GetComponent<CharacterController> ();
		childStart ();
	}

	void Update () {
		//if (health <= 0) Die ();
		if (alive) {
			Move ();
			childUpdate ();
		}
		childUpdateDoA();
    }
	
	public void takeDamage(float dmg) {
		health -= dmg;
		if (health <= 0) {
			Die ();
			alive = false;
		}
		print ("Took "+ dmg + " hp of damage.");
	}
	
	public virtual void Die() {}

	public virtual void childStart() {}

	public virtual void childUpdate() {}

	public virtual void childUpdateDoA() { }

	public virtual void Move() {}
}
