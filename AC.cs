using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AC : MonoBehaviour {
	public Vector3 position;

	public AudioClip drum;
	public AudioClip pianoA;
	public AudioClip pianoB;
	public AudioClip pianoC;

	// Use this for initialization
	void Start () {
		position = transform.position;
	}

	// Update is called once per frame
	void Update () {
	}
}
