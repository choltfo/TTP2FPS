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
	}

	void Update () {
		Move ();
		childUpdate ();
	}

	public virtual void childUpdate() {}

	public virtual void Move() {}
}
