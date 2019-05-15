using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JTUtility.Interactables
{
	public class TimedSwitch : MonoInteractable
	{
		[SerializeField] float time;

		Timer timer;

		public override void StartInteracting()
		{
			if (!isInteracting)
			{
				InvokeStartInteracting();
			}
			isInteracting = true;

			if (isActivated) return;

			isActivated = true;
			timer.Start(time);
			InvokeActivated();

			if (onActivated != null)
				onActivated.Invoke();
		}

		public override void StopInteracting()
		{
			if (isInteracting)
			{
				InvokeStopInteracting();
			}

			isInteracting = false;
		}

		protected virtual void Awake()
		{
			timer = new Timer();
			timer.OnTimeOut += Timer_OnTimeOut;
		}

		protected void OnDestroy()
		{
			timer.Dispose();
		}

		protected void Timer_OnTimeOut()
		{
			InvokeDeactivated();

			if (onDeactivated != null)
				onDeactivated.Invoke();
			isActivated = false;
		}
	}
}
