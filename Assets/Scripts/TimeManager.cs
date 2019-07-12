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
			if (pause > 0 || _bulletTimer <= 0) return;
			
			_bulletTimer -= UnscaleDeltaTime;
			if (_bulletTimer > 0) return;
			
			UpdateTimeScale();
			OnBulletTimeEnd?.Invoke();
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

		public void ActivateBulletTime()
		{
			if (_bulletTimer <= 0)
				OnBulletTimeBegin?.Invoke();

			_playerBulletTimeScaleRatio = _playerBulletTimeScale / _bulletTimeScale;
			_bulletTimer = _bulletTimeDuration;
			UpdateTimeScale();
		}

		public void DeactivateBulletTime()
		{
			_bulletTimer = 0;
			UpdateTimeScale();
		}

		void UpdateTimeScale()
		{
			if (pause > 0)
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
