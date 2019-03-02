﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZLookScript : MonoBehaviour
{
    [SerializeField] Main Main;
    private Biology BiologyPlayer;
    Vector3 cam2Target;


    // Use this for initialization
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        set2FollwTargetBack();
        // transform.LookAt(lookTarget.transform.Find("N1").gameObject.transform);
    }
    // void cameraFollow()
    // {
    //     transform.position = followTarget.transform.position + cam2Target;
    // }
    // void setCam2target()
    // {
    //     cam2Target = transform.position - followTarget.transform.position;
    // }

    private void set2FollwTargetBack()
    {
        SetPlayer(Main.Player);
        Vector3 back = BiologyPlayer.Target.transform.position - BiologyPlayer.transform.position;
        back = new Vector3(back.x, 0, back.z);
        back = Vector3.Normalize(back);
        back = back * -1.25f;
        transform.position = BiologyPlayer.transform.position + back + Vector3.up * 1.25f;
        transform.LookAt(BiologyPlayer.Target.transform.Find("N1").position);
        BiologyPlayer.transform.LookAt(BiologyPlayer.Target.transform.position);
    }

    public void SetPlayer(Biology player)
    {
        BiologyPlayer = player;
    }

}
