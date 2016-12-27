using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class DrumSound : AbstractSound {

	float lastSphereRadius;
	int drumThreshold = 38;

	public DrumSound() {
	}

	override public bool UpdateFrame (ref Frame frame) {
		HandList hands = frame.Hands;
		Hand firstHand = hands [0];
		bool isTrue = false;
		float recentSphereRadius = firstHand.SphereRadius;
		if (recentSphereRadius > 0) {
			if (lastSphereRadius > drumThreshold && recentSphereRadius <= drumThreshold) {
				Debug.Log (firstHand.SphereRadius);
				AudioClip drum = GameObject.Find("Hand Controller").GetComponent<AC>().drum;
				Vector3 position = GameObject.Find ("Hand Controller").GetComponent<AC> ().position;
				AudioSource.PlayClipAtPoint (drum,position);
				isTrue = true;
			}
			lastSphereRadius = recentSphereRadius;
		}
		return isTrue;
	}
}
