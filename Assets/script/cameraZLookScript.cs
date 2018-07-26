using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraZLookScript : MonoBehaviour
{
    public GameObject lookTarget, followTarget;
    Vector3 cam2Target;
    bool isZLook;

    // Use this for initialization
    void Start()
    {
        isZLook = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (isZLook)
        {
            set2FollwTargetBack();
            transform.LookAt(lookTarget.transform);
        }
    }
    void cameraFollow()
    {
        transform.position = followTarget.transform.position + cam2Target;
    }
    void setCam2target()
    {
        cam2Target = transform.position - followTarget.transform.position;
    }
    void setLookTarget(GameObject n)
    {
        lookTarget = n.transform.Find("N1").gameObject;  //todo:當目標沒有N1點的時候會當機
    }
    void set2FollwTargetBack()
    {
        Vector3 back = lookTarget.transform.position - followTarget.transform.position;
        back = new Vector3(back.x, 0, back.z);
        back = Vector3.Normalize(back);
        back = back * -1.25f;
        transform.position = followTarget.transform.position + back + Vector3.up * 1.25f;
    }

    public void setFollowTarget(Transform n)
    {
        followTarget = n.gameObject;
    }

    public void setZLookOff()
    {
        isZLook = false;
    }
    public void setZLookOn(GameObject t)
    {
        if (!isZLook)
        {
            isZLook = true;
        }
        setLookTarget(t);
    }
}
