using System;
using System.Collections.Generic;
using UnityEngine;

namespace JTUtility
{
	public class ObjectPool : GlobalSingleton<ObjectPool>
	{
		/// <summary>
		/// For pooling the GameObjects
		/// </summary>
		[NonSerialized]
		private readonly Dictionary<GameObject, Stack<GameObject>> pool =
			new Dictionary<GameObject, Stack<GameObject>>();

		/// <summary>
		/// For tracking the GameObjects which created by <see cref="ObjectPool"/>
		/// to prevent releasing an object that is not created from this pool.
		/// </summary>
		[NonSerialized]
		private readonly Dictionary<GameObject, GameObject> prefabMap =
			new Dictionary<GameObject, GameObject>();

		/// <summary>
		/// Acquire a object from pool by the prefab of the object.
		/// </summary>
		/// <param name="prefab"></param>
		/// <returns></returns>
		public static GameObject Acquire(GameObject prefab)
		{
			return Instance.AcquireInstance(prefab);
		}

		/// <summary>
		/// Acquire a Component(with GameObject) from pool by the prefab of the Component.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="prefab"></param>
		/// <returns></returns>
		public static T Acquire<T>(T prefab) where T : Component
		{
			if (prefab == null) return null;
			return Acquire(prefab.gameObject).GetComponent<T>();
		}

		/// <summary>
		/// Release a object to pool.
		/// </summary>
		/// <param name="obj"></param>
		public static void Release(GameObject obj)
		{
			Instance.ReleaseInstance(obj);
		}

		/// <summary>
		/// Release a component(with GameObject) to pool.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="prefab"></param>
		public static void Release<T>(T prefab) where T : Component
		{
			if (prefab == null) return;
			Release(prefab.gameObject);
		}

		/// <summary>
		/// Backing method of <see cref="Acquire(GameObject)"/>
		/// </summary>
		/// <param name="prefab"></param>
		/// <returns></returns>
		private GameObject AcquireInstance(GameObject prefab)
		{
			if (prefab == null)
				throw new ArgumentNullException("perfab");

			Stack<GameObject> stack;

			if (!pool.TryGetValue(prefab, out stack))
				pool.Add(prefab, stack = new Stack<GameObject>());

			GameObject ret;
			if (stack.Count > 0)
			{
				ret = stack.Pop();
				ret.SetActive(true);
				ret.transform.parent = null;
			}
			else
			{
				ret = Instantiate(prefab);
				prefabMap[ret] = prefab;
			}

			return ret;
		}

		/// <summary>
		/// Backing method of <see cref="Release(GameObject)"/>
		/// </summary>
		/// <param name="obj"></param>
		private void ReleaseInstance(GameObject obj)
		{
			if (obj == null)
				throw new ArgumentNullException("obj");

			GameObject prefab;
			if (!prefabMap.TryGetValue(obj, out prefab))
				throw new ArgumentException("The object isn't created from this pool.");


			Stack<GameObject> stack;
			if (!pool.TryGetValue(prefab, out stack))
			{
				Debug.LogWarning("Releasing a object with no stack trace, please do some checking.");
				pool.Add(prefab, stack = new Stack<GameObject>());
			}
			
			obj.transform.SetParent(GlobalObject.DeactivedObject.transform);
			obj.SetActive(false);
			stack.Push(obj);
		}
	}
}

