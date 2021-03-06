﻿using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

//我是從下面這個檔案改得
//http://wiki.unity3d.com/index.php?title=MouseOrbitImproved

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class CameraScript : MonoBehaviour
{
    [SerializeField] internal Main Main;

    float distance = 2f;
    float xSpeed = 5.0f;
    float ySpeed = 5.0f;
    float yMinLimit = -20f;
    float yMaxLimit = 80f;
    float distanceMin = 1f;
    float distanceMax = 3f;
    public float x = 0.0f, startX = 0.0f, finalX = 0.0f, y = 0.0f;
    Vector3 cam2Target;
    [SerializeField] private Transform Target;
    private float _TargetY;

    // Use this for initialization
    void Start()
    {
        SetAsEditor();
    }

    void LateUpdate()
    {
        mouseOrbitWhenMouseInput();
        KeepTargetY();
        cameraFollow();
    }
    public void MouseOrbit()
    {
        x += Input.GetAxis("Mouse X") * xSpeed;
        y -= Input.GetAxis("Mouse Y") * ySpeed;
        y = ClampAngle(y, yMinLimit, yMaxLimit);
        Quaternion rotation = Quaternion.Euler(y, x, 0);
        distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);
        Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance);
        transform.rotation = rotation;
        transform.position = position + Target.position;
        SetCame2Target();
    }


    void mouseOrbitWhenMouseInput()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.currentSelectedGameObject)
        {
            MouseOrbit();
        }
    }
    void cameraFollow()
    {
        transform.position = Target.position + cam2Target;
    }
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
    public void SetTarget(Transform n)
    {
        Target = n;
        _TargetY = Target.position.y;
        MouseOrbit();
    }
    private void KeepTargetY()
    {
        Vector3 n = Target.transform.position;
        Target.transform.position = new Vector3(n.x, _TargetY, n.z);
    }

    void SetCame2Target()
    {
        cam2Target = transform.position - Target.position;
    }
    void SetAsEditor()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    public void setPos(Transform n)
    {
        y = n.eulerAngles.x;
        x = n.eulerAngles.y;
        MouseOrbit();
    }

    public void StartShake(Collider other)
    {
        StartCoroutine("Shake", other);
    }

    IEnumerator Shake(Collider other)
    {

        float waitCounter = 0;
        Vector3 directon = other.transform.up;
        directon = new Vector3(directon.x, 0, directon.z);
        Vector3 _position = Target.localPosition;
        float lastShakeTime = 0;
        float shakeTimes = Main.HitStopTime / Main.ShakeTimes;
        float _ShakePower = Main.ShakePower;
        while (waitCounter < Main.HitStopTime)
        {
            waitCounter += Time.deltaTime;

            if (waitCounter - lastShakeTime >= shakeTimes)
            {
                lastShakeTime = waitCounter;
                Target.localPosition = _position - directon * _ShakePower;
                _ShakePower *= UnityEngine.Random.Range(0.5f, 0.9f);
            }
            yield return null;
        }
        Target.localPosition = _position;
    }

}