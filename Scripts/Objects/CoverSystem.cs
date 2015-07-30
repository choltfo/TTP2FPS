using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CoverSystem : MonoBehaviour {
	public CoverNode[] nodes;
	
	void Update () {
		// Draw all cover nodes.
		if (Application.isPlaying) {
			// This might actually be important....
		} else {
			// In editor. Draw lines.
			
		}
	}
}

[System.Serializable]
public class CoverNode {
	// Stored as world coords.
	public Vector3 position;
	public int[] endpointIndex;
}


