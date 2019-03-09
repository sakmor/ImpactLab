﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    private float KeyboardMoveSpeed = 1f;
    [SerializeField] internal Biology Player;
    [SerializeField] internal float ShakeTimes, ShakePower;
    [SerializeField] private GameObject TaregtMark;
    [SerializeField] internal Biology[] AllBiologys;

    private GameObject target;
    bool IsZlook;
    [SerializeField] private UnityEngine.UI.Slider Slider;
    [SerializeField] private UnityEngine.UI.Toggle CameraShake_Toggle;
    [SerializeField] private UnityEngine.UI.Toggle HittedShake_Toggle;
    [SerializeField] private UnityEngine.UI.Toggle HittedFlash_Toggle;
    [SerializeField] private UnityEngine.UI.Toggle HittedFx_Toggle;
    [SerializeField] internal float HitStopTime;
    public bool IsCameraShake, IsHittedShake, IsHittedFlash, IsHittedFx;

    [SerializeField] public CameraScript Cam;
    [SerializeField] private CameraZLookScript CamZook;
    [SerializeField] private VisualJoyStick VisualJoyStick;
    [SerializeField]
    internal List<GameObject> Effects = new List<GameObject>();

    // Use this for initialization
    private void Start()
    {
        UpdateSettingValue();
        SetupCameraSystem();
        AllBiologys = FindObjectsOfType<Biology>();

    }

    // Update is called once per frame
    private void Update()
    {
        controlPlayer();
        showTarget();
    }

    private void SetupCameraSystem()
    {
        Cam.setTarget(Player.CameraPoint);
        CamZook.SetPlayer(Player);
    }

    private void showTarget()
    {
        if (Player.Target == null) { return; }
        TaregtMark.transform.position = Player.Target.transform.position + Vector3.up * 1.2f;
    }

    private void controlPlayer()
    {
        Vector3 newDirect = Vector3.zero;

        if (Input.GetKey("left shift"))
        {
            useZLookCam();
        }

        if (Input.GetKeyUp("left shift"))
        {
            useNormalCam();
        }


        if (VisualJoyStick.IsTouch)
        {
            newDirect = transformJoyStickSpace(VisualJoyStick.joyStickVec, Cam.transform);
            if (IsZlook) newDirect = transformJoyStickSpace(VisualJoyStick.joyStickVec, CamZook.transform);
            Player.MoveTo(newDirect);
        }

        if (Input.GetKey("w"))
        {
            newDirect = transformJoyStickSpace(Vector2.up, Cam.transform);
            if (IsZlook) newDirect = Player.transform.forward.normalized;
            Player.MoveTo(newDirect * KeyboardMoveSpeed);
        }
        if (Input.GetKey("a"))
        {
            newDirect = transformJoyStickSpace(Vector2.left, Cam.transform);
            if (IsZlook) newDirect = -Player.transform.right.normalized;
            Player.MoveTo(newDirect * KeyboardMoveSpeed);
        }
        if (Input.GetKey("s"))
        {
            newDirect = transformJoyStickSpace(Vector2.down, Cam.transform);
            if (IsZlook) newDirect = -Player.transform.forward.normalized;
            Player.MoveTo(newDirect * KeyboardMoveSpeed);
        }
        if (Input.GetKey("d"))
        {
            newDirect = transformJoyStickSpace(Vector2.right, Cam.transform);
            if (IsZlook) newDirect = Player.transform.right.normalized;
            Player.MoveTo(newDirect * KeyboardMoveSpeed);
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            KeyboardMoveSpeed += 0.1f;
            KeyboardMoveSpeed = Mathf.Clamp(KeyboardMoveSpeed, 1, 6);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            KeyboardMoveSpeed -= 0.1f;
            KeyboardMoveSpeed = Mathf.Clamp(KeyboardMoveSpeed, 1, 6);
        }
        if (Input.GetMouseButtonDown(1))
        {
            Player.AttackButtonDown();
        }
        if (Input.GetKeyDown("space"))
        {
            Player.SpaceButtonDown();
        }

    }
    private void useZLookCam()
    {
        IsZlook = true;
        Cam.gameObject.SetActive(false);
        CamZook.gameObject.SetActive(true);
        Player.SetIsZookTrue();
    }
    private void useNormalCam()
    {
        IsZlook = false;
        Cam.setPos(CamZook.transform);
        Cam.gameObject.SetActive(true);
        CamZook.gameObject.SetActive(false);
        Player.SetIsZookFalse();

    }

    //將 參數一 的2D向量，改為 參數二 Transform為座標空間
    Vector3 transformJoyStickSpace(Vector2 vec, Transform t)
    {
        //取得Transform t的朝前、朝右向量
        Vector3 forward = new Vector3(t.transform.forward.x, 0, t.transform.forward.z);
        Vector3 right = new Vector3(t.transform.right.x, 0, t.transform.right.z);

        //將左右向量標準化（變成最短）
        forward = Vector3.Normalize(forward);
        right = Vector3.Normalize(right);

        //將標準化左右向量加上搖桿的放大量
        right *= vec.x;
        forward *= vec.y;

        return forward + right;
    }

    public void UpdateSettingValue()
    {
        HitStopTime = Slider.value;
        IsCameraShake = CameraShake_Toggle.isOn;
        IsHittedShake = HittedShake_Toggle.isOn;
        IsHittedFlash = HittedFlash_Toggle.isOn;
        IsHittedFx = HittedFx_Toggle.isOn;
    }

}
