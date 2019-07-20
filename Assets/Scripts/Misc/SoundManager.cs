using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using JTUtility;

[System.Serializable]
public class SoundClips
{
	public string label;
	public AudioClip[] sounds;
	public AudioMixerGroup output;
	public bool mute;
	public bool playOnAwake = true;
	public bool loop;

	[Range(0f, 1f)] public float volume = 1;
	[Range(-3f, 3f)] public float pitch = 1;

	public float standardVolume = 1;
	public float standardPitch = 1;

	[Header("Debug")]
	public AudioSource source;

	public bool IsActivated { get; private set; }

	public bool IsPlaying
	{
		get { return source.isPlaying; }
	}

	public void SetSource(AudioSource s)
	{
		source = s;
		source.outputAudioMixerGroup = output;
		source.mute = mute;
		source.playOnAwake = playOnAwake;
		source.loop = loop;
		source.volume = volume;
		source.pitch = pitch;
	}

	public void Play()
	{
		if (sounds.Length <= 0) return;
		var index = Random.Range(0, sounds.Length);

		SetSource(source);
		source.clip = sounds[index];
		source.Play();
		IsActivated = true;
	}

	public void Stop()
	{
		source.Stop();
		IsActivated = false;
	}
}

public class SoundManager : MonoSingleton<SoundManager>
{
	[SerializeField] SoundClips[] clips;

	[Header("Debug")]
	[SerializeField] GameObject soundObject;
	[SerializeField] List<AudioSource> sources;

	public float FadeInTime { get; set; } = 1;
	public float FadeOutTime { get; set; } = 3;

	protected override void Awake()
	{
		base.Awake();

		soundObject = new GameObject("Sound Object");
		soundObject.transform.SetParent(transform);
		sources = new List<AudioSource>();

		for (int i = 0; i < clips.Length; i ++)
		{
			if (clips[i].source != null) continue;

			clips[i].SetSource(soundObject.AddComponent<AudioSource>());
			clips[i].standardVolume = clips[i].volume;
			clips[i].standardPitch = clips[i].pitch;
			if (clips[i].playOnAwake)
				clips[i].Play();

			sources.Add(clips[i].source);
		}
	}

	protected void Update()
	{
		var foundNull = false;
		for (int i = 0; i < sources.Count; i++)
		{
			if (sources[i] != null) continue;
			foundNull = true;
			sources.RemoveAt(i);
			i--;
		}

		if (foundNull)
		{
			soundObject = new GameObject("Sound Object");
			soundObject.transform.SetParent(transform);
			print(transform);

			foreach (var s in clips)
			{
				s.SetSource(soundObject.AddComponent<AudioSource>());
				if (s.playOnAwake)
					s.Play();
				sources.Add(s.source);
			}
		}

		foreach (var s in clips)
		{
			s.SetSource(s.source);
			if (s.IsPlaying) continue;

			if (s.IsActivated && s.loop)
				s.Play();
		}
	}

	public static void Play(string label)
	{
		Instance._Play(label);
	}

	public static void Stop(string label)
	{
		Instance._Stop(label);
	}

	public static bool IsPlaying(string label)
	{
		foreach (var s in Instance.clips)
		{
			if (s.label != label)
				continue;

			return s.IsPlaying;
		}

		return false;
	}

	public void _Play(string label)
	{
		foreach (var s in clips)
		{
			if (s.label != label)
				continue;

			s.Play();
		}
	}

	public void _Stop(string label)
	{
		foreach (var s in clips)
		{
			if (s.label != label)
				continue;

			s.Stop();
		}
	}

	public void _FadeIn(string label)
	{
		foreach (var s in clips)
		{
			if (s.label != label)
				continue;

			StartCoroutine(FadeIn(s));
		}
	}

	public void _FadeOut(string label)
	{
		foreach (var s in clips)
		{
			if (s.label != label)
				continue;

			StartCoroutine(FadeOut(s));
		}
	}

	IEnumerator FadeIn(SoundClips clip)
	{
		float time = FadeInTime;
		float origVolume = clip.standardVolume;

		while (time > 0)
		{
			time -= Time.deltaTime;

			clip.volume = Mathf.Lerp(origVolume, 0, time / FadeInTime);

			yield return null;
		}
		clip.volume = origVolume;
		clip.Play();
	}

	IEnumerator FadeOut(SoundClips clip)
	{
		float time = FadeInTime;
		float origVolume = clip.standardVolume;

		while (time > 0)
		{
			time -= Time.deltaTime;

			clip.volume = Mathf.Lerp(0, origVolume, time / FadeInTime);

			yield return null;
		}
		clip.volume = 0;
		clip.Stop();
	}
}
