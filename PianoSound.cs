using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class PianoSound : AbstractSound {

	//ArrayList FrameBuffer;

	//const int max = 20;
	bool isDown = false;

	const int BUFFER_MAX=5;

	private int m_CurBufIndex=0;

	public E_HandInAboveView m_AboveView = E_HandInAboveView.None;//首先检测到的手

	private Dictionary<Finger.FingerType,FingerData>[] m_FingerDatas = new Dictionary<Finger.FingerType, FingerData>[2];//当前帧中手指的数据

	private Dictionary<Finger.FingerType,FingerData>[,] m_FingerDatasBuffer=new Dictionary<Finger.FingerType, FingerData>[2,BUFFER_MAX];//手指数据的所有缓存

	private PointData[] m_PalmDatas = new PointData[2];//当前帧中左手两手掌心点的数据（包括方向以及位置）

	private readonly PointData m_DefaultPointData = new PointData(Vector.Zero, Vector.Zero);

	private readonly FingerData m_DefaultFingerData = new FingerData(Vector.Zero,Vector.Zero,Vector.Zero);

	public PianoSound() {
		//空间分配
		m_FingerDatas[0] = new Dictionary<Finger.FingerType, FingerData>();
		m_FingerDatas[1] = new Dictionary<Finger.FingerType, FingerData>();
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < BUFFER_MAX; j++)
			{
				m_FingerDatasBuffer[i, j] = new Dictionary<Finger.FingerType, FingerData>();
			}
		}
		m_PalmDatas [0]=m_DefaultPointData;
		m_PalmDatas [1]=m_DefaultPointData;
		DicAddDefaultData(m_FingerDatas [0]) ;
		DicAddDefaultData(m_FingerDatas [1]) ;
		for(int i=0;i<2;i++)
		{
			for(int j=0;j<BUFFER_MAX;j++)
			{
				DicAddDefaultData(m_FingerDatasBuffer[i,j]);
			}
		}
	}

	void DicAddDefaultData(Dictionary<Finger.FingerType, FingerData> dic)
	{
		dic.Add (Finger.FingerType.TYPE_INDEX,m_DefaultFingerData);
		dic.Add (Finger.FingerType.TYPE_MIDDLE,m_DefaultFingerData);
		dic.Add (Finger.FingerType.TYPE_PINKY,m_DefaultFingerData);
		dic.Add (Finger.FingerType.TYPE_RING,m_DefaultFingerData);
		dic.Add (Finger.FingerType.TYPE_THUMB,m_DefaultFingerData);
	}

	void ClearTheHandData(int handIndex)
	{
		m_PalmDatas [handIndex].Set (m_DefaultPointData);
		m_FingerDatas[handIndex].Clear();
		m_FingerDatasBuffer [handIndex, m_CurBufIndex].Clear();
	}

	void SaveFingerDataWithHandIndex(Hand hand)                            
	{
		bool isLeft = hand.IsLeft;
		int handIndex = isLeft ? 0 : 1;
		foreach (Finger finger in hand.Fingers)
		{
			Finger.FingerType fingerType = finger.Type;
			Vector fingerDir = finger.Direction;
			Vector distalPos = finger.TipPosition;
			Vector metacarpalPos = finger.Bone(Bone.BoneType.TYPE_METACARPAL).Center;
			//如果是拇指，用近端骨指的位置代替
			if (finger.Type==Finger.FingerType.TYPE_THUMB)
			{
				metacarpalPos = finger.Bone(Bone.BoneType.TYPE_PROXIMAL).Center;
			}
			FingerData fingerData = new FingerData(distalPos,fingerDir, metacarpalPos);
			SaveFingerData(handIndex, fingerType, fingerData);                                                   
		}
	}
	void SaveFingerData(int handIndex,Finger.FingerType fingerType,FingerData fingerData)
	{
		//将data保存或者覆盖到m_FingerDatas中
		if(m_FingerDatas[handIndex].ContainsKey(fingerType))
		{
			m_FingerDatas[handIndex][fingerType] = fingerData;
		}
		else
		{
			m_FingerDatas[handIndex].Add(fingerType, fingerData);
		}

		//保存或者覆盖到buffer中
		if(m_FingerDatasBuffer[handIndex,m_CurBufIndex].ContainsKey(fingerType))
		{
			m_FingerDatasBuffer[handIndex, m_CurBufIndex][fingerType] = fingerData;
		}
		else
		{
			m_FingerDatasBuffer[handIndex, m_CurBufIndex].Add(fingerType, fingerData);
		}
	}
	override public bool UpdateFrame (ref Frame frame) {
		HandList hands = frame.Hands;//所有的手
		int count = hands.Count;
		if (count == 0) {
			m_AboveView = E_HandInAboveView.None;//无
		} else {
			bool isRight = hands [0].IsRight;
			if(isRight)
			{
				m_AboveView = E_HandInAboveView.Right;//右手
			}
			else
			{
				m_AboveView = E_HandInAboveView.Left;//左手
			}
		}
		//清除不存在的手信息
		if (count == 0) {
			for (int i = 0; i < 2; i++) {
				ClearTheHandData (i);
			}
		}
		else if (count == 1) {
			int emptyHandIndex = hands [0].IsLeft ? 1 : 0;
			ClearTheHandData (emptyHandIndex);
		} else {
		}

		foreach (Hand hand in hands)
		{
			if (hand.IsLeft)
			{
				m_PalmDatas[0].m_Position = hand.PalmPosition;
				m_PalmDatas[0].m_Direction = hand.PalmNormal;
			}
			else if(hand.IsRight)
			{
				m_PalmDatas[1].m_Position = hand.PalmPosition;
				m_PalmDatas[1].m_Direction = hand.PalmNormal;
			}
		}
		foreach ( Hand hand in hands )
		{
			SaveFingerDataWithHandIndex( hand );
		}
		//根据两帧之间手的数据来判断是不是食指在点击
		Debug.Log ("In update!");
		if (m_FingerDatas [1].ContainsKey (Finger.FingerType.TYPE_INDEX) && m_FingerDatasBuffer [1, (m_CurBufIndex + 3) % 4].ContainsKey (Finger.FingerType.TYPE_INDEX)
			&& m_FingerDatas [1].ContainsKey (Finger.FingerType.TYPE_MIDDLE) && m_FingerDatasBuffer [1, (m_CurBufIndex + 3) % 4].ContainsKey (Finger.FingerType.TYPE_MIDDLE)) {
			//Debug.Log (m_FingerDatas [1] [Finger.FingerType.TYPE_INDEX].m_Point.m_Position);
			//Debug.Log (m_FingerDatas [1] [Finger.FingerType.TYPE_MIDDLE].m_Point.m_Position);
			if (((m_FingerDatas [1] [Finger.FingerType.TYPE_INDEX].m_Point.m_Position.y < m_FingerDatasBuffer [1,(m_CurBufIndex + 3) % 4] [Finger.FingerType.TYPE_INDEX].m_Point.m_Position.y - 15.0))
				&& System.Math.Abs(m_FingerDatas [1] [Finger.FingerType.TYPE_MIDDLE].m_Point.m_Position.y - m_FingerDatasBuffer [1, (m_CurBufIndex + 3) % 4] [Finger.FingerType.TYPE_MIDDLE].m_Point.m_Position.y)<10.0
				&& isDown == false) {
				isDown = true;
				Debug.Log ("Downing!!");
			}
			if (m_FingerDatas [1] [Finger.FingerType.TYPE_INDEX].m_Point.m_Position.y > m_FingerDatasBuffer [1, (m_CurBufIndex + 3) % 4] [Finger.FingerType.TYPE_INDEX].m_Point.m_Position.y + 15.0
				&& System.Math.Abs(m_FingerDatas [1] [Finger.FingerType.TYPE_MIDDLE].m_Point.m_Position.y - m_FingerDatasBuffer [1, (m_CurBufIndex + 3) % 4] [Finger.FingerType.TYPE_MIDDLE].m_Point.m_Position.y)<10.0) {
				Debug.Log("Uping!!");
				isDown = false;
			}
		}
		m_CurBufIndex=(m_CurBufIndex+1)%(BUFFER_MAX-1);
		return true;
	}
}
