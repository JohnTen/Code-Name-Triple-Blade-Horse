using Cinemachine;
using JTUtility;
using UnityEngine;

namespace TripleBladeHorse.VFX
{
    [RequireComponent(typeof(CinemachineBrain))]
    public class ShakeScreen : MonoSingleton<ShakeScreen>
    {
        [Header("Debug")]
        [SerializeField] ScreenShakeParams _param;
        [SerializeField] float _timer;
        CinemachineBrain _brain;

        CinemachineVirtualCameraBase _vcam;
        ShakeScreenExtension _shaker;

        ShakeScreenExtension Shaker
        {
            get
            {
                var vcam = _brain.ActiveVirtualCamera as CinemachineVirtualCameraBase;
                if (_vcam != vcam && vcam != null)
                {
                    _vcam = vcam;
                }

                var shaker = _vcam.GetComponent<ShakeScreenExtension>();
                if (shaker == null)
                    shaker = _vcam.GetOrAddComponent<ShakeScreenExtension>();

                if (shaker != _shaker)
                    _shaker = shaker;

                return _shaker;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _brain = GetComponent<CinemachineBrain>();
        }

        private void Update()
        {
            if (_timer > 0)
                Shaker.SetOffset(GetOffset());
        }

        public void Shake(ScreenShakeParams param)
        {
            if (_timer > _param._duration) return;
            _param = param;
            _timer = _param._duration;
        }

        Vector3 GetOffset()
        {
            if (_timer <= 0) return Vector3.zero;

            _timer -= TimeManager.DeltaTime;
            var decay = _param._decay.Evaluate((_param._duration - _timer) / _param._duration);
            var percentage = (_param._duration - _timer) * Mathf.PI * 2;
            return new Vector3(
                Mathf.Sin(percentage * _param._XFrequency) * _param._XAmplitude * decay,
                Mathf.Sin(percentage * _param._YFrequency) * _param._YAmplitude * decay,
                Mathf.Sin(percentage * _param._YFrequency) * _param._ZAmplitude * decay
                );
        }
    }
}
