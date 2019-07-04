using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

namespace TripleBladeHorse.Animation
{
	[System.Serializable]
	public class FSMState : ISerializationCallbackReceiver
	{
		[System.NonSerialized] public Dictionary<string, bool> _boolMap;
		[System.NonSerialized] public Dictionary<string, bool> _toggleMap;
		[System.NonSerialized] public Dictionary<string, float> _floatMap;
		[System.NonSerialized] public Dictionary<string, int> _intMap;
		[SerializeField] List<StrBoolPair> boolPairs;
		[SerializeField] List<StrBoolPair> togglePairs;
		[SerializeField] List<StrFloatPair> floatPairs;
		[SerializeField] List<StrIntPair> intPairs;
		public Animation _current;
		public Animation _previous;

		public FSMState()
		{
			_boolMap = new Dictionary<string, bool>();
			_toggleMap = new Dictionary<string, bool>();
			_intMap = new Dictionary<string, int>();
			_floatMap = new Dictionary<string, float>();
			boolPairs = new List<StrBoolPair>();
			togglePairs = new List<StrBoolPair>();
			floatPairs = new List<StrFloatPair>();
			intPairs = new List<StrIntPair>();
		}

		public void OnAfterDeserialize()
		{
			foreach (var item in boolPairs)
			{
				_boolMap[item.Key] = item.Value;
			}
			foreach (var item in togglePairs)
			{
				_toggleMap[item.Key] = item.Value;
			}
			foreach (var item in floatPairs)
			{
				_floatMap[item.Key] = item.Value;
			}
			foreach (var item in intPairs)
			{
				_intMap[item.Key] = item.Value;
			}
		}

		public void OnBeforeSerialize()
		{
			boolPairs.Clear();
			foreach (var item in _boolMap)
			{
				boolPairs.Add(new StrBoolPair() { Key = item.Key, Value = item.Value });
			}

			togglePairs.Clear();
			foreach (var item in _toggleMap)
			{
				togglePairs.Add(new StrBoolPair() { Key = item.Key, Value = item.Value });
			}

			floatPairs.Clear();
			foreach (var item in _floatMap)
			{
				floatPairs.Add(new StrFloatPair() { Key = item.Key, Value = item.Value });
			}

			intPairs.Clear();
			foreach (var item in _intMap)
			{
				intPairs.Add(new StrIntPair() { Key = item.Key, Value = item.Value });
			}
		}
	}
}