using UnityEngine;
using System.Collections;

public class WorldSettings : MonoBehaviour {
	public spawnLocation[] spawnLocations;
}

[System.Serializable]
public struct spawnLocation {
	public Vector3 position;
	public int factionNumber;
}
