using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class biology : MonoBehaviour
{
    [SerializeField] internal main Main;
    private float HitStopTime;
    [SerializeField] internal bool IsPunchNext;
    [SerializeField] internal bool IsMoveable = true;
    private Vector3 GoalPos;
    [SerializeField] private float Speed;
    [SerializeField] private Renderer ModelRender;
    private Material Material;
    [SerializeField] private float MoveStep;
    public biology Target;
    public bool isRandom;
    [SerializeField] internal float LastAttackTime;
    [SerializeField] private AnimationState AnimationStates;
    [SerializeField] internal Transform CameraPoint;
    private Animator Animator;
    public float IdleCrossFadeTime = 1f;
    [SerializeField] private float HitBack;

    public bool IsAttackable { get; private set; }

    public enum AnimationState
    {
        Idle, Walking, Running, Punching_1, Punching_2, Punching_3, HurtLeft, HurtRight
    }
    private void Start()
    {
        GoalPos = transform.position;
        Animator = GetComponent<Animator>();
        InvokeRepeating("randomMove", 1f, UnityEngine.Random.Range(3f, 5f));
        Material = Instantiate(ModelRender.materials[0]);
        ModelRender.materials[0] = ModelRender.materials[1] = Material;
    }
    private void Update()
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
            SetIsMoveableTrue();
        }
        if (animationStates == AnimationState.Walking)
        {
            if (AnimationStates == AnimationState.Walking) { Animator.speed = Speed * MoveStep; return; }
            Animator.CrossFade("Walking", 1.0f);
            SetAnimationStates(animationStates);
        }
        if (animationStates == AnimationState.Running)
        {
            if (AnimationStates == AnimationState.Running) { Animator.speed = Speed * 0.25f * MoveStep; return; }
            Animator.CrossFade("Running", 0.80f);
            SetAnimationStates(animationStates);
        }
        if (animationStates == AnimationState.Punching_1)
        {
            if (AnimationStates == AnimationState.Punching_1) { return; }
            if (AnimationStates == AnimationState.Punching_2) { return; }
            Animator.CrossFade("Punching_1", 0.1f);
            Animator.speed = 1.5f;
            SetAnimationStates(animationStates);
            SetIsMoveableFalse();
        }
        if (animationStates == AnimationState.Punching_2)
        {
            if (AnimationStates == AnimationState.Punching_2) { return; }
            Animator.CrossFade("Punching_2", 0.1f);
            Animator.speed = 1.5f;
            SetAnimationStates(animationStates);
            SetIsMoveableFalse();
        }
        if (animationStates == AnimationState.HurtLeft)
        {
            Animator.CrossFade("HurtLeft", 0.1f, 0, 0.1f);
            Animator.speed = 1;
            SetAnimationStates(animationStates);
            SetIsMoveableFalse();
        }
        if (animationStates == AnimationState.HurtRight)
        {
            Animator.CrossFade("HurtRight", 0.1f, 0, 0.1f);
            Animator.speed = 1;
            SetAnimationStates(animationStates);
            SetIsMoveableFalse();
        }
        if (animationStates == AnimationState.Punching_3)
        {
            Animator.CrossFade("Punching_3", 0.1f);
            Animator.speed = 1.5f;
            SetAnimationStates(animationStates);
            SetIsMoveableFalse();
        }
    }
    internal void searchClosetTarget()
    {
        float dist = Mathf.Infinity;
        GameObject tempTarget = null;
        foreach (var t in Main.EmenyBiologys)
        {
            float temp = Vector3.Distance(t.transform.position, transform.position);
            if (temp < dist)
            {
                tempTarget = t;
                dist = temp;
            }
        }
        Target = tempTarget.GetComponent<biology>();

    }

    internal void SpaceButtonDown()
    {
        StopWalking();
        PlayAnimation(AnimationState.Punching_3);
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
    internal void SetIsMoveableFalse()
    {
        IsMoveable = false;
    }
    internal void SetIsAttackableTrue(float n)
    {
        HitBack = n;
        IsAttackable = true;
    }
    internal void SetIsAttackableFalse()
    {
        IsAttackable = false;
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

    internal void OffsetBackward(float n)
    {
        transform.position -= transform.forward * n;
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


    void OnTriggerStay(Collider other)
    {
        if (other.transform.tag != "Player") return;

        biology biology = other.transform.root.GetComponent<biology>();
        if (biology == this) return;

        AnimationState AnimationStates = biology.AnimationStates;
        if (AnimationStates != AnimationState.Punching_1 && AnimationStates != AnimationState.Punching_2) return;

        if (biology.IsAttackable == false) return;

        if (biology.AnimationStates == AnimationState.Punching_1) PlayAnimation(AnimationState.HurtRight);
        if (biology.AnimationStates == AnimationState.Punching_2) PlayAnimation(AnimationState.HurtLeft);
        transform.LookAt(other.transform.root);
        // biology.transform.LookAt(transform.root);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
        transform.position -= transform.forward * biology.HitBack;

        StopAnimator();
        if (Main.IsHittedFlash) StartHitFLash();
        StartShake(other);
        biology.StopAnimator();

        biology.IsAttackable = false;// 這項目控制這一次動畫傷害是否只算一次

        if (Main.IsCameraShake) Main.cam.StartShake(other);

        if (Main.IsHittedFx == false) return;

        GameObject Effect = Instantiate(Main.Effects[2]);
        Effect.transform.position = other.ClosestPoint(transform.position);
    }

    internal void StopAnimator()
    {
        HitStopTime = Main.HitStopTime;
        StartCoroutine("Wait");
    }
    internal void StartShake(Collider other)
    {
        StartCoroutine("Shake", other);
    }
    internal void StartHitFLash()
    {
        StartCoroutine("HitFLash");
    }
    IEnumerator Wait()
    {
        float waitCounter = 0;
        Animator.enabled = false;
        while (waitCounter < HitStopTime)
        {
            waitCounter += Time.fixedDeltaTime;
            //Yield until the next frame
            yield return null;
        }
        Animator.enabled = true;
    }
    IEnumerator HitFLash()
    {
        float waitCounter = 0;
        Color _Color = Material.GetColor("_EmissionColor");
        ModelRender.materials[0].SetColor("_EmissionColor", _Color + 0.65f * Color.white);
        ModelRender.materials[1].SetColor("_EmissionColor", _Color + 0.65f * Color.white);
        while (waitCounter < HitStopTime)
        {
            waitCounter += Time.deltaTime;
            //Yield until the next frame
            yield return null;
        }
        ModelRender.materials[0].SetColor("_EmissionColor", _Color);
        ModelRender.materials[1].SetColor("_EmissionColor", _Color);
    }


    IEnumerator Shake(Collider other)
    {
        float waitCounter = 0;
        Vector3 directon = other.transform.up;
        directon = new Vector3(directon.x, 0, directon.z);
        Vector3 _position = transform.position;
        float lastShakeTime = 0;
        float shakeTimes = HitStopTime / Main.ShakeTimes;
        float _ShakePower = Main.ShakePower;
        while (waitCounter < HitStopTime)
        {
            waitCounter += Time.deltaTime;

            if (waitCounter - lastShakeTime >= shakeTimes && Main.IsHittedShake)
            {
                lastShakeTime = waitCounter;
                transform.position = _position + directon * _ShakePower;
                _ShakePower *= UnityEngine.Random.Range(0.5f, 0.9f);
            }
            yield return null;
        }
        transform.position = _position;
    }

}
