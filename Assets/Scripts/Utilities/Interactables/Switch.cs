using System;
using System.Collections.Generic;
using UnityEngine;

namespace JTUtility.Interactables
{
	public class Switch : MonoInteractable
	{
		public override void StartInteracting()
		{
			SetActiveStatus(!isActivated);

			base.StartInteracting();
		}
	}
}
