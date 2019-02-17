using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biology : MonoBehaviour
{
    [SerializeField] internal main Main;
    private float HitStopTime;
    private Vector3 GoalPos;
    [SerializeField] private float Speed;

    private Material Material;
    [SerializeField] private float WalkStep, RunStep;
    public Biology Target;
    public bool isRandom;
    [SerializeField] private AnimationState AnimationStates;
    [SerializeField] internal Transform CameraPoint;
    [SerializeField] internal Animator Animator;
    [SerializeField] private Renderer ModelRender;

    public bool IsAttackable;

    [SerializeField] private Animation[] Animations;
    public enum AnimationState
    {
        Idle, Walking, Running, Punching_1, Punching_2, Punching_3, HurtLeft, HurtRight
    }
    private void Start()
    {
        GoalPos = transform.position;
        InvokeRepeating("randomMove", 1f, UnityEngine.Random.Range(3f, 5f));
        Material = Instantiate(ModelRender.materials[0]);
        ModelRender.materials[0] = ModelRender.materials[1] = Material;

    }
    private void Update()
    {

    }

    internal void SearchCloseBiology()
    {
        float dist = Mathf.Infinity;
        Biology tempTarget = null;
        foreach (var t in Main.AllBiologys)
        {
            if (t == this) return;
            float temp = Vector3.Distance(t.transform.position, transform.position);
            if (temp < dist)
            {
                tempTarget = t;
                dist = temp;
            }
        }
        SetTaregt(tempTarget);

    }

    private void SetTaregt(Biology t)
    {
        Target = t;
    }

    internal void SpaceButtonDown()
    {
        // StopWalking();
        // PlayAnimation(AnimationState.Punching_3);
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

    // Call by AnimationEvent
    internal void SetIsMoveableTrue() { Animator.SetBool("IsMoveable", true); } //todo:IsMoveable、IsAttackabl、是不是該放到 Animator 內呢...
    internal void SetIsMoveableFalse() { Animator.SetBool("IsMoveable", false); }
    internal void SetIsAttackableTrue() { IsAttackable = true; }
    internal void SetIsAttackableFalse() { IsAttackable = false; }
    internal void SetIsPunchNextTrue() { Animator.SetBool("IsPunchNext", true); }
    internal void SetIsPunchNextFalse() { Animator.SetBool("IsPunchNext", false); }
    internal void SetIsPunchingTrue() { Animator.SetBool("IsPunching", true); }
    internal void SetIsPunchingFalse() { Animator.SetBool("IsPunching", false); }
    internal void SetIsHurtLeftTrue() { Animator.SetBool("IsHurtLeft", true); }
    internal void SetIsHurtLeftFalse() { Animator.SetBool("IsHurtLeft", false); }
    internal void SetIsHurtRightTrue() { Animator.SetBool("IsHurtRight", true); }
    internal void SetIsHurtRightFalse() { Animator.SetBool("IsHurtRight", false); }
    internal void SetSpeedPercent(float t) { Animator.SetFloat("SpeedPercent", t); }
    private void SetMoveSpeed(float t) { Animator.SetFloat("MoveSpeed", t); }


    internal void MoveTo(Vector3 direct)
    {
        if (Animator.GetBool("IsMoveable") == false) { SetMoveSpeed(0); return; }

        GoalPos = transform.position + direct;
        SetSpeedPercent(direct.magnitude);
        float finalSpeed = Speed * (direct.magnitude);
        SetMoveSpeed(finalSpeed);
        Animator.SetFloat("WalkStep", WalkStep * finalSpeed);
        Animator.SetFloat("RunStep", RunStep * finalSpeed);
        transform.position = Vector3.MoveTowards(transform.position, GoalPos, finalSpeed * Time.deltaTime);
        FaceTarget(GoalPos, Speed * 10.0f);

    }

    internal void AttackButtonDown()
    {
        SetIsMoveableFalse();
        SetIsPunchingTrue();
        // if (Animator.GetInteger("PuchingState") == 2 && IsAttackable == false) { SetPuchingState(0); }
        // if (Animator.GetInteger("PuchingState") == 1 && IsAttackable == false) { SetPuchingState(2); }
        // if (Animator.GetInteger("PuchingState") == 0 && IsAttackable == false) { SetPuchingState(1); }

    }

    internal void OffsetBackward(float n)
    {
        transform.position -= transform.forward * n;
    }

    internal void StopWalking()
    {
        // if (AnimationStates == AnimationState.Punching_1) return;
        // if (AnimationStates == AnimationState.Punching_2) return;
        // SetIsMoveableFalse();
        SetSpeedPercent(0);
        SetMoveSpeed(0);
    }

    internal void UpdateAnimationState(AnimationState animationState)
    {
        AnimationStates = animationState;

    }


    private void FaceTarget(Vector3 etarget, float espeed)
    {
        Vector3 targetDir = etarget - this.transform.position;
        float step = espeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(this.transform.forward, targetDir, step, 0.0f);
        this.transform.rotation = Quaternion.LookRotation(newDir);
    }


    void OnTriggerStay(Collider other)
    {
        if (other.transform.root.GetComponent<Biology>() == null) return;
        Biology hitMeBiology = other.transform.root.GetComponent<Biology>();
        if (hitMeBiology == this) return;

        AnimationState AnimationStates = hitMeBiology.AnimationStates;

        if (hitMeBiology.IsAttackable == false) return;

        if (other.gameObject.name == "SphereRight") SetIsHurtRightTrue();
        if (other.gameObject.name == "SphereLeft") SetIsHurtLeftTrue();
        transform.LookAt(other.transform.root);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);


        StopAnimator();
        if (Main.IsHittedFlash) StartHitFLash();
        StartShake(other);
        hitMeBiology.StopAnimator();

        hitMeBiology.SetIsAttackableFalse();// 這項目控制這一次動畫傷害是否只算一次

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
