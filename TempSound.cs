﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class TempSound : AbstractSound {
	override public bool UpdateFrame (ref Frame frame) {
		Debug.Log ("BBBBBBBBBBBBBBBBBBB");
		bool isTrue = false; 
		GestureList gestures = frame.Gestures ();
		Debug.Log ("CCCCCCCCCCCCCCCCCCCC");
		for (int i = 0; i < gestures.Count; i++)  
		{  
			Debug.Log ("DDDDDDDDDDDDDDDDDDDD");
			Gesture gesture = gestures[i];  
			switch (gesture.Type)  
			{  
			case Gesture.GestureType.TYPE_CIRCLE:  
				CircleGesture circle = new CircleGesture (gesture); 
				if (circle.State == Gesture.GestureState.STATE_STOP) {
					isTrue = true;
					AudioClip Gong = GameObject.Find("Hand Controller").GetComponent<AC>().gong;
					Vector3 position = GameObject.Find ("Hand Controller").GetComponent<AC> ().position;
					AudioSource.PlayClipAtPoint (Gong,position);
					Debug.Log ("AAAAAAAAAAAAAAAAA");
				}
				break;  
			default:
				break;
			}  

		}   
		return isTrue;
	}
}
