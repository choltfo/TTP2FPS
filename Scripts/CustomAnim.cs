using UnityEngine;
using System.Collections;

// An animation detailing the local rotation and movemtn curves for a single object.
public class CustomAnim : ScriptableObject {
	public Vector3Anim localPosition;
	public Vector3Anim localRotation;

	public void apply (GameObject go, float time) {
		go.transform.localPosition = localPosition.get (time);
		go.transform.localEulerAngles = localRotation.get (time);
	}
}
