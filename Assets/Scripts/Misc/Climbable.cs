using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TripleBladeHorse.Combat;

namespace TripleBladeHorse
{
	public class Climbable : MonoBehaviour, ICanStickKnife, ICanHandleRespawn
	{
		[SerializeField] bool _canStick;
		[SerializeField] bool _canPullingJump;
		[SerializeField] int _restoredStamina;
		[SerializeField] int _maxStuckedKnife = 1;
		[SerializeField] int _numberOfEnergyBall = 1;
		[SerializeField, Range(0f, 1f)] float _pullForceFactor;
		[SerializeField] EnergyBall _enegryBallPrefab;

		[SerializeField] UnityEvent _OnStab;
		[SerializeField] UnityEvent _OnDraw;
		[SerializeField] UnityEvent _OnExhausted;

		[Header("Debug")]
		[SerializeField] List<GameObject> stuckObj;
		[SerializeField] Rigidbody2D _rigidbody;
		[SerializeField] int _generatedEnergyBalls;

		public bool CanStick => _canStick;
		public bool CanPullingJump => _canPullingJump;
		public int RestoredStamina => _restoredStamina;
		public float PullForceFactor => _pullForceFactor;

		private void Awake()
		{
			_rigidbody = GetComponent<Rigidbody2D>();
		}

		private void OnDisable()
		{
			for (int i = 0; i < stuckObj.Count; i++)
			{
				var knife = stuckObj[i].GetComponent<ThrowingKnife>();
				if (!knife) continue;
				knife.Withdraw();
			}
		}

		public bool TryStick(GameObject obj)
		{
			if (stuckObj.Count >= _maxStuckedKnife)
				return false;

			print("Stab");
			_OnStab.Invoke();
			stuckObj.Add(obj);
			return true;
		}

		public bool TryPullOut(GameObject obj, ref Vector2 pullingVelocity)
		{
			if (!stuckObj.Contains(obj))
				return false;

			if (_rigidbody && PullForceFactor < 1)
			{
				var toObj = obj.transform.position - this.transform.position;
				toObj.Normalize();
				// pulling obj
			}

			if (_restoredStamina > 0 
                && _enegryBallPrefab != null 
                && (_generatedEnergyBalls < _numberOfEnergyBall
                || _numberOfEnergyBall <= 0))
			{
				var energyBall = 
					Instantiate(
						_enegryBallPrefab.gameObject, 
						transform.position,
						transform.rotation).GetComponent<EnergyBall>();

				energyBall.ReplenishAmount = _restoredStamina;
				_generatedEnergyBalls++;

				if (_generatedEnergyBalls >= _numberOfEnergyBall)
					_OnExhausted?.Invoke();
			}

			print("Draw");
			_OnDraw.Invoke();
			stuckObj.Remove(obj);
			return true;
		}

		public void Remove(GameObject obj)
		{
			if (!stuckObj.Contains(obj))
				return;
			stuckObj.Remove(obj);
		}

		public void Respawn()
		{
			_generatedEnergyBalls = 0;
		}
	}
}
