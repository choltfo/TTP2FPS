﻿using UnityEngine;
using System.Collections;

public class Pause : MonoBehaviour {

	public bool paused = false;
	public bool pauseGameplay = true; // In multiplayer, this will be set to false so that the player keeps sending updates when 
	

	// Update is called once per frame
	void Update () {
		//Screen.lockCursor = !paused;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = paused;
	}
}
