using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TripleBladeHorse
{
	public class Climbable : MonoBehaviour, ICanStickKnife
	{
		[SerializeField] bool _canStick;
		[SerializeField] bool _canPullingJump;
		[SerializeField] int _restoredStamina;
		[SerializeField] int _maxStuckedKnife;
		[SerializeField, Range(0f, 1f)] float _pullForceFactor;

		[Header("Debug")]
		[SerializeField] List<GameObject> stuckObj;

		public bool CanStick => _canStick;
		public bool CanPullingJump => _canPullingJump;
		public int RestoredStamina => _restoredStamina;
		public float PullForceFactor => _pullForceFactor;

		public bool TryStick(GameObject obj)
		{
			if (stuckObj.Count >= _maxStuckedKnife)
				return false;

			stuckObj.Add(obj);
			return true;
		}

		public bool TryTakeOut(GameObject obj)
		{
			if (!stuckObj.Contains(obj))
				return false;

			stuckObj.Remove(obj);
			return true;
		}


	}
}
