using UnityEngine;
using System.Collections;

// An object that manages the logic of the game,
// 	game meaning the players, CPs, respawns, and objectives.

// Also deals with local screen space sharing between players.
// 	Will put one and two on a horizontal (top,bottom) split by default, and then progressively split.

public class GameManager : MonoBehaviour {

	public WorldSettings world;
	
	public Player[] players;


	PlayerManager playerMan;

	// Use this for initialization
	// Also, this might not be the right way to do this.... Only time will tell!
	void Start () {
		playerMan = GetComponent<PlayerManager>();

		// Generate our debug player.
		playerMan.CreatePlayers(3);

		//if (gameManager != null) Destroy (gameManager);
		//gameManager = this;
	}

	// Update is called once per frame
	void Update () {
		// Debug! :D
		if (Input.GetKeyDown(KeyCode.Backspace)) Application.LoadLevel(Application.loadedLevel);
	}
	
	void OnGUI() {
		return;
	}
}



