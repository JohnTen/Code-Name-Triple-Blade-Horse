using System;
using UnityEngine;
using UnityEngine.Events;

namespace JTUtility
{
	[Serializable] public class IntEvent : UnityEvent<int> { }
	[Serializable] public class FloatEvent : UnityEvent<float> { }
	[Serializable] public class BoolEvent : UnityEvent<bool> { }
	[Serializable] public class StringEvent : UnityEvent<string> { }
	[Serializable] public class Vector2Event : UnityEvent<Vector2> { }
	[Serializable] public class Vector3Event : UnityEvent<Vector3> { }
	[Serializable] public class QuaternionEvent : UnityEvent<Quaternion> { }
	[Serializable] public class TransformEvent : UnityEvent<Transform> { }
	[Serializable] public class GameObjectEvent : UnityEvent<GameObject> { }
}
