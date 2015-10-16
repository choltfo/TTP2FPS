using UnityEngine;

public class Vector3Anim {
	public AnimationCurve x;
	public AnimationCurve y;
	public AnimationCurve z;

	public Vector3 get(float time) {
		return new Vector3(x.Evaluate (time), y.Evaluate (time), z.Evaluate (time));
	}
}
