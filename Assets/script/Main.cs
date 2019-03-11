using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    private float KeyboardMoveSpeed = 1f;
    [SerializeField] internal Biology Player;
    [SerializeField] internal float ShakeTimes, ShakePower;
    [SerializeField] private GameObject TaregtMark;
    [SerializeField] internal Biology[] AllBiologys;
    [SerializeField] internal CameraZLookScript CameraZLookScript;

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
    [SerializeField] public Animator ZlookUIAnimator;
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
        PlayerController();
        ShowTarget();
    }

    private void SetupCameraSystem()
    {
        Cam.SetTarget(Player.CameraPoint);
    }

    private void ShowTarget()
    {
        if (Player.Target == null) { return; }
        TaregtMark.transform.position = Player.Target.transform.position + Vector3.up * 1.2f;
    }

    private void PlayerController()
    {
        Vector3 newDirect = Vector3.zero;

        if (Input.GetKey("left shift"))
        {
            UseZLookCam();
        }

        if (Input.GetKeyUp("left shift"))
        {
            UseNomralCam();
        }

        if (VisualJoyStick.IsTouch)
        {
            newDirect = transformPlayerMoveDirect(VisualJoyStick.joyStickVec);
            Player.MoveTo(newDirect);
        }

        if (Input.GetKey("w"))
        {
            newDirect = transformPlayerMoveDirect(Vector2.up);
            Player.MoveTo(newDirect * KeyboardMoveSpeed);
        }
        if (Input.GetKey("a"))
        {
            newDirect = transformPlayerMoveDirect(Vector2.left);
            Player.MoveTo(newDirect * KeyboardMoveSpeed);
        }
        if (Input.GetKey("s"))
        {
            newDirect = transformPlayerMoveDirect(Vector2.down);
            Player.MoveTo(newDirect * KeyboardMoveSpeed);
        }
        if (Input.GetKey("d"))
        {
            newDirect = transformPlayerMoveDirect(Vector2.right);
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
    private void UseZLookCam()
    {
        //fixme:GetComponet很耗效能
        if (IsZlook == false) CamZook.gameObject.GetComponent<CameraZLookScript>().Set2FollwTargetBack();
        IsZlook = true;
        // Player.transform.LookAt(Player.Target.transform.position);
        ZlookUIAnimator.SetBool("IsZLook", true);
        //fixme:GetComponent很耗效能
        Cam.gameObject.GetComponent<Camera>().enabled = false;
        CamZook.gameObject.GetComponent<Camera>().enabled = true;
        CamZook.gameObject.GetComponent<CameraZLookScript>().enabled = true;
        Player.SetIsZookTrue();
    }
    private void UseNomralCam()
    {
        IsZlook = false;
        ZlookUIAnimator.SetBool("IsZLook", false);
        // Cam.setPos(CamZook.transform);
        //fixme:GetComponent很耗效能
        Cam.gameObject.GetComponent<Camera>().enabled = true;
        CamZook.gameObject.GetComponent<Camera>().enabled = false;
        CamZook.gameObject.GetComponent<CameraZLookScript>().enabled = false;
        Player.SetIsZookFalse();

    }

    Vector3 transformPlayerMoveDirect(Vector2 vec)
    {
        Vector3 forward = Vector3.zero;
        Vector3 right = Vector3.zero;

        if (IsZlook == false)
        {
            forward = Cam.transform.forward;
            right = Cam.transform.right;
        }

        // Fixme:
        // 這裡的繞圈寫法有誤差在，一直繞著目標做橫移會越跑"越遠"或"越近"
        // 要寫到沒有誤差也不是不可能，只要求出下面數值即可：
        // --- 假設圓點為 Player.Target.transform.position
        // --- 該圓半徑為 Vector3.Distance(Player.Target.transform.position,Player.transform.position);
        // --- 求right向量與圓上之做為新的right，應該就可以了。
        // --- ....好麻煩...這個誤差應該玩家無法察覺
        if (IsZlook == true)
        {
            forward = Player.Target.transform.position - Player.transform.position;
            right = Quaternion.AngleAxis(90f - vec.x, Vector3.up) * forward.normalized;
            float dist = Vector3.Distance(Player.transform.position, Player.Target.transform.position);
            Debug.Log(dist);
        }

        //將標準化左右向量加上搖桿的放大量
        right *= vec.x;
        forward *= vec.y;

        return Vector3.Normalize(forward + right);
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
