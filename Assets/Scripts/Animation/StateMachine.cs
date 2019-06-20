using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

public class StateMachine : MonoBehaviour
{
    private StateData stateData;
    private DataScriptable dataScriptable;
    private UnityArmatureComponent _amature;
    private List<Transition> transitions;
    private List<AnimationData> animationDatas;
    private void Start()
    {
        _amature.AddDBEventListener(EventObject.START, this.OnAnimationEventHandler);
        _amature.AddDBEventListener(EventObject.FADE_OUT_COMPLETE, this.OnAnimationEventHandler);
    }

    private void Initialization()
    {
        _amature = this.GetComponent<UnityArmatureComponent>();
        transitions = dataScriptable.transitions;
        animationDatas = new List<AnimationData>(dataScriptable.animationDatas);
        transitions = new List<Transition>();

        foreach (var transition in dataScriptable.transitions)
        {
            transitions.Add(new Transition(transition));
        }

        stateData = new StateData();

        foreach (var key in dataScriptable._boolState)
        {
            stateData._boolMap.Add(key, false);
        }

        foreach (var key in dataScriptable._intState)
        {
            stateData._intMap.Add(key, 0);
        }

        foreach (var key in dataScriptable._floatState)
        {
            stateData._floatMap.Add(key, 0.0f);
        }
    }

    private void AnimationPlay()
    {
        Transition transition = GetTransition(_amature.animationName);
        if(transition != null)
        {
            _amature.animation.FadeIn(
                transition.nextAnim, 
                transition.transitionTime, 
                GetAnimationData(transition.nextAnim).playTimes);
        }
    }

    public string GetCurrentAnimationName()
    {
        return _amature.animationName;
    }

    private Transition GetTransition(string currentAnimaName)
    {
        foreach(var transition in transitions)
        {
            if(transition.currentAnim == currentAnimaName && transition.Test(stateData))
            {
                return transition;
            }
        }
        return null;
    }

    private AnimationData GetAnimationData(string animName)
    {
        foreach(var animationData in animationDatas)
        {
            if(animationData.name == animName)
            {
                return animationData;
            }
        }
        return new AnimationData();
    }

    public void SetBool(string stateName, bool state)
    {
        stateData._boolMap[stateName] = state;
    }

    public void SetFloat(string stateName, float state)
    {
        stateData._floatMap[stateName] = state;
    }

    public void SetInt(string stateName, int state)
    {
        stateData._intMap[stateName] = state;
    }

    private void SetStateAnimData()
    {
        stateData._animData.name = _amature.animationName;
        stateData._animData.playTimes = _amature.animation.animationConfig.playTimes;
        stateData._animData.fadeInTime = _amature.animation.animationConfig.fadeInTime;
    }

    private void OnAnimationEventHandler(string type, EventObject eventObject)
    {
        if (type == EventObject.FADE_OUT_COMPLETE)
        {
            stateData._animData.isFadeOut = true;
            stateData._animData.isStart = false;
        }
        if(type == EventObject.START)
        {
            stateData._animData.isStart = true;
            stateData._animData.isFadeOut = false;
        }
    }
}
