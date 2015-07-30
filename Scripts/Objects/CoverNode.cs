using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CoverNode : MonoBehaviour {
	void Update () {
		
	}
	void OnDrawGizmos() {
		if (!Application.isPlaying) {
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(transform.position, 0.2f);
		}
	}
}


