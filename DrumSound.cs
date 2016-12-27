using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class DrumSound : AbstractSound {

	float[] lastSphereRadius;
	int drumThreshold = 38;

	public DrumSound() {
		lastSphereRadius = new float[2];
	}

	override public bool UpdateFrame (ref Frame frame) {
		HandList hands = frame.Hands;
		bool isTrue = false;
		for (int i = 0; i < hands.Count; ++i) {
			Hand firstHand = hands [i];
			float recentSphereRadius = firstHand.SphereRadius;
			if (recentSphereRadius > 0) {
				if (lastSphereRadius[i] > drumThreshold && recentSphereRadius <= drumThreshold) {
					// Debug.Log (firstHand.SphereRadius);
					AudioClip drum = GameObject.Find ("Hand Controller").GetComponent<AC> ().drum;
					Vector3 position = GameObject.Find ("Hand Controller").GetComponent<AC> ().position;
					AudioSource.PlayClipAtPoint (drum, position);
					isTrue = true;
				}
				lastSphereRadius[i] = recentSphereRadius;
			}
		}
		return isTrue;
	}
}
