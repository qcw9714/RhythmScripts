using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class PianoSound : AbstractSound {
	public bool isDown = false;
	public int Index_Now = 0;
	public Vector[,] LeftPosition;//左手
	public Vector[,] RightPosition;//右手
	public double[] RightDisResult = new double[5];
	private readonly Vector VectorZero = new Vector(0,0,0);
	public PianoSound(){
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
		Debug.Log (RightPosition [Index_Now,0]);

		//手信息存储完毕
		//根据右手两帧之间的不同来判断是不是点击手势
		//Index帧前一帧为(Index+1)%2
		for (int i = 0; i < 5; i++) {
			RightDisResult [i] = CalDis (RightPosition [Index_Now,i], RightPosition [(Index_Now + 1) % 2,i]);
			//Debug.Log (RightDisResult [i]);
		}
		int tt = 0;//另外4根手指中移动距离<2.5的个数
		for (int i = 1; i < 5; i++) {
			if (RightDisResult [i] < 2.5) {
				tt++;
			}
		}
		bool isTrue = false;
		if(RightDisResult[0] > 3.0 && tt>=3 && isDown==false && RightPosition [Index_Now,0].y < RightPosition [(Index_Now + 1) % 2,0].y){
			isTrue = true;
			AudioClip drum;
			Vector3 position;
			if (RightPosition [Index_Now, 0].x < -120.0) {
				drum = GameObject.Find ("Hand Controller").GetComponent<AC> ().pianoA;
				position = GameObject.Find ("Hand Controller").GetComponent<AC> ().position;
			} else if (RightPosition [Index_Now, 0].x >= -120.0 && RightPosition [Index_Now, 0].x < -90.0) {
				drum = GameObject.Find ("Hand Controller").GetComponent<AC> ().pianoB;
				position = GameObject.Find ("Hand Controller").GetComponent<AC> ().position;
			} 
			else if (RightPosition [Index_Now, 0].x >= -90.0 && RightPosition [Index_Now, 0].x < -60.0) {
				drum = GameObject.Find ("Hand Controller").GetComponent<AC> ().pianoC;
				position = GameObject.Find ("Hand Controller").GetComponent<AC> ().position;
			} 
			else if (RightPosition [Index_Now, 0].x >= -60.0 && RightPosition [Index_Now, 0].x < -30.0) {
				drum = GameObject.Find ("Hand Controller").GetComponent<AC> ().pianoD;
				position = GameObject.Find ("Hand Controller").GetComponent<AC> ().position;
			} 
			else if (RightPosition [Index_Now, 0].x >= -30.0 && RightPosition [Index_Now, 0].x < 0) {
				drum = GameObject.Find ("Hand Controller").GetComponent<AC> ().pianoE;
				position = GameObject.Find ("Hand Controller").GetComponent<AC> ().position;
			} 
			else if (RightPosition [Index_Now, 0].x >= 0 && RightPosition [Index_Now, 0].x < 30.0) {
				drum = GameObject.Find ("Hand Controller").GetComponent<AC> ().pianoF;
				position = GameObject.Find ("Hand Controller").GetComponent<AC> ().position;
			} 
			else if (RightPosition [Index_Now, 0].x >= 30.0 && RightPosition [Index_Now, 0].x < 60.0) {
				drum = GameObject.Find ("Hand Controller").GetComponent<AC> ().pianoG;
				position = GameObject.Find ("Hand Controller").GetComponent<AC> ().position;
			} 
			else if (RightPosition [Index_Now, 0].x >= 60.0 && RightPosition [Index_Now, 0].x < 90.0) {
				drum = GameObject.Find ("Hand Controller").GetComponent<AC> ().pianoH;
				position = GameObject.Find ("Hand Controller").GetComponent<AC> ().position;
			} 
			else if (RightPosition [Index_Now, 0].x >= 90.0 && RightPosition [Index_Now, 0].x < 120.0) {
				drum = GameObject.Find ("Hand Controller").GetComponent<AC> ().pianoI;
				position = GameObject.Find ("Hand Controller").GetComponent<AC> ().position;
			} 
			else {
				drum = GameObject.Find ("Hand Controller").GetComponent<AC> ().pianoJ;
				position = GameObject.Find ("Hand Controller").GetComponent<AC> ().position;
			}
			isDown = true;
			//Debug.Log("Downing!");
		//	AudioClip drum = GameObject.Find("Hand Controller").GetComponent<AC>().pianoA;
		//	Vector3 position = GameObject.Find ("Hand Controller").GetComponent<AC> ().position;
			AudioSource.PlayClipAtPoint (drum,position);
		}
		if(RightPosition [Index_Now,0].y > RightPosition [(Index_Now + 1) % 2,0].y + 1.0){
			isDown = false;
		}
		Index_Now = (Index_Now + 1) % 2;
		return isTrue;
	}
}
