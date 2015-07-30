using UnityEngine;
using System.Collections.Generic;


[ExecuteInEditMode]
public class CoverNode : MonoBehaviour {

	// Register node for quick finding in the future.
	void Start () {
		if (CoverList.allNodes == null) CoverList.allNodes = new List<CoverNode>();
		CoverList.allNodes.Add(this);
	}
	
	// For editor. And also debug.
	void OnDrawGizmos() {
		if (!Application.isPlaying) {
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(transform.position, 0.2f);
		}
	}
}

public static class CoverList {
	public static List<CoverNode> allNodes;
}
