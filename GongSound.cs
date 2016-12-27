using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class GongSound : AbstractSound {
	static readonly float FingerStrightState_Radian = Mathf.PI/12;//15度
	public bool isDown = false;//手掌是不是已经向下运动
	public Vector[] RightPosition;//指尖位置
	public Vector[] RightDirection;//指尖方向
	public Vector[] RightRoot;//根骨位置
	public Vector PalmPosition;//手掌掌心位置
	public Vector PalmNormal;//手掌法向量
	public Vector PalmVelocity;//手掌运动方向
	public bool[] IsAllStraight = new bool[5];
	public bool isMiddleStraight;
	bool isHandDown = false;
	bool isMoveDown = false;
	bool isMoveUp = false;
	public GongSound() {
		RightPosition  = new Vector[5];
		RightDirection  = new Vector[5];
		PalmPosition = new Vector (0, 0, 0);
		PalmNormal = new Vector (0, 0, 0);
		PalmVelocity = new Vector (0, 0, 0);
		RightRoot = new Vector[5];
		for (int j = 0; j < 5; j++) {
			RightPosition [j] = new Vector (0, 0, 0);
			RightDirection [j] = new Vector (0, 0, 0);
			RightRoot [j] = new Vector (0, 0, 0);
		}
	}
	public void MakeAllZero(){
		PalmPosition = Vector.Zero;
		PalmNormal = Vector.Zero;
		PalmVelocity = Vector.Zero;
		for (int j = 0; j < 5; j++) {
			RightPosition [j] = Vector.Zero;
			RightDirection [j] = Vector.Zero;
			RightRoot [j] = Vector.Zero;
		}
	}
	public bool IsStraight(int i, float adjustBorder=0f)
	{
		bool isStraight =false;
		Vector Dir = RightDirection [i];
		//如果指尖方向为0向量，表示无效的数据
		if (!Dir.Equals(Vector.Zero)) 
		{
			Vector fingerDir = RightPosition [i] - RightRoot [i];//指尖位置减去指根位置，由指根指向指尖的向量	        
			float radian = fingerDir.AngleTo(Dir);

			if (radian < FingerStrightState_Radian + adjustBorder)
			{
				isStraight = true;//表示是伸直的
			}
		}
		return isStraight;
	}
	public void SetData(Hand hand){//参数为右手
		foreach (Finger finger in hand.Fingers) {
			Finger.FingerType fingerType = finger.Type;
			switch (fingerType) {
			case Finger.FingerType.TYPE_INDEX://食指
				RightPosition [0] = finger.TipPosition;
				RightDirection [0] = finger.Direction;
				RightRoot [0] = finger.Bone (Bone.BoneType.TYPE_METACARPAL).Center;
				break;
			case Finger.FingerType.TYPE_MIDDLE://中指
				RightPosition [1] = finger.TipPosition;
				RightDirection [1] = finger.Direction;
				RightRoot [1] = finger.Bone (Bone.BoneType.TYPE_METACARPAL).Center;
				break;
			case Finger.FingerType.TYPE_PINKY://小指
				RightPosition [2] = finger.TipPosition;
				RightDirection [2] = finger.Direction;
				RightRoot [2] = finger.Bone (Bone.BoneType.TYPE_METACARPAL).Center;
				break;
			case Finger.FingerType.TYPE_RING://无名指
				RightPosition [3] = finger.TipPosition;
				RightDirection [3] = finger.Direction;
				RightRoot [3] = finger.Bone (Bone.BoneType.TYPE_METACARPAL).Center;
				break;
			case Finger.FingerType.TYPE_THUMB://大拇指
				RightPosition [4] = finger.TipPosition;
				RightDirection [4] = finger.Direction;
				RightRoot [4] = finger.Bone (Bone.BoneType.TYPE_PROXIMAL).Center;
				break;
			default:
				break;
			}
		}
	}
	override public bool UpdateFrame (ref Frame frame) {
		bool isTrue = false;
		HandList hands = frame.Hands;
		int count = hands.Count;
		if (count == 0) {
			MakeAllZero ();
		} else if (count == 1) {
			if (hands [0].IsRight) {//场景中有右手
				PalmPosition = hands [0].PalmPosition;
				PalmNormal = hands [0].PalmNormal;
				PalmVelocity = hands [0].PalmVelocity;
				SetData (hands [0]);
			} else {
				MakeAllZero ();
			}
		} else {//有两只手
			Hand hand = null;
			if (hands [0].IsRight) {
				hand = hands [0];
			} else {
				hand = hands [1];
			}//hand为右手
			PalmPosition = hand.PalmPosition;
			PalmNormal = hand.PalmNormal;
			PalmVelocity = hand.PalmVelocity;
			SetData(hand);
		}
		//数据采集完毕
		//首先看手指是不是伸直的,中指必须伸直并且伸直手指数大于等于3
		int Sum = 0;
		for (int i = 0; i < 5; i++) {
			IsAllStraight[i] = IsStraight (i);
			if (i == 1) {
				isMiddleStraight = IsAllStraight [i];//中指是否伸直
			}
			if (IsAllStraight [i] == true) {
				Sum = Sum + 1;//伸直手指总数
			}
		}
		//然后看手是不是向下的,如果手掌法向量与（0,-1,0）夹角小于15度则是向下
		float f = Mathf.PI/12;//15度
		if (!PalmNormal.Equals (Vector.Zero)) {
			float radian = PalmNormal.AngleTo (new Vector (0, -1, 0));
			if (radian < f) {
				isHandDown = true;
			}
		}
			
		//然后计算手掌当前运动方向与（0,-1,0）夹角，小于15度认为是向下运动
		if (!PalmVelocity.Equals (Vector.Zero)) {
			float radian = PalmVelocity.AngleTo (new Vector (0, -1, 0));
			if (radian < f) {
				isMoveDown = true;
			}
		}

		if (!PalmVelocity.Equals (Vector.Zero)) {
			float radian = PalmVelocity.AngleTo (new Vector (0, 1, 0));
			if (radian < f) {
				isMoveUp = true;
			}
		}
		//还需要限制移动速度
		double Downgg =400.0;
		double Upgg = 300.0;
		double temp0 = System.Math.Pow((double)PalmVelocity.x,2.0);
		double temp1 = System.Math.Pow((double)PalmVelocity.y,2.0);
		double temp2 = System.Math.Pow((double)PalmVelocity.z,2.0);
		double res = System.Math.Sqrt (temp0 + temp1 + temp2); 
		Debug.Log (res);
		if (isHandDown && isMoveDown && Sum >= 3 && isMiddleStraight && !isDown && res>=Downgg) {
			isDown = true;
			AudioClip drum = GameObject.Find("Hand Controller").GetComponent<AC>().gong;
			Vector3 position = GameObject.Find ("Hand Controller").GetComponent<AC> ().position;
			AudioSource.PlayClipAtPoint (drum,position);
			isTrue = true;
		}

		if (isHandDown && isMoveUp && Sum >= 3 && isMiddleStraight && res>=Upgg) {
			isDown = false;
		}
		//isDown恢复

		return isTrue;
	}
}
