using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

namespace TripleBladeHorse
{
	public class WeakpointMovementHandler : MonoBehaviour
	{
		[System.Serializable] class FormationNamePair : PairedValue<string, WeakpointFormation> { }

		[SerializeField] List<Transform> _weakpoints;
		[SerializeField] List<FormationNamePair> _formationList;

		public void SwitchToFormat(string name)
		{
			var format = _formationList.Find((x) => { return x.Key == name; }).Value;
			if (format == null)
			{
				Debug.LogError("Cannot find " + name + " in formation list.");
				return;
			}
		}
	}
}
