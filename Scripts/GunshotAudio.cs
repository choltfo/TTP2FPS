using UnityEngine;
using System.Collections;

public class GunshotAudio : MonoBehaviour {

	public AudioClip soundClip;

	// Use this for initialization
	void Start () {
		GetComponent<AudioSource>().clip = soundClip;
		GetComponent<AudioSource>().Play ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!GetComponent<AudioSource>().isPlaying) Destroy(gameObject);
	}
}
