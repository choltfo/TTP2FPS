﻿using UnityEngine;
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
	
	private WeaponInstance pickup;
	

	public override void childStart() {
		if (starter) weapons [0] = starter.create (head, 2, HoldPos.hold, this);
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
		if (weapons.Length != 0 && weapons[currentWeapon]) {
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
			if (Input.GetKeyDown (KeyCode.R)) {
				weapons[currentWeapon].reload();
			}
			
			if (weapons [currentWeapon].template.canAim) { 
				if (Input.GetMouseButtonDown (1)) {
					weapons[currentWeapon].setHoldPos(weapons[currentWeapon].holdPos == HoldPos.scope ? HoldPos.hold : HoldPos.scope);
				}
			}
		}

		if (recoilVel > 0) {
			recoilVel -= recoilAbsorption * Time.deltaTime;
			rotationVert -= recoilVel;
		} else
			recoilVel = 0;

		if (Input.GetKeyDown (KeyCode.X)) {
			weapons[currentWeapon].incrementFireSelect();
		}
		
		
		pickup = null;
		Collider[] hits = Physics.OverlapSphere (transform.position, 10);
		
		float prox = float.MaxValue;
		
		foreach (Collider hit in hits) {
			if (Vector3.Angle(head.transform.forward, hit.transform.position - head.transform.position) < 45) {
				if (hit != GetComponent<Collider>()) { 
					if (Vector3.Distance(transform.position, hit.transform.position) <  prox) {
						WeaponInstance pickupWI = hit.gameObject.GetComponent<WeaponInstance>();
						if (pickupWI) {
							pickup = pickupWI;
							prox = Vector3.Distance(transform.position, hit.transform.position);
						}
					}
				}
			}
		}
		
		if (pickup != null) {
			if (Input.GetKeyDown(KeyCode.E)) {
				int slot = chooseSlot();
				
				print ("Attempting pickup of " + pickup.template.Name);
				
				if (weapons[slot] != null) weapons[slot].drop();
				weapons[slot] = pickup;
				
				weapons[slot].transform.parent = head.transform;
				weapons[slot].holder = this;
				weapons[slot].init ();
				weapons[slot].holdPos = HoldPos.hold;
				weapons[slot].gameObject.SetActive(true);
				
				switchWeapons(slot);
			}
		}
		
		if (Input.GetAxis("Mouse ScrollWheel") > 0) {
			switchWeapons((currentWeapon+1) % weapons.Length);
		}
		
	}
	
	public void switchWeapons(int newWeap) {
		if (weapons[newWeap] != null) {
			if (weapons[currentWeapon] != null) {
				weapons[currentWeapon].disableWeapon();
			}
			currentWeapon = newWeap;
			if (weapons[currentWeapon] != null) {
				weapons[currentWeapon].enableWeapon();
			}
		}
	}

	public override void recoil(float powerCoef) {
		recoilVel += powerCoef;
	}

	// Draw GUI, enumerate pickup options.
	void OnGUI () {
		if (pickup != null) {
			Vector3 sPos = head.GetComponent<Camera>().WorldToScreenPoint(pickup.transform.position);
			sPos.x = Mathf.Clamp(sPos.x, 0, Screen.width-100);
			sPos.y = Screen.height - Mathf.Clamp(sPos.y, 20, Screen.height);
			GUI.Box(new Rect(sPos.x, sPos.y, 100,20), pickup.template.Name);
		}
	}

	public int chooseSlot() {
		for (int i = 0; i < weapons.Length; i++) if (!weapons[i]) return i;
		return currentWeapon;
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

