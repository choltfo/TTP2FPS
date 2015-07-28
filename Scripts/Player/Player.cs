using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : CombatantEntity {

	public int playerNumber = 0;
	public string playerName = "Player 1";

	public GameObject head;

	public float speed = 6.0F;
	public float crouchedSpeed = 3.0F;
	public float proneSpeed = 1.0F;
	public float sprintSpeed = 10.0F;
	public float jumpSpeed = 8.0F;
	public float gravity = 20.0F;
	
	public float lookSensitivityX = 3f;
	public float lookSensitivityY = 3f;
	public float scopeSensitivity = 0.5f;
	
	public float sensitivtyVertical = 1;
	public float maxVert = 60;
	public float minVert = -60;
	private float rotationVert = 0F;

	public float recoilVel = 0F;
	public float recoilAbsorption = 100F;

	public float sensitivtyHorizontal = 1;

	public bool invertVertical = false;
	
	public PlayerStance stance = PlayerStance.standing;
	public bool sprinting = false;
	
	private Vector3 moveDirection = Vector3.zero;

	public WeaponTemplate starter;
	
	private WeaponInstance pickup;
	
	public RectTransform reticleTransform;
	
	public RawImage reticleImage;
	
	Color reticleColor = Color.clear;
	bool wasHoldingFireSwitch = false;
	bool wasHoldingWeaponSwitch = false;
	
	public RectTransform WeaponUI;
	public RectTransform PickupUI;
	public RectTransform CurrentUI;
	

	public override void childStart() {
		if (starter) weapons [0] = starter.create (head, 2, HoldPos.hold, this);
	}
	
	public override void shotNotify (BulletReceiver BR) {
		reticleColor = Color.red;
	}

	void handleLook() {
		float inputHorizontal = Input.GetAxis ("P"+playerNumber+"LookX")*lookSensitivityX*
			(weapons[currentWeapon] != null ? (weapons[currentWeapon].holdPos == HoldPos.scope ? scopeSensitivity : 1) : 1);;
		float inputVertical = Input.GetAxis ("P"+playerNumber+"LookY")*lookSensitivityY*
				(weapons[currentWeapon] != null ? (weapons[currentWeapon].holdPos == HoldPos.scope ? scopeSensitivity : 1) : 1);
		
		rotationVert += (inputVertical * (invertVertical ? sensitivtyVertical : -sensitivtyVertical));
		rotationVert = Mathf.Clamp(rotationVert, -Mathf.Abs(maxVert), Mathf.Abs(minVert));
		
		head.transform.localEulerAngles = new Vector3 (rotationVert,0,0);
		
		transform.localEulerAngles = new Vector3 (0,transform.localEulerAngles.y + (inputHorizontal * sensitivtyHorizontal) % 360,0);
	}
	
	public void setStance(PlayerStance st) {
		stance = st;
		if (st == PlayerStance.standing) {
			c.radius = 0.25f;
			c.height = 1.75f;
			c.center = Vector3.zero;
		}
		if (st == PlayerStance.crouching) {
			c.radius = 0.25f;
			c.height = 1f;
			c.center = Vector3.zero;
			setSprinting(false);
		}
		if (st == PlayerStance.prone) {
			c.radius = 0.25f;
			c.height = 0.5f;
			c.center = Vector3.zero;
			setSprinting(false);
		}
	}

	public override void childUpdate () {
		handleLook ();
		if (weapons.Length != 0 && weapons[currentWeapon]) {
			int[] fm = weapons [currentWeapon].template.fireModes;
			if (fm[weapons [currentWeapon].fireSelect] == 0) {
				if (Input.GetButton("P"+playerNumber+"Shoot")) {				// Automatic, fireMode is 0.
					weapons [currentWeapon].trigger (this);
				}
			} else {
				if (Input.GetButtonDown ("P"+playerNumber+"Shoot")) {			// Burst or semi otherwise.
					weapons [currentWeapon].trigger (this);
				}
			}
			if (Input.GetButtonDown ("P"+playerNumber+"Reload")) {
				weapons[currentWeapon].reload();
			}
			
			if (weapons [currentWeapon].template.canAim) { 
				//if (Input.GetMouseButtonDown (1)) {
				//	if (!sprinting) weapons[currentWeapon].setHoldPos(weapons[currentWeapon].holdPos == HoldPos.scope ? HoldPos.hold : HoldPos.scope);
				//}
				//print(Input.GetButton("P"+playerNumber+"Aim"));
				if (Input.GetButton("P"+playerNumber+"Aim") == (weapons[currentWeapon].holdPos == HoldPos.hold) || sprinting)
						weapons[currentWeapon].setHoldPos((Input.GetButton("P"+playerNumber+"Aim") && !sprinting) ? HoldPos.scope : HoldPos.hold);
				//if (Input.GetMouseButton(1) == (weapons[currentWeapon].holdPos == HoldPos.hold) || sprinting) weapons[currentWeapon].setHoldPos((Input.GetMouseButton (1) && !sprinting) ? HoldPos.scope : HoldPos.hold);
			} 
		}

		if (recoilVel > 0) {
			recoilVel -= recoilAbsorption * Time.deltaTime;
			rotationVert -= recoilVel;
		} else recoilVel = 0;

		if (Input.GetAxis ("P"+playerNumber+"FireMode") > 0 && wasHoldingFireSwitch == false) {
			if (weapons[currentWeapon] != null) weapons[currentWeapon].incrementFireSelect();
		}
		wasHoldingFireSwitch = Input.GetAxis ("P"+playerNumber+"FireMode") > 0;
		
		if (Input.GetKeyDown (KeyCode.C)) {
			if (stance != PlayerStance.crouching) {
				setStance(PlayerStance.crouching);
			} else setStance(PlayerStance.standing);
		}
		
		if (Input.GetKeyDown (KeyCode.LeftControl)) {
			if (stance != PlayerStance.prone) {
				setStance(PlayerStance.prone);
			} else setStance(PlayerStance.standing);
		}
		
		/*if (Input.GetKey (KeyCode.C) != (stance == PlayerStance.crouching)) {
			if (stance == PlayerStance.crouching) {
				setStance(PlayerStance.standing);
			} else setStance(PlayerStance.crouching);
		}
		if (Input.GetKey (KeyCode.LeftControl) != (stance == PlayerStance.prone)) {
			if (stance == PlayerStance.prone) {
				setStance(PlayerStance.standing);
			} else setStance(PlayerStance.prone);
		}*/
		
		pickup = null;
		Collider[] hits = Physics.OverlapSphere (transform.position, 5);
		
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
			if (Input.GetButtonDown("P"+playerNumber+"Interact")) {
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
		
		// TODO: Fix input of ambiguous type. Shit!
		if (Input.GetButton("P"+playerNumber+"Switch") && !wasHoldingWeaponSwitch) {
			switchWeapons((currentWeapon+1) % weapons.Length);
		}
		wasHoldingWeaponSwitch = Input.GetButton("P"+playerNumber+"Switch");
	}
	
	public void switchWeapons(int newWeap) {
		if (weapons[newWeap] != null) {
			if (weapons[currentWeapon] != null) {
				weapons[currentWeapon].disableWeapon();
			}
			currentWeapon = newWeap;
			if (weapons[currentWeapon] != null) {
				weapons[currentWeapon].enableWeapon(this);
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
		y = layoutLabel("Fire rate:\t"+(1/weap.template.rearmTime) + " RPS", y);
		
		return y + 5;
	}
	
	void pickupWindow (int id) {
		pickupWindowHeight = weaponDescriptionWindow(pickup, pickupWindowHeight);
	}
	
	// Current weapon only.
	void comparisonWindow (int id) {
		comparisonWindowHeight = weaponDescriptionWindow(weapons[currentWeapon], comparisonWindowHeight);
	}
	
	// Draw GUI, enumerate pickup options.
	void OnGUI () {
		if (pickup != null) {
			Vector3 sPos = head.GetComponent<Camera>().WorldToScreenPoint(pickup.transform.position);
			sPos.x = Mathf.Clamp(sPos.x, 0, Screen.width-pickupWindowWidth);
			sPos.y = Screen.height - Mathf.Clamp(sPos.y, pickupWindowHeight, Screen.height);
			
			// Contemplated weapon only.
			GUI.Window(0, new Rect(sPos.x, sPos.y, pickupWindowWidth,pickupWindowHeight), pickupWindow, "");
			
			// Current weapon only.
			if (weapons[currentWeapon]) {
				GUI.Window(1, new Rect(Screen.width-pickupWindowWidth, Screen.height-comparisonWindowHeight,
					pickupWindowWidth,comparisonWindowHeight), comparisonWindow, "");
			}
		}
		
		reticleImage.color = reticleColor;
		reticleColor = Color.Lerp(reticleColor, new Color(255,0,0,0), Time.deltaTime);
		
		
		// Show weapon info (name, mag, fire mode, reserve.
		// Needs to be linked to hand-built GUI.
		// 
		if (weapons[currentWeapon] != null) {
			WeaponUI.FindChild("ShortName").GetComponent<Text>().text = weapons[currentWeapon].name;
			WeaponUI.FindChild("LongName").GetComponent<Text>().text = weapons[currentWeapon].name;
			WeaponUI.FindChild("Ammo").GetComponent<Text>().text = "" + weapons[currentWeapon].magazine + '/' +
					weapons[currentWeapon].template.magSize + " (" + weapons[currentWeapon].ammoReserve + ')';
			
			int n = weapons[currentWeapon].template.fireModes[weapons[currentWeapon].fireSelect];
			WeaponUI.FindChild("FireMode").GetComponent<Text>().text = n == 0 ? "...|||" : new string('|',n);
		} 
		
		
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
			setSprinting(Input.GetButton("P"+playerNumber+"Sprint") && stance == PlayerStance.standing);
			moveDirection = new Vector3(Input.GetAxis("P"+playerNumber+"MoveX"), 0, Input.GetAxis("P"+playerNumber+"MoveY"));
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= sprinting ? sprintSpeed : (stance == PlayerStance.standing ? speed : (stance == PlayerStance.crouching ? crouchedSpeed : proneSpeed));
			if (Input.GetButton("P"+playerNumber+"Jump"))
				moveDirection.y = jumpSpeed;
		} else setSprinting(false);
		
		moveDirection.y -= gravity * Time.deltaTime;
		c.Move(moveDirection * Time.deltaTime);
	}
}





