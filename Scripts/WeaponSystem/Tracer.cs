using UnityEngine;
using System.Collections;

[RequireComponent (typeof(LineRenderer))]
public class Tracer : MonoBehaviour {
	public float TTL = 0.01f;
	
	public Color newCol;
	
	Color col;
	Color trans;
	
	float Birth;
	
	LineRenderer lr;
	
	void Start() {
		col = new Color(1f,0,0,1f);
		trans = new Color(1f,0.5f,0,0f);
		Birth = Time.time;
		lr = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		
		//GetComponent<LineRenderer>().SetColors(
		//		Color.Lerp(col, trans, (Time.time - Birth)/TTL),
		//		Color.Lerp(trans, col, (Time.time - Birth)/TTL));
		
		//lr.SetColors(Color.blue,Color.green);
		
		newCol = new Color(
			Mathf.Lerp(col.r,trans.r,(Time.time - Birth)/TTL),
			Mathf.Lerp(col.g,trans.g,(Time.time - Birth)/TTL),
			Mathf.Lerp(col.b,trans.b,(Time.time - Birth)/TTL),
			Mathf.Lerp(col.a,trans.a,(Time.time - Birth)/TTL));
		
		//newCol = new Color(Random.Range(0f,1f), Random.Range(0f,1f), Random.Range(0f,1f),Random.Range(0f,1f));
		
		lr.SetColors(newCol,newCol);
		
		//print ((Time.time - Birth)/TTL);
		
		if (Time.time - Birth > TTL) {
			Destroy(gameObject);
		}
	}
}
