using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

namespace TripleBladeHorse
{
	public class TimeManager : GlobalSingleton<TimeManager>
	{
		[SerializeField] float _bulletTimeScale = 0.4f;
		[SerializeField] float _playerBulletTimeScale = 0.6f;
		[SerializeField] float _bulletTimeDuration = 1.5f;

		float _baseFixedDeltaTime;
		float _playerBulletTimeScaleRatio;
		float _bulletTimer;
		float _frameFrozenTimer;
		int pause = 0;

		public static float Time
		{
			get => UnityEngine.Time.time;
		}

		public static float UnscaleTime
		{
			get => UnityEngine.Time.unscaledTime;
		}

		public static float UnscaleDeltaTime
		{
			get => UnityEngine.Time.unscaledDeltaTime;
		}

		public static float DeltaTime
		{
			get => UnityEngine.Time.deltaTime;
		}

		public static float PlayerDeltaTime
		{
			get
			{
				if (Instance._bulletTimer > 0)
				{
					return UnityEngine.Time.deltaTime * Instance._playerBulletTimeScaleRatio;
				}
				return UnityEngine.Time.deltaTime;
			}
		}

		public event Action OnBulletTimeBegin;
		public event Action OnBulletTimeEnd;

		protected override void Awake()
		{
			_baseFixedDeltaTime = UnityEngine.Time.fixedDeltaTime;
			base.Awake();
		}

		private void Update()
		{
			if (pause > 0) return;

			if (_frameFrozenTimer > 0)
			{
				_frameFrozenTimer -= UnscaleDeltaTime;
			}

			if (_bulletTimer > 0)
			{
				_bulletTimer -= UnscaleDeltaTime;
				if (_bulletTimer <= 0)
					OnBulletTimeEnd?.Invoke();
			}
			
			UpdateTimeScale();
		}

		public void Pause()
		{
			pause++;
			UpdateTimeScale();
		}

		public void Unpause()
		{
			pause--;
			UpdateTimeScale();
		}

		public void FrozenFrame(float duration)
		{
			if (_frameFrozenTimer < duration)
				_frameFrozenTimer = duration;

			if (_frameFrozenTimer > 0)
				UnityEngine.Time.timeScale = 0;
		}

		public void ActivateBulletTime()
		{
			var start = _bulletTimer <= 0;
			_playerBulletTimeScaleRatio = _playerBulletTimeScale / _bulletTimeScale;
			_bulletTimer = _bulletTimeDuration;
			UpdateTimeScale();

			if (start)
				OnBulletTimeBegin?.Invoke();
		}

		public void DeactivateBulletTime()
		{
			_bulletTimer = 0;
			UpdateTimeScale();
		}

		void UpdateTimeScale()
		{
			if (pause > 0 || _frameFrozenTimer > 0)
			{
				UnityEngine.Time.timeScale = 0;
				return;
			}
			else if (_bulletTimer > 0)
			{
				UnityEngine.Time.timeScale = _bulletTimeScale;
				UnityEngine.Time.fixedDeltaTime = _baseFixedDeltaTime * _bulletTimeScale;
			}
			else
			{
				UnityEngine.Time.timeScale = 1;
				UnityEngine.Time.fixedDeltaTime = _baseFixedDeltaTime;
			}
		}
	}
}
