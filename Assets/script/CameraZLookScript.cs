using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZLookScript : MonoBehaviour
{
    [SerializeField] Main Main;
    private Vector3 cam2Target;



    // Use this for initialization
    private void Start()
    {
        cam2Target = transform.position - Main.Player.transform.position;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        transform.LookAt(Main.Player.Target.CameraPoint.gameObject.transform);
        // transform.position = Main.Player.transform.position + cam2Target;
    }
    // void cameraFollow()
    // {
    //     transform.position = followTarget.transform.position + cam2Target;
    // }
    // void setCam2target()
    // {
    //     cam2Target = transform.position - followTarget.transform.position;
    // }

    internal void set2FollwTargetBack()
    {

        Vector3 back = Main.Player.Target.transform.position - Main.Player.transform.position;
        back = new Vector3(back.x, 0, back.z);
        back = Vector3.Normalize(back);
        back = back * -1.25f;
        transform.position = Main.Player.transform.position + back + Vector3.up * 1.25f;
        transform.LookAt(Main.Player.Target.transform.Find("N1").position);
        Main.Player.transform.LookAt(Main.Player.Target.transform.position);
    }


}
