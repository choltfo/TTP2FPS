using UnityEngine;
using System.Collections;

public class ObjectAnchor : MonoBehaviour {

	public Transform anchor;

	public bool anchorRotation;
	public bool anchorPosition;

	// Update is called once per frame
	void Update () {
		if (anchorPosition) transform.position = anchor.position;
		if (anchorRotation) transform.rotation = anchor.rotation;
	}
}
