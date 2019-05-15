using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using JTUtility;

namespace JTUtility.Interactables
{
	public abstract class MonoInteractable : MonoBehaviour, IInteractable
	{
		[SerializeField]
		protected bool isInteracting;

		[SerializeField]
		protected bool isActivated;

		[SerializeField]
		protected UnityEvent onActivated;

		[SerializeField]
		protected UnityEvent onDeactivated;

		public virtual bool IsInteracting { get { return isInteracting; } }
		public virtual bool IsActivated { get { return isActivated; } }

		public virtual event Action OnStartInteracting;
		public virtual event Action OnKeepInteracting;
		public virtual event Action OnStopInteracting;

		public virtual event Action OnActivated;
		public virtual event Action OnDeactivated;

		protected void InvokeStartInteracting() { if (OnStartInteracting != null) OnStartInteracting(); }
		protected void InvokeKeepInteracting() { if (OnKeepInteracting != null) OnKeepInteracting(); }
		protected void InvokeStopInteracting() { if (OnStopInteracting != null) OnStopInteracting(); }
		protected void InvokeActivated() { if (OnActivated != null) OnActivated(); }
		protected void InvokeDeactivated() { if (OnDeactivated != null) OnDeactivated(); }

		public virtual void StartInteracting()
		{
			SetInteractingStatus(true);
		}

		public virtual void StopInteracting()
		{
			SetInteractingStatus(false);
		}

		protected virtual void SetActiveStatus(bool active)
		{
			if (active == isActivated) return;
			isActivated = active;

			if (isActivated)
			{
				InvokeActivated();
				if (onActivated != null)
					onActivated.Invoke();
			}
			else
			{
				InvokeDeactivated();
				if (onDeactivated != null)
					onDeactivated.Invoke();
			}
		}

		protected virtual void SetInteractingStatus(bool interacting)
		{
			if (interacting == isInteracting) return;
			isInteracting = interacting;

			if (isInteracting)
			{
				InvokeStartInteracting();
			}
			else
			{
				InvokeStopInteracting();
			}
		}

		protected virtual void Update()
		{
			if (isInteracting)
			{
				InvokeKeepInteracting();
			}
		}
	}
}
