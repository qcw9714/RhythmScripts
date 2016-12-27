using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class MotionDetection : MonoBehaviour {

	ArrayList sounds;

	int actionKind = 0;
	int actionStep = 0;
	float lastRotateAngle;

	// Use this for initialization
	void Start () {
		sounds = new ArrayList ();
		sounds.Add (new DrumSound());
		sounds.Add (new PianoLeftSound ());
		sounds.Add (new PianoRightSound ());
		sounds.Add (new GongSound ());
	}

	// Update is called once per frame
	void Update () {
		
		Controller controller = GameObject.Find ("Hand Controller").GetComponent<HandController>().GetLeapController();
		Frame frame = controller.Frame (); // controller is a Controller object

		int index = 0;
		foreach (AbstractSound sound in sounds) {
			++index;
			bool isDetected = sound.UpdateFrame (ref frame);
			if (isDetected && actionKind == 0)
				actionKind = index;
		}

		// idle
		bool isPlaying = GameObject.Find ("astronaut_prefab").GetComponent<Animation> ().isPlaying;
		if (actionKind == 0) {
			if (!isPlaying) GameObject.Find ("astronaut_prefab").GetComponent<Animation> ().Play ("idle");
		}

		// other actions
		if (GameObject.Find ("astronaut_prefab").GetComponent<Animation> ().IsPlaying ("idle"))
			isPlaying = false;
		if (actionKind == 1) {
			if (actionStep == 0 && !isPlaying) {
				++actionStep;
				GameObject.Find ("astronaut_prefab").GetComponent<Animation> ().Play ("jump");
			} else
			if (actionStep == 1 && !isPlaying) {
				++actionStep;
				GameObject.Find ("astronaut_prefab").GetComponent<Animation> ().Play ("jumpland");
			} else
			if (actionStep == 2 && !isPlaying) {
				actionKind = actionStep = 0;
			}
		}
		if (actionKind == 2) {
			if (actionStep == 0) {
				if (!isPlaying) {
					++actionStep;
					lastRotateAngle = Random.Range (0, 360);
					for (int i = 0; i < 4; ++i) {
						if (Mathf.Cos(lastRotateAngle / 180f * Mathf.PI) * GameObject.Find ("astronaut_prefab").GetComponent<Transform> ().position.x <= 0)
						if (Mathf.Sin(lastRotateAngle / 180f * Mathf.PI) * GameObject.Find ("astronaut_prefab").GetComponent<Transform> ().position.z >= 0)
							break;
						lastRotateAngle += 90f;
					}
					GameObject.Find ("astronaut_prefab").GetComponent<Transform> ().Rotate (0, lastRotateAngle, 0);
					GameObject.Find ("astronaut_prefab").GetComponent<Animation> ().Play ("run");
				}
			} else
			if (actionStep == 1) {
				if (!isPlaying) {
					actionKind = actionStep = 0;
					GameObject.Find ("astronaut_prefab").GetComponent<Transform> ().Rotate (0, -lastRotateAngle, 0);
				} else {
					GameObject.Find ("astronaut_prefab").GetComponent<Transform> ().Translate (Vector3.forward * 2f * Time.deltaTime);
				}
			}
		}
		if (actionKind == 3) {
			if (actionStep == 0 && !isPlaying) {
				++actionStep;
				int randomNumber = (int)Random.Range (0, 4);
				if (randomNumber == 0)
					GameObject.Find ("astronaut_prefab").GetComponent<Animation> ().Play ("attack");
				if (randomNumber == 1)
					GameObject.Find ("astronaut_prefab").GetComponent<Animation> ().Play ("hit1");
				if (randomNumber == 2)
					GameObject.Find ("astronaut_prefab").GetComponent<Animation> ().Play ("hit2");
				if (randomNumber == 3)
					GameObject.Find ("astronaut_prefab").GetComponent<Animation> ().Play ("attackStab");
			} else if (actionStep == 1 && !isPlaying) {
				actionKind = actionStep = 0;
			}
		}
		if (actionKind == 4) {
			if (actionStep == 0 && !isPlaying) {
				++actionStep;
				GameObject.Find ("astronaut_prefab").GetComponent<Animation> ().Play ("attackSpearThrow");
			} else if (actionStep == 1 && !isPlaying) {
				actionKind = actionStep = 0;
			}
		}
	}
}
