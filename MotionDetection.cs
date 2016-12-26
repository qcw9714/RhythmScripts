using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class MotionDetection : MonoBehaviour {

	ArrayList sounds;

	// Use this for initialization
	void Start () {
		
		sounds = new ArrayList ();
		sounds.Add (new DrumSound());
		sounds.Add (new PianoSound ());
		sounds.Add (new PianoSound ());
		sounds.Add (new PianoSound ());
	}

	// Update is called once per frame
	void Update () {
		Controller controller = GameObject.Find ("Hand Controller").GetComponent<HandController>().GetLeapController();
		//controller.EnableGesture(Gesture.GestureType.TYPEKEYTAP);
		Frame frame = controller.Frame (); // controller is a Controller object
		foreach (AbstractSound sound in sounds) {
			sound.UpdateFrame (ref frame);//返回值bool
		}
	}
}
