using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JTUtility;

public class Test : MonoBehaviour
{
	[SerializeField] PlatformEffector2D effector;
	[SerializeField] float minAngle;
	[SerializeField] float maxAngle;
	[SerializeField] float sideAngle;

	private void Update()
	{
		minAngle = -effector.surfaceArc * 0.5f + effector.rotationalOffset;
		maxAngle = effector.surfaceArc * 0.5f + effector.rotationalOffset;
		sideAngle = Mathf.Cos(-effector.rotationalOffset * Mathf.Deg2Rad) * -90;

		Debug.DrawRay(transform.position, Vector2.up.Rotate(minAngle), Color.red);
		Debug.DrawRay(transform.position, Vector2.up.Rotate(maxAngle), Color.red);
		Debug.DrawRay(transform.position, Vector2.up.Rotate(sideAngle), Color.blue);
	}
}
