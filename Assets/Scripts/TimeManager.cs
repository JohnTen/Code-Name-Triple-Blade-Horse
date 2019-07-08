using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

namespace TripleBladeHorse
{
	public class TimeManager : GlobalSingleton<TimeManager>
	{
		[SerializeField] float _bulletTimeScale = 0.4f;
		[SerializeField] float _playerBulletTimeScale = 1.5f;

		int bulletTime = 0;
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
				if (Instance.bulletTime > 0)
				{
					return UnityEngine.Time.deltaTime * Instance._playerBulletTimeScale;
				}
				return UnityEngine.Time.deltaTime;
			}
		}

		public static void Pause()
		{
			Instance.pause++;
			UpdateTimeScale();
		}

		public static void Unpause()
		{
			Instance.pause--;
			UpdateTimeScale();
		}

		public static void ActivateBulletTime()
		{
			Instance.bulletTime++;
			UpdateTimeScale();
		}

		public static void DeactivateBulletTime()
		{
			Instance.bulletTime--;
			UpdateTimeScale();
		}

		static void UpdateTimeScale()
		{
			if (Instance.pause > 0)
			{
				UnityEngine.Time.timeScale = 0;
				return;
			}
			else if (Instance.bulletTime > 0)
			{
				UnityEngine.Time.timeScale = Instance._bulletTimeScale;
			}
			else
			{
				UnityEngine.Time.timeScale = 1;
			}
		}
	}
}
