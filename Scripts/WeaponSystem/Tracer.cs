using UnityEngine;
using System.Collections;

[RequireComponent (typeof(LineRenderer))]
public class Tracer : MonoBehaviour {
	public float TTL = 0.01f;
	
	Color col;
	Color trans;
	
	float Birth;
	
	void Start() {
		col = new Color(255,128,0,255);
		trans = new Color(255,128,0,0);
		Birth = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		
		GetComponent<LineRenderer>().SetColors(
				Color.Lerp(trans, col, (Time.time - Birth)/(TTL*100)),
				Color.Lerp(trans, col, (Time.time - Birth)/TTL));
		
		if (Birth + TTL < Time.time) {
			Destroy(gameObject);
		}
	}
}
