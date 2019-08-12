using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerAgent : MonoBehaviour
{
    public float FadeInTime
    {
        get => SoundManager.Instance.FadeInTime;
        set => SoundManager.Instance.FadeInTime = value;
    }
    public float FadeOutTime
    {
        get => SoundManager.Instance.FadeOutTime;
        set => SoundManager.Instance.FadeOutTime = value;
    }

    public void Play(string label)
    {
        SoundManager.Instance.Play(label);
    }
    public void Stop(string label)
    {
        SoundManager.Instance.Stop(label);
    }

    public void FadeIn(string label)
    {
        SoundManager.Instance.FadeIn(label);
    }
    public void FadeOut(string label)
    {
        SoundManager.Instance.FadeOut(label);
    }

    public void GroupFadeIn(string label)
    {
        SoundManager.Instance.GroupFadeIn(label);
    }
    public void GroupFadeOut(string label)
    {
        SoundManager.Instance.GroupFadeOut(label);
    }

    public bool IsPlaying(string label)
    {
        return SoundManager.IsPlaying(label);
    }
}
