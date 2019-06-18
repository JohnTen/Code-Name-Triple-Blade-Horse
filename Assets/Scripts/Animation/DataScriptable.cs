using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataScriptable : ScriptableObject
{
    public List<Transition> transitions;
    public Dictionary<string, bool> _boolMap;
    public Dictionary<string, float> _floatMap;
    public Dictionary<string, int> _intMap;
}
