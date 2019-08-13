using UnityEngine;
using UnityEngine.Audio;

namespace TripleBladeHorse
{
    public class AudioManager : MonoBehaviour
    {
        public AudioMixer audioMixer;    // 进行控制的Mixer变量

        public void SetMasterVolume(float volume)    // 控制主音量的函数
        {
            audioMixer.SetFloat("MasterVolume", volume);
            // MasterVolume为我们暴露出来的Master的参数
        }

        public void SetMusicVolume(float volume)    // 控制背景音乐音量的函数
        {
            audioMixer.SetFloat("MusicVolume", volume);
            // MusicVolume为我们暴露出来的Music的参数
        }

        public void SetSoundEffectVolume(float volume)    // 控制音效音量的函数
        {
            audioMixer.SetFloat("SoundEffectVolume", volume);
            // SoundEffectVolume为我们暴露出来的SoundEffect的参数
        }
    }
}

