using UnityEngine;
using UnityEngine.EventSystems;

//我是從下面這個檔案改得
//http://wiki.unity3d.com/index.php?title=MouseOrbitImproved

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class cameraScript : MonoBehaviour
{
    bool isZLook;
    float distance = 2f;
    float xSpeed = 5.0f;
    float ySpeed = 5.0f;
    float yMinLimit = -20f;
    float yMaxLimit = 80f;
    float distanceMin = 1f;
    float distanceMax = 3f;
    private main main;
    public float x = 0.0f, startX = 0.0f, finalX = 0.0f, y = 0.0f;
    Vector3 cam2Target;
    [SerializeField] private Transform target;
    private float _targetY;

    // Use this for initialization
    void Start()
    {
        isZLook = false;
        setAsEditor();
    }

    void LateUpdate()
    {
        mouseOrbitWhenMouseInput();
        KeepTargetY();
        cameraFollow();
    }
    public void mouseOrbit()
    {
        x += Input.GetAxis("Mouse X") * xSpeed;
        y -= Input.GetAxis("Mouse Y") * ySpeed;
        y = ClampAngle(y, yMinLimit, yMaxLimit);
        Quaternion rotation = Quaternion.Euler(y, x, 0);
        distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);
        Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance);
        transform.rotation = rotation;
        transform.position = position + target.position;
        setCame2target();
    }


    void mouseOrbitWhenMouseInput()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.currentSelectedGameObject)
        {
            mouseOrbit();
        }
    }
    void cameraFollow()
    {
        transform.position = target.position + cam2Target;
    }
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
    public void setTarget(Transform n)
    {
        target = n;
        _targetY = target.position.y;
        mouseOrbit();
    }
    private void KeepTargetY()
    {
        Vector3 n = target.transform.position;
        target.transform.position = new Vector3(n.x, _targetY, n.z);
    }

    void setCame2target()
    {
        cam2Target = transform.position - target.position;
    }
    void setAsEditor()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    public void setPos(Transform n)
    {
        y = n.eulerAngles.x;
        x = n.eulerAngles.y;
        mouseOrbit();
    }

}