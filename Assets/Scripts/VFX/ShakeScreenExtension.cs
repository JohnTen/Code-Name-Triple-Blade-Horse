using Cinemachine;
using UnityEngine;

namespace TripleBladeHorse.VFX
{
    [ExecuteInEditMode]
    [SaveDuringPlay]
    [AddComponentMenu("")] // Hide in menu
    public class ShakeScreenExtension : CinemachineExtension
    {
        [Header("Debug")]
        [SerializeField] Vector3 _offset;

        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage == CinemachineCore.Stage.Body)
            {
                Vector3 shakeAmount = _offset;
                state.PositionCorrection += shakeAmount;
            }
        }

        public void SetOffset(Vector3 offset)
        {
            _offset = offset;
        }
    }
}
