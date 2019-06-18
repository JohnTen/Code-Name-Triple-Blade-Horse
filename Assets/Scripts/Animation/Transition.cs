using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

public class Transition
{
    public string currentAnim;
    public string nextAnim;

    Func<bool, StateData> rule;
    public float transitionTime;

    public Transition(string currentAnim, string nextAnim, float transitionTime, Func<bool, StateData> rule)
    {
        this.currentAnim = currentAnim;
        this.nextAnim = nextAnim;
        this.transitionTime = transitionTime;
        this.rule = rule;
    }

    public bool Test( StateData stateData)
    {
        return rule(stateData);
    }
}
