using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StateData
{
    public Dictionary<string, bool> _boolMap;
    public Dictionary<string, float> _floatMap;
    public Dictionary<string, int> _intMap;
    public AnimationData _animData;

    public StateData()
    {
        _boolMap = new Dictionary<string, bool>();
        _intMap = new Dictionary<string, int>();
        _floatMap = new Dictionary<string, float>();
    }
}
