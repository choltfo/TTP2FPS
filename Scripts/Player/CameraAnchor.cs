using UnityEngine;
using System.Collections;

public class CameraAnchor : MonoBehaviour {

	public Transform anchor;

	public bool anchorRotation;

	// Update is called once per frame
	void Update () {
		transform.position = anchor.position;
	}
}
