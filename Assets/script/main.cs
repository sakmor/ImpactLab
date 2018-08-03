using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class main : MonoBehaviour
{
	private float KeyboardMoveSpeed = 3.5f;
	[SerializeField] private biology player;
	[SerializeField] internal float ShakeTimes, ShakePower;
	private GameObject[] targets;
	private GameObject target;
	bool isZLook;
	[SerializeField] private UnityEngine.UI.Slider Slider;
	[SerializeField] private UnityEngine.UI.Toggle CameraShake_Toggle;
	[SerializeField] private UnityEngine.UI.Toggle HittedShake_Toggle;
	[SerializeField] private UnityEngine.UI.Toggle HittedFlash_Toggle;
	[SerializeField] private UnityEngine.UI.Toggle HittedFx_Toggle;
	[SerializeField] internal float HitStopTime;
	public bool IsCameraShake, IsHittedShake, IsHittedFlash, IsHittedFx;

	[SerializeField] public cameraScript cam;
	[SerializeField] private cameraZLookScript camZLook;
	[SerializeField] private visualJoyStick visualJoyStick;
	[SerializeField]
	internal List<GameObject> Effects = new List<GameObject>();

	// Use this for initialization
	private void Start()
	{
		UpdateSettingValue();
		targets = GameObject.FindGameObjectsWithTag("blue");
		cam.setTarget(player.CameraPoint);
		camZLook.setFollowTarget(player.transform);
	}

	// Update is called once per frame
	private void Update()
	{
		controlPlayer();
		searchClosetTarget();
		// showTarget();
	}

	private void searchClosetTarget()
	{
		if (isZLook) return;

		float dist = Mathf.Infinity;
		GameObject tempTarget = null;
		foreach (var t in targets)
		{
			float temp = Vector3.Distance(t.transform.position, player.transform.position);
			if (temp < dist)
			{
				tempTarget = t;
				dist = temp;
			}
		}
		target = tempTarget;

	}

	private void showTarget()
	{
		GameObject.Find("Cube").transform.position = target.transform.position + Vector3.up * 1.2f;
	}

	private void controlPlayer()
	{
		Vector3 newDirect = Vector3.zero;

		if (Input.GetKey("left shift"))
		{
			useZLookCam();
			camZLook.setZLookOn(target);
			isZLook = true;
		}
		if (Input.GetKeyUp("left shift"))
		{
			useNormalCam();
			isZLook = false;
		}
		if (visualJoyStick.touch)
		{
			//如果沒在注視模式時，使用一般攝影機座標
			newDirect = transformJoyStickSpace(visualJoyStick.joyStickVec, cam.transform);
			//如果正在注視模式時，使用注視攝影機座標
			if (isZLook) newDirect = transformJoyStickSpace(visualJoyStick.joyStickVec, camZLook.transform);
			player.MoveTo(newDirect / 2.0f);//fixme:0.25要放到外部控制
		}
		if (Input.GetKey("w"))
		{
			newDirect = transformJoyStickSpace(Vector2.up, cam.transform);
			player.MoveTo(newDirect * 0.5f * KeyboardMoveSpeed);//fixme:0.25要放到外部控制
		}
		if (Input.GetKey("a"))
		{
			newDirect = transformJoyStickSpace(Vector2.left, cam.transform);
			player.MoveTo(newDirect * 0.5f * KeyboardMoveSpeed);//fixme:0.25要放到外部控制
		}
		if (Input.GetKey("s"))
		{
			newDirect = transformJoyStickSpace(Vector2.down, cam.transform);
			player.MoveTo(newDirect * 0.5f * KeyboardMoveSpeed);//fixme:0.25要放到外部控制
		}
		if (Input.GetKey("d"))
		{
			newDirect = transformJoyStickSpace(Vector2.right, cam.transform);
			player.MoveTo(newDirect * 0.5f * KeyboardMoveSpeed);//fixme:0.25要放到外部控制s
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
		if (Input.GetMouseButton(1))
		{
			player.AttackButtonDown();
		}
		if (Input.GetKeyDown("space"))
		{
			player.SpaceButtonDown();
		}
		if (Input.anyKey == false)
		{
			player.StopWalking();
		}
	}
	private void useZLookCam()
	{
		cam.GetComponent<Camera>().enabled = false;
		camZLook.GetComponent<Camera>().enabled = true;
	}
	private void useNormalCam()
	{
		cam.setPos(camZLook.transform);
		cam.GetComponent<Camera>().enabled = true;
		camZLook.GetComponent<Camera>().enabled = false;
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
