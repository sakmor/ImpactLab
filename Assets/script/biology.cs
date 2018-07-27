using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class biology : MonoBehaviour
{
	[SerializeField] internal bool IsPunchNext;
	[SerializeField] internal bool IsMoveable = true;
	private Vector3 GoalPos;
	[SerializeField] private float Speed;
	[SerializeField] private float MoveStep;
	public bool isRandom;
	[SerializeField] internal float LastAttackTime;

	[SerializeField] private AnimationState AnimationStates;
	private Animator Animator;
	public float IdleCrossFadeTime = 1f;

	public enum AnimationState
	{
		Idle, Walking, Running, Punching_1, Punching_2
	}
	void Start()
	{
		GoalPos = transform.position;
		Animator = GetComponent<Animator>();
		InvokeRepeating("randomMove", 1f, UnityEngine.Random.Range(3f, 5f));
	}
	void Update()
	{

	}

	internal void PlayAnimation(AnimationState animationStates)
	{
		Animator.ResetTrigger("Idle");
		if (animationStates == AnimationState.Idle)
		{
			if (AnimationStates == AnimationState.Idle) return;
			Animator.SetTrigger("Idle");
			Animator.CrossFade("Idle", 0.1f);
			Animator.speed = 1;
			SetAnimationStates(animationStates);
			IsMoveable = true;
		}
		if (animationStates == AnimationState.Walking)
		{
			if (AnimationStates == AnimationState.Walking) { Animator.speed = Speed * MoveStep; return; }
			Animator.CrossFade("Walking", 1.0f);
			SetAnimationStates(animationStates);
		}
		if (animationStates == AnimationState.Running)
		{
			if (AnimationStates == AnimationState.Running) { Animator.speed = Speed * MoveStep; return; }
			Animator.CrossFade("Running", 0.80f);
			SetAnimationStates(animationStates);
		}
		if (animationStates == AnimationState.Punching_1)
		{
			if (AnimationStates == AnimationState.Punching_1) { return; }
			if (AnimationStates == AnimationState.Punching_2) { return; }
			Animator.CrossFade("Punching_1", 0.1f);
			Animator.speed = 1;
			SetAnimationStates(animationStates);
			IsMoveable = false;
		}
		if (animationStates == AnimationState.Punching_2)
		{
			if (AnimationStates == AnimationState.Punching_2) { return; }
			Animator.CrossFade("Punching_2", 0.1f);
			Animator.speed = 1;
			SetAnimationStates(animationStates);
			IsMoveable = false;
		}
	}

	internal void SetAnimationStates(AnimationState animationStates)
	{
		AnimationStates = animationStates;
	}

	void randomMove()
	{
		if (isRandom)
		{
			int n = UnityEngine.Random.Range(1, 4);
			GoalPos = GameObject.Find("w" + n).transform.position; //fixme:不要在用Find了
		}
	}

	internal void SetIsMoveableTrue()
	{
		IsMoveable = true;
	}

	internal void SetIsPunchNext()
	{
		IsPunchNext = true;
		LastAttackTime = Time.time;
	}
	internal void MoveTo(Vector3 direct)
	{
		if (IsMoveable == false) return;

		GoalPos = transform.position + direct;
		Speed = direct.magnitude;

		transform.position = Vector3.MoveTowards(transform.position, GoalPos, Speed * Time.deltaTime);
		faceTarget(GoalPos, Speed * 10.0f);
		float threshold = 0.9f;
		if (Speed <= threshold) PlayAnimation(AnimationState.Walking);
		if (Speed > threshold) PlayAnimation(AnimationState.Running);

	}

	internal void AttackButtonDown()
	{
		StopWalking();
		if (Time.time - LastAttackTime < IdleCrossFadeTime)
		{
			PlayAnimation(AnimationState.Punching_2);
		}
		if (IsPunchNext == false && AnimationStates == AnimationState.Idle)
		{
			PlayAnimation(AnimationState.Punching_1);
		}

	}

	internal void StopWalking()
	{
		if (AnimationStates == AnimationState.Punching_1) return;
		if (AnimationStates == AnimationState.Punching_2) return;
		PlayAnimation(AnimationState.Idle);
	}


	private void faceTarget(Vector3 etarget, float espeed)
	{
		Vector3 targetDir = etarget - this.transform.position;
		float step = espeed * Time.deltaTime;
		Vector3 newDir = Vector3.RotateTowards(this.transform.forward, targetDir, step, 0.0f);
		this.transform.rotation = Quaternion.LookRotation(newDir);
	}
}
