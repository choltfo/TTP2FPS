using UnityEngine;
using System.Collections;

// An object that manages the logic of the game,
// 	game meaning the players, CPs, respawns, and objectives.

// Also deals with local screen space sharing between players.
// 	Will put one and two on a horizontal (top,bottom) split by default, and then progressively split.

public class GameManager : MonoBehaviour {
	
	public static GameManager gameManager;
	
	public Game currentGame;
	
	public Player[] players;
	public Vector3 spawnPoint;
	
	// Use this for initialization
	// Also, this might not be the right way to do this.... Only time will tell!
	void Start () {
		if (gameManager != null) Destroy (gameManager);
		gameManager = this;
	}
	
	// Update is called once per frame
	void Update () {
		// Debug
		if (Input.GetKeyDown(KeyCode.Backspace)) Application.LoadLevel(Application.loadedLevel);
	}
	
	void OnGUI() {
		
	}
}

public class Game : ScriptableObject {
	// All entities in the current game, player included.
	public CombatantEntity[] entities;
}



