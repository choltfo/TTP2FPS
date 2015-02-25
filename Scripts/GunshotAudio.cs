using UnityEngine;
using System.Collections;

public class GunshotAudio : MonoBehaviour {

	public AudioClip soundClip;

	// Use this for initialization
	void Start () {
		print (transform.position);
		audio.clip = soundClip;
		audio.Play ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!audio.isPlaying) Destroy(gameObject);
	}
}
