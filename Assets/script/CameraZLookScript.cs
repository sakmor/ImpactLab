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
		LookAtCenter();
	}

	private void LookAtCenter()
	{
		//fixme：一直查找transform會很耗效能
		Vector3 playerPos = Main.Player.CameraPoint.gameObject.transform.position;
		Vector3 playerTargetPos = Main.Player.Target.CameraPoint.gameObject.transform.position;
		Vector3 center = (playerTargetPos - playerPos) * 0.5f + playerPos;
		transform.LookAt(center);
	}

	// void cameraFollow()
	// {
	//     transform.position = followTarget.transform.position + cam2Target;
	// }
	// void setCam2target()
	// {
	//     cam2Target = transform.position - followTarget.transform.position;
	// }

	internal void Set2FollwTargetBack()
	{

		Vector3 back = Main.Player.Target.transform.position - Main.Player.transform.position;
		back = new Vector3(back.x, 0, back.z);
		back = Vector3.Normalize(back);
		back = back * -1.25f;
		transform.position = Main.Player.transform.position + back + Vector3.up * 1.25f;
		transform.LookAt(Main.Player.Target.transform.Find("N1").position);
		Main.Player.transform.LookAt(Main.Player.Target.transform.position);

		//fixme：不要包在這裡
		SetCameraSidePos();
	}

	internal void SetCameraSidePos()
	{
		//fixme：一直查找transform會很耗效能
		Vector3 playerPos = Main.Player.CameraPoint.gameObject.transform.position;
		Vector3 playerTargetPos = Main.Player.Target.CameraPoint.gameObject.transform.position;
		Vector3 n1 = playerTargetPos - playerPos;
		Vector3 n2 = playerTargetPos - transform.position;
		float angle = Vector2.SignedAngle(new Vector2(n1.x, n1.z), new Vector2(n2.x, n2.z));
		transform.position += Main.Player.transform.right * 2;
		if (angle < 0) transform.position += Main.Player.transform.right * -2;

	}


}
