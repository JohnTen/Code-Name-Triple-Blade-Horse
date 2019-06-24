using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TripleBladeHorse.Animation
{
	[System.Serializable]
	public struct Animation
	{
		public string name;  //动画名称，Animation name
		public float timeScale; //动画播放速度 The speed of playing animation
		public int playTimes; //动画播放的循环次数
		public float duration; //动画播放的持续时间 The duration of current animation
		public bool playing; //动画是否已经开始 Is animation started
		public bool completed;
		public bool fadingIn;
		public bool fadingOut;
		public bool fadeInComplete;
		public bool fadeOutComplete;

		//public bool isFadeOut; //动画是否淡出 Is animation fading out
		//public bool isFadeIn;
		//public float fadeInTime;

		public Animation(string name, float timeScale, int playTimes)
		{
			this.name = name;
			this.timeScale = timeScale;
			this.playTimes = playTimes;
			this.duration = 0;
			this.playing = false;
			this.completed = false;
			this.fadingIn = false;
			this.fadingOut = false;
			this.fadeInComplete = false;
			this.fadeOutComplete = false;
	}
	}
}

