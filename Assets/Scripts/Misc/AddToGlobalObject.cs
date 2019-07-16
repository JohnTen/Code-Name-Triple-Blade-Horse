using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

public class AddToGlobalObject : MonoSingleton<AddToGlobalObject>
{
	protected override void Awake()
	{
		base.Awake();
		this.transform.SetParent(GlobalObject.Instance.transform);
	}
}
