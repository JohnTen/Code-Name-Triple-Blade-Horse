using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

[System.Serializable]
public struct AnimationData
{
    public string name;  //动画名称，Animation name
    public float timeScale; //动画播放速度 The speed of playing animation
    public int playTimes; //动画播放的循环次数
    public float duration; //动画播放的持续时间 The duration of current animation
    public bool isFadeOut; //动画是否淡出 Is animation fading out
    public bool isStart; //动画是否已经开始 Is animation started
    public float fadeInTime;
}


