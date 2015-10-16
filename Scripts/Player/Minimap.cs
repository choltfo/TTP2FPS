using UnityEngine;
using System.Collections;

public class Minimap : MonoBehaviour {

	public Camera mmCamera;
	public int rightBorder;
	public int topBorder;

	public RectTransform mmOverlay;

	public int size = 200;

	// Use this for initialization
	void Start () {
		mmCamera.pixelRect = new Rect (Screen.width-rightBorder - size,Screen.height-topBorder-size, size, size);
		//mmOverlay.size(Screen.width-rightBorder - size,Screen.height-topBorder-size, size, size);
	}
}
