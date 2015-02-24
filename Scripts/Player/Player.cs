using UnityEngine;
using System.Collections;

public class Player : CombatantEntity {

	public int playerNumber = 0;
	public string playerName = "Player 1";

	public GameObject head;

	public float speed = 6.0F;
	public float jumpSpeed = 8.0F;
	public float gravity = 20.0F;

	public float sensitivtyVertical = 1;
	public float maxVert = 60;
	public float minVert = -60;
	private float rotationVert = 0F;

	public float recoilVel = 0F;
	public float recoilAbsorption = 100F;

	public float sensitivtyHorizontal = 1;

	public bool invertVertical = false;

	private Vector3 moveDirection = Vector3.zero;

	public WeaponTemplate starter;
	


	public override void childStart() {
		weapons [0] = starter.create (head, 2, HoldPos.hold);
	}

	void handleLook() {
		float inputHorizontal = Input.GetAxis ("Mouse X");
		float inputVertical = Input.GetAxis ("Mouse Y");
		
		rotationVert += (inputVertical * (invertVertical ? sensitivtyVertical : -sensitivtyVertical));
		rotationVert = Mathf.Clamp(rotationVert, -Mathf.Abs(maxVert), Mathf.Abs(minVert));
		
		head.transform.localEulerAngles = new Vector3 (rotationVert,0,0);
		
		transform.localEulerAngles = new Vector3 (0,transform.localEulerAngles.y + (inputHorizontal * sensitivtyHorizontal) % 360,0);
	}

	public override void childUpdate () {
		handleLook ();
		if (weapons.Length != 0) {
			int[] fm = weapons [currentWeapon].template.fireModes;
			if (fm[weapons [currentWeapon].fireSelect] == 0) {
				if (Input.GetMouseButton (0)) {				// Automatic, fireMode is 0.
					weapons [currentWeapon].trigger (this);
				}
			} else {
				if (Input.GetMouseButtonDown (0)) {			// Burst or semi otherwise.
					weapons [currentWeapon].trigger (this);
				}
			}
		}

		if (recoilVel > 0) {
			recoilVel -= recoilAbsorption * Time.deltaTime;
			rotationVert -= recoilVel;
		} else
			recoilVel = 0;

		if (Input.GetKeyDown (KeyCode.X)) {
			weapons[currentWeapon].fireSelect = (weapons[currentWeapon].fireSelect + 1) % weapons[currentWeapon].template.fireModes.Length;
		}

		/*float wheelIn = Input.GetAxis ("Mouse ScrollWheel");
		if (wheelIn != 0 && weapons.Length != 0) {
			print("Changed curweap, deploying weapon!");
			weapons[currentWeapon].gameObject.SetActive(false);
			currentWeapon += (wheelIn > 0 ? 1 : -1);
			weapons[currentWeapon].gameObject.SetActive(true);
		}*/

		if (weapons [currentWeapon].template.canAim) { 
			if (Input.GetMouseButtonDown (1)) {
				weapons[currentWeapon].setHoldPos(weapons[currentWeapon].holdPos == HoldPos.scope ? HoldPos.hold : HoldPos.scope);
			}
		}
		
	}

	public override void recoil(float powerCoef, int sequence) {
		recoilVel += powerCoef;
	}

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

