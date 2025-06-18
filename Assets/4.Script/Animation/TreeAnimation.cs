using System;
using UnityEngine;

public class TreeAnimation : MonoBehaviour
{
	private float m_AnimationSpeed = 1f;

	private float m_AnimationCounter;

	

	private void Start()
	{
	
		m_AnimationCounter = UnityEngine.Random.Range(0f, (float)Math.PI * 2f);
	}

	private void Update()
	{
		m_AnimationCounter = Mathf.Repeat(m_AnimationCounter + Time.deltaTime, (float)Math.PI * 2f);
		Vector3 localEulerAngles = transform.localEulerAngles;
		localEulerAngles.z = Mathf.Sin(m_AnimationCounter * m_AnimationSpeed);
		transform.localEulerAngles = localEulerAngles;
	}
}