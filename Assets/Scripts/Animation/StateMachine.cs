using System.Collections.Generic;
using UnityEngine;
using DragonBones;
using JTUtility;

public class StateMachine : MonoBehaviour
{
    private StateData stateData;
    private DataScriptable dataScriptable;
    private UnityArmatureComponent _amature;
    private List<Transition> transitions;
    private List<AnimationData> animationDatas;
    public event Action<string, MyEventArgs> ReceiveFrameEvent;
    private MyEventArgs eventArgs;
    Transition transitionTest = null;
    private void Awake()
    {
        _amature = this.GetComponent<UnityArmatureComponent>();

    }
    private void Start()
    {
       
        Initialization();
        _amature.AddDBEventListener(EventObject.START, OnAnimationEventHandler);
        _amature.AddDBEventListener(EventObject.FADE_OUT_COMPLETE, OnAnimationEventHandler);
        _amature.AddDBEventListener(EventObject.FADE_IN, OnAnimationEventHandler);
        _amature.AddDBEventListener(EventObject.FRAME_EVENT, OnFrameEventHandler);
    }

    private void Initialization()
    {
        dataScriptable = ScriptableObject.CreateInstance<DataScriptable>();
        //transitions = dataScriptable.transitions;
        animationDatas = new List<AnimationData>(dataScriptable.animationDatas);
        dataScriptable.transitions = new List<Transition>();
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

    private void Update()
    {
        AnimationPlay();
        //print(_amature.animationName);
        print(dataScriptable.transitions[0].currentAnim);
    }

    private void AnimationPlay()
    {
        //transitionTest = GetTransition(GetCurrentAnimationName());
        transitionTest = dataScriptable.transitions[0];
        if (transitionTest != null && Input.GetKeyDown(KeyCode.N))
        {
            _amature.animation.FadeIn(
                        transitionTest.nextAnim,
                        transitionTest.transitionTime,
                        GetAnimationData(transitionTest.nextAnim).playTimes);

        }
    }

    public string GetCurrentAnimationName()
    {
        return _amature.animationName;
    }

    private Transition GetTransition(string currentAnimaName)
    {
        foreach (var transition in transitions)
        {
            if (transition.currentAnim == currentAnimaName && 
                transition.Test(stateData) && 
                transition != null)
            {
                return transition;
            }
        }
        return null;
    }

    public StateData GetStateData()
    {
        return stateData;
    }

    private AnimationData GetAnimationData(string animName)
    {
        foreach (var animationData in animationDatas)
        {
            if (animationData.name == animName)
            {
                return animationData;
            }
        }
        return new AnimationData();
    }

    public bool GetBool(string stateName)
    {
        return stateData._boolMap[stateName];
    }

    public float GetFloat(string stateName)
    {
        return stateData._floatMap[stateName];
    }

    public int GetInt(string stateName)
    {
        return stateData._intMap[stateName];
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
        if (type == EventObject.START)
        {
            stateData._animData.isStart = true;
            stateData._animData.isCompleted = false;
            stateData._animData.isFadeInComplete = false;
        }
        if (type == EventObject.FADE_IN)
        {
            stateData._animData.isFadeIn = true;

        }
        if (type == EventObject.FADE_IN_COMPLETE)
        {
            stateData._animData.isFadeInComplete = true;
            //stateData._animData.isCompleted = true;
            //stateData._animData.isStart = true;
        }
        if (type == EventObject.COMPLETE)
        {
            stateData._animData.isCompleted = true;
            SetStateAnimData();
        }
    }

    private void OnFrameEventHandler(string type , EventObject eventObject)
    {
        foreach(var key in stateData._boolMap)
        {
            if(eventObject.name == key.Key)
            {
                stateData._boolMap[key.Key] = eventObject.data.GetInt(0) != 0;
                return;
            }
        }
        foreach (var key in dataScriptable._intState)
        {
            if (eventObject.name == key)
            {
                stateData._intMap[key] = eventObject.data.GetInt(0);
                return;
            }
        }
        foreach (var key in dataScriptable._floatState)
        {
            if (eventObject.name == key)
            {
                stateData._floatMap[key] = eventObject.data.GetFloat(0);
                return;
            }
        }
        eventArgs = new MyEventArgs(
                        eventObject.animationState.isPlaying,
                        eventObject.animationState.playTimes,
                        eventObject.animationState.currentPlayTimes);
        ReceiveFrameEvent?.Invoke(eventObject.name, eventArgs);


    }
}