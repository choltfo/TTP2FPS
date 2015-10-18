using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour {
	public GameObject playerPrefab;

	public WorldSettings world;

	void Start () {
		//world = GetComponent<WorldSettings>();
	}

	// Generates player objects, sets their numbers, configures their viewports, and sends them to the loadout menu.
	public void CreatePlayers(int nPlayers) {

		int playerFaction = 0; // For now, stipulate that all local players are on the same team. This is, of course, wrong, but who cares?
		int spawnPoint = 0;

		// Choose spawn point for non-computer players.
		for (int i = 0; i < world.spawnLocations.Length; i++) {
			if (world.spawnLocations[i].factionNumber == playerFaction) {
				spawnPoint = i;
				break;
			}
		}

		Rect[] viewports = {
			new Rect(0, 0, 1, 1),
			new Rect(0, 0, 0, 0),
			new Rect(0, 0, 0, 0),
			new Rect(0, 0, 0, 0)
		};


		// Screen splitting algorithm.
		// Why? Because if, say, we want to change the viewport width for some dumb reason, we only need to change 2 magic numbers.

		// Does a horizontal-line two player split.
		if (nPlayers > 1) {
			viewports[0].height /= 2;
			viewports[0].y = viewports[0].height;
			viewports[1].height = viewports[0].height;
			viewports[1].width = viewports[0].width;
		}

		// Does needed 3/4 player splits via horizontal lines.
		for (int i = 2; i < nPlayers; i++) {
			viewports[i - 2].width /= 2;
			viewports[i].width = viewports[i - 2].width;
			viewports[i].x = viewports[i - 2].width + viewports[i - 2].x;
			viewports[i].height = viewports[i - 2].height;
			viewports[i].y = viewports[i - 2].y;
		}

		// To prevent spawned units from clipping into eachother, stagger spawned groups in a 1x1 grid.
		//  This same code will be used in part when CP spawning is added.

		List<Player> players = new List<Player>();

		for (int i = 0; i < nPlayers; i++) {
			// Create a new player GameObject, and store it by reference to its Player component.
			players.Add(Instantiate(playerPrefab).GetComponent<Player>());

			players[i].transform.FindChild("Camera").GetComponent<Camera>().rect = viewports[i];
			players[i].playerNumber = 1;	// Debugging with controller: 1. Real: i
			players[i].playerName = "Player number " + (i + 1) + "";

			players[i].transform.position = world.spawnLocations[spawnPoint].position;

			// Next, open the player's in-game-menu.
			//players[i].openIGMenu("Loadout");
        }

	}


}
