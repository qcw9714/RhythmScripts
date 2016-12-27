using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class PianoLeftSound : AbstractSound {
	public bool isDown = false;
	public int Index_Now = 0;
	public Vector[,] LeftPosition;//左手
	public Vector[,] RightPosition;//右手
	public double[] LeftDisResult = new double[5];
	private readonly Vector VectorZero = new Vector(0,0,0);
	public PianoLeftSound(){
		LeftPosition  = new Vector[2,5];
		RightPosition  = new Vector[2,5];
		for (int i = 0; i < 2; i++) {
			for (int j = 0; j < 5; j++) {
				LeftPosition [i, j] = new Vector (0, 0, 0);
				RightPosition [i, j] = new Vector (0, 0, 0);
			}
		}
	}
	double CalDis(Vector v,Vector e){
		double temp0 = System.Math.Pow((double)v.x-(double)e.x,2.0);
		double temp1 = System.Math.Pow((double)v.y-(double)e.y,2.0);
		double temp2 = System.Math.Pow((double)v.z-(double)e.z,2.0);
		double res = System.Math.Sqrt (temp0 + temp1 + temp2);
		return res;
	}
	void SaveData(Hand hand){
		if (hand != null) {
			if (hand.IsRight) {//右手
				foreach (Finger finger in hand.Fingers) {///有可能有手指没有检测到·
					Finger.FingerType fingerType = finger.Type;
					switch (fingerType) {
					case Finger.FingerType.TYPE_INDEX:
						RightPosition [Index_Now,0] = finger.TipPosition;
						break;
					case Finger.FingerType.TYPE_MIDDLE:
						RightPosition [Index_Now,1] = finger.TipPosition;
						break;
					case Finger.FingerType.TYPE_PINKY:
						RightPosition [Index_Now,2] = finger.TipPosition;
						break;
					case Finger.FingerType.TYPE_RING:
						RightPosition [Index_Now,3] = finger.TipPosition;
						break;
					case Finger.FingerType.TYPE_THUMB:
						RightPosition [Index_Now,4] = finger.TipPosition;
						break;
					default:
						break;
					}
				}
			} else {
				foreach (Finger finger in hand.Fingers) {///有可能有手指没有检测到·
					Finger.FingerType fingerType = finger.Type;
					switch (fingerType) {
					case Finger.FingerType.TYPE_INDEX:
						LeftPosition [Index_Now,0] = finger.TipPosition;
						break;
					case Finger.FingerType.TYPE_MIDDLE:
						LeftPosition [Index_Now,1] = finger.TipPosition;
						break;
					case Finger.FingerType.TYPE_PINKY:
						LeftPosition [Index_Now,2] = finger.TipPosition;
						break;
					case Finger.FingerType.TYPE_RING:
						LeftPosition [Index_Now,3] = finger.TipPosition;
						break;
					case Finger.FingerType.TYPE_THUMB:
						LeftPosition [Index_Now,4] = finger.TipPosition;
						break;
					default:
						break;
					}
				}		
			}
		}
	}
	override public bool UpdateFrame (ref Frame frame){
		HandList hands = frame.Hands;//所有的手
		int count = hands.Count;
		if (count == 0) {//没有手
			for (int i = 0; i < 5; i++) {
				LeftPosition [Index_Now,i] = VectorZero;
				RightPosition [Index_Now,i] = VectorZero;
			}
		} else if (count == 1) {
			Hand hand = hands [0];
			bool isRight = hand.IsRight;
			if (isRight) {//右手
				SaveData(hand);
				for (int i = 0; i < 5; i++) {//左手没有检测到，置0
					LeftPosition [Index_Now,i] = VectorZero;
				}
			} else {//左手
				SaveData(hand);
				for (int i = 0; i < 5; i++) {//左手没有检测到，置0
					RightPosition [Index_Now,i] = VectorZero;
				}
			}
		} else {//左右手都有
			SaveData(hands[0]);
			SaveData(hands[1]);
		}
		// Debug.Log (RightPosition [Index_Now,0]);

		//手信息存储完毕
		//根据右手两帧之间的不同来判断是不是点击手势
		//Index帧前一帧为(Index+1)%2
		for (int i = 0; i < 5; i++) {
			LeftDisResult [i] = CalDis (LeftPosition [Index_Now,i], LeftPosition [(Index_Now + 1) % 2,i]);
			//Debug.Log (LeftDisResult [i]);
		}
		int tt = 0;//另外4根手指中移动距离<2.5的个数
		for (int i = 1; i < 5; i++) {
			if (LeftDisResult [i] < 2.5) {
				tt++;
			}
		}
		bool hasDrop = false;
		if (LeftPosition [Index_Now, 1].y - LeftPosition [Index_Now, 0].y > 15)
			hasDrop = true;
		bool isTrue = false;
		if(hasDrop && LeftDisResult[0] > 3.0 && tt>=3 && !isDown && LeftPosition [Index_Now,0].y < LeftPosition [(Index_Now + 1) % 2,0].y){
			isTrue = true;
			AudioClip drum;
			Vector3 position;
			position = GameObject.Find ("Hand Controller").GetComponent<AC> ().position;
			Vector hit = LeftPosition [Index_Now, 0];
			int hitX = 1, hitZ = 1;
			if (hit.x < -30) hitX = 0;
			if (hit.x > 30) hitX = 2;
			if (hit.z < -30) hitZ = 0;
			if (hit.z > 30) hitZ = 2;
			int hitValue = hitX * 3 + hitZ;
			if (hitValue == 0) drum = GameObject.Find ("Hand Controller").GetComponent<AC> ().pianoA;
			else if (hitValue == 1) drum = GameObject.Find ("Hand Controller").GetComponent<AC> ().pianoB;
			else if (hitValue == 2) drum = GameObject.Find ("Hand Controller").GetComponent<AC> ().pianoC;
			else if (hitValue == 3) drum = GameObject.Find ("Hand Controller").GetComponent<AC> ().pianoD;
			else if (hitValue == 4) drum = GameObject.Find ("Hand Controller").GetComponent<AC> ().pianoE;
			else if (hitValue == 5) drum = GameObject.Find ("Hand Controller").GetComponent<AC> ().pianoF;
			else if (hitValue == 6) drum = GameObject.Find ("Hand Controller").GetComponent<AC> ().pianoG;
			else if (hitValue == 7) drum = GameObject.Find ("Hand Controller").GetComponent<AC> ().pianoH;
			else if (hitValue == 8) drum = GameObject.Find ("Hand Controller").GetComponent<AC> ().pianoI;
			else drum = GameObject.Find ("Hand Controller").GetComponent<AC> ().pianoJ;
			isDown = true;
			AudioSource.PlayClipAtPoint (drum ,position);

			//	Debug.Log (LeftPosition [Index_Now, 0].x);
			//	Debug.Log (LeftPosition [Index_Now, 0].z);
			//	Debug.Log("Downing!");
			//	AudioClip drum = GameObject.Find("Hand Controller").GetComponent<AC>().pianoA;
			//	Vector3 position = GameObject.Find ("Hand Controller").GetComponent<AC> ().position;

		}
		if(LeftPosition [Index_Now,0].y > LeftPosition [(Index_Now + 1) % 2,0].y + 1.0){
			isDown = false;
		}
		Index_Now = (Index_Now + 1) % 2;
		return isTrue;
	}
}
