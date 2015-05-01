using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : CombatantEntity {

	public int playerNumber = 0;
	public string playerName = "Player 1";

	public GameObject head;

	public float speed = 6.0F;
	public float sprintSpeed = 10.0F;
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
	
	public bool sprinting = false;
	
	private Vector3 moveDirection = Vector3.zero;

	public WeaponTemplate starter;
	
	private WeaponInstance pickup;
	
	public RectTransform reticleTransform;
	
	public RawImage reticleImage;
	
	Color reticleColor = Color.clear;
	

	public override void childStart() {
		if (starter) weapons [0] = starter.create (head, 2, HoldPos.hold, this);
	}
	
	public override void shotNotify (BulletReceiver BR) {
		reticleColor = Color.red;
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
				//if (Input.GetMouseButtonDown (1)) {
				//	if (!sprinting) weapons[currentWeapon].setHoldPos(weapons[currentWeapon].holdPos == HoldPos.scope ? HoldPos.hold : HoldPos.scope);
				//}
				if (Input.GetMouseButton (1) == (weapons[currentWeapon].holdPos == HoldPos.hold) || sprinting) weapons[currentWeapon].setHoldPos((Input.GetMouseButton (1) && !sprinting) ? HoldPos.scope : HoldPos.hold);
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
	
	int pickupWindowWidth = 150;
	int pickupWindowHeight = 100;
	int comparisonWindowHeight = 100;
	
	int layoutLabel (string text, int y) {
		GUI.Label(new Rect(4, (float)y, pickupWindowWidth, 20), text);
		return y + 15;
	}
	
	// Returns the height of the window.
	int weaponDescriptionWindow (WeaponInstance weap, int height) {
		GUI.Box(new Rect(0,0,pickupWindowWidth,height), weap.template.Name);
		int y = 15;
		y = layoutLabel("Damage:\t"+weap.template.damage+" pts", y);
		y = layoutLabel("Range:\t"+weap.template.range+" m", y);
		y = layoutLabel("Mag size:\t"+weap.template.magSize, y);
		y = layoutLabel("Ammo:\t"+(weap.ammoReserve+pickup.magazine), y);
		y = layoutLabel("Fire modes:\t", y);
		for (int i = 0; i < weap.template.fireModes.Length; i++) {
			//pickup.template.fireModes[i]
			y = layoutLabel("    "+(weap.template.fireModes[i] == 0 ? "Auto" : (weap.template.fireModes[i] == 1 ? "Semi-auto" : weap.template.fireModes[i]+"-round burst")), y);
		}
		y = layoutLabel("Fire rate:\t"+(1/weap.template.rearmTime)*60 + " rpm", y);
		
		return y + 5;
	}
	
	void pickupWindow (int id) {
		pickupWindowHeight = weaponDescriptionWindow(pickup, pickupWindowHeight);
	}
	
	void comparisonWindow (int id) {
		comparisonWindowHeight = weaponDescriptionWindow(weapons[currentWeapon], comparisonWindowHeight);
	}
	
	// Draw GUI, enumerate pickup options.
	void OnGUI () {
		if (pickup != null) {
			Vector3 sPos = head.GetComponent<Camera>().WorldToScreenPoint(pickup.transform.position);
			sPos.x = Mathf.Clamp(sPos.x, 0, Screen.width-pickupWindowWidth);
			sPos.y = Screen.height - Mathf.Clamp(sPos.y, pickupWindowHeight, Screen.height);
			//GUI.Box(, pickup.template.Name);
			GUI.Window(0, new Rect(sPos.x, sPos.y, pickupWindowWidth,pickupWindowHeight), pickupWindow, "");
			if (weapons[currentWeapon]) {
				GUI.Window(1, new Rect(Screen.width-pickupWindowWidth, Screen.height-comparisonWindowHeight,
					pickupWindowWidth,pickupWindowHeight), comparisonWindow, "");
			}
		}
		
		reticleImage.color = reticleColor;
		reticleColor = Color.Lerp(reticleColor, new Color(255,0,0,0), Time.deltaTime);
		
	}

	public int chooseSlot() {
		for (int i = 0; i < weapons.Length; i++) if (!weapons[i]) return i;
		return currentWeapon;
	}
	
	public void setSprinting(bool newValue) {
		if (sprinting != newValue) {
			sprinting = newValue;
			if (weapons[currentWeapon]!=null) weapons[currentWeapon].setSprinting(newValue);
		}
	}

	public override void Move () {
		if (c.isGrounded) {
			setSprinting(Input.GetKey(KeyCode.LeftShift));
			moveDirection = new Vector3(Input.GetAxis("Horizontal"+playerNumber), 0, Input.GetAxis("Vertical"+playerNumber));
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= sprinting ? sprintSpeed : speed;
			if (Input.GetButton("Jump"+playerNumber))
				moveDirection.y = jumpSpeed;
		} else setSprinting(false);
		
		moveDirection.y -= gravity * Time.deltaTime;
		c.Move(moveDirection * Time.deltaTime);
	}
}





