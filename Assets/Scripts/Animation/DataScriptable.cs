using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataScriptable : ScriptableObject
{
    public List<Transition> transitions;
    public List<string> _boolState;
    public List<string> _floatState;
    public List<string> _intState;
    public List<AnimationData> animationDatas;
}
