using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCollision : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter() {
		AudioClip drum = GameObject.Find ("Hand Controller").GetComponent<AC> ().bomb;
		Vector3 position = GameObject.Find ("Hand Controller").GetComponent<AC> ().position;
		AudioSource.PlayClipAtPoint (drum ,position);
	}
}
