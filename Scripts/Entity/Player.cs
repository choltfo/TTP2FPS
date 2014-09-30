using UnityEngine;
using System.Collections;

public class Player : CombatantEntity {

	public int playerNumber = 0;
	public string playerName = "Player 1";

	public float speed = 6.0F;
	public float jumpSpeed = 8.0F;
	public float gravity = 20.0F;

	private Vector3 moveDirection = Vector3.zero;


	public override void Move () {
		if (c.isGrounded) {
			moveDirection = new Vector3(Input.GetAxis("Horizontal"+playerNumber), 0, Input.GetAxis("Vertical"+playerNumber));
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
			if (Input.GetButton("Jump"+playerNumber))
				moveDirection.y = jumpSpeed;
			
		}
		moveDirection.y -= gravity * Time.deltaTime;
		c.Move(moveDirection * Time.deltaTime);
	}
}
