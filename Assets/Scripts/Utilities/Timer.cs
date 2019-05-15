using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace JTUtility
{
	public class Timer : IDisposable
	{
		class InnerTImer : MonoBehaviour
		{
			public List<int> keys = new List<int>();
			public Dictionary<int, Timer> timers = new Dictionary<int, Timer>();

			private void Update()
			{
				for (int i = 0; i < keys.Count; i ++)
				{
					var k = keys[i];
					Timer timer;
					if (!timers.TryGetValue(k, out timer))
					{
						keys.Remove(k);
						i--;
					}
					else if (timer.disposing)
					{
						keys.Remove(k);
						timers.Remove(k);
						i--;
					}
				}

				foreach (var k in keys)
				{
					if (timers[k].timeLeft <= 0) continue;

					if (timers[k].RealTime)
						timers[k].timeLeft -= Time.unscaledDeltaTime;
					else
						timers[k].timeLeft -= Time.deltaTime;
					if (timers[k].timeLeft > 0) continue;
					
					if (timers[k].Repeat)
					{
						timers[k].timeLeft += timers[k].startTime;
					}

					if (timers[k].OnTimeOut != null)
						timers[k].OnTimeOut.Invoke();
				}
			}
		}

		static InnerTImer innerTimer;
		static InnerTImer InnerTimer
		{
			get
			{
				if (innerTimer != null)
					return innerTimer;

				innerTimer = GlobalObject.GetOrAddComponent<InnerTImer>();
				return innerTimer;
			}
		}

		private int timerID;
		private bool disposing = false;

		private float startTime;
		private float timeLeft;

		public event Action OnTimeOut;

		public Timer()
		{
			timerID = GetHashCode();
			InnerTimer.keys.Add(timerID);
			InnerTimer.timers.Add(timerID, this);
		}

		public bool Repeat { get; set; }
		public bool RealTime { get; set; }

		public bool IsReachedTime()
		{
			return timeLeft <= 0;
		}

		public float PassedTime
		{
			get { return startTime - timeLeft; }
		}

		public float LeftTime
		{
			get { return timeLeft; }
		}

		public float PassedPercentage
		{
			get { return timeLeft / startTime; }
		}

		public void Start(float sec)
		{
			startTime = sec;
			timeLeft = sec;
		}

		public void Abort()
		{
			timeLeft = -1;
		}

		public void Dispose()
		{
			disposing = true;
		}
	}
}

