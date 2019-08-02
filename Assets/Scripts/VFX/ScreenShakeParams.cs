using UnityEngine;

namespace TripleBladeHorse.VFX
{
    [System.Serializable]
    public struct ScreenShakeParams
    {
        public float _duration;
        public float _XFrequency;
        public float _YFrequency;
        public float _ZFrequency;
        public float _XAmplitude;
        public float _YAmplitude;
        public float _ZAmplitude;

        public AnimationCurve _decay;
    }
}
