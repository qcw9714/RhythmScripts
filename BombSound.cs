using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class BombSound : AbstractSound {

	float lastLeftPosition = 0;
	bool isLeft = false;

	public BombSound() {
	}

	override public bool UpdateFrame (ref Frame frame) {
		HandList hands = frame.Hands;

		float recentLeftPosition = 0;
		for (int i = 0; i < hands.Count; ++i)
			if (!hands [i].IsLeft)
				foreach (Finger finger in hands[i].Fingers)
					if (finger.Type == Finger.FingerType.TYPE_INDEX) {
						recentLeftPosition = finger.TipPosition.x;
					}

		// not get correct left position
		if (recentLeftPosition == 0)
			return false;

		bool isTrue = false;
		if (-60 < recentLeftPosition && recentLeftPosition < 60)
			if (recentLeftPosition < lastLeftPosition - 45f && !isLeft && lastLeftPosition != 0) {
				isLeft = true;
				isTrue = true;
				
				for (int i = 0; i < 40; ++i) {
					GameObject go = GameObject.Instantiate (GameObject.Find ("Hand Controller").GetComponent<AC> ().prefabCube);
					go.transform.position = new Vector3 (Random.Range(-1f, 1f), 20, Random.Range(-3f, 3f));
				}
			}
		if (recentLeftPosition > lastLeftPosition + 1f)
			isLeft = false;
		lastLeftPosition = recentLeftPosition;
		return isTrue;
	}
}
