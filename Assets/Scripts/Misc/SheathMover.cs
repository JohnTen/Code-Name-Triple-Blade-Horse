using JTUtility;
using System.Collections.Generic;
using UnityEngine;

namespace TripleBladeHorse
{

    public class SheathMover : MonoBehaviour
    {
        [System.Serializable]
        class TimerPair : PairedValue<Timer, Transform>
        {
            public TimerPair() : base() { }
            public TimerPair(Timer timer, Transform transform) : base(timer, transform) { }
        }

        [SerializeField] List<Transform> _sheaths;
        [SerializeField] Vector3 _rotation;
        [SerializeField] float _followUpSpeed = 3f;
        [SerializeField] float _floatDist = 0.2f;
        [SerializeField] float _floatFreq = 0.5f;
        [SerializeField] float _radius = 1f;
        [SerializeField] float _radiusDynamic = 0.2f;
        [SerializeField] float _radiusDynamicSpeed = 0.3f;
        [SerializeField] float _knifeRotateSpeed = 8f;
        [SerializeField] float _knifeRandomRotateSpeed = 2f;
        [SerializeField] AnimationCurve _knifeRotateAcceleration;
        [Header("Debug")]
        [SerializeField] List<Transform> _activeSheaths;

        public float FloatScale { get; set; } = 1;
        public float RadiusScale { get; set; } = 1;
        public float RotationScale { get; set; } = 1;

        [SerializeField] List<TimerPair> _knifeRotatePairs;

        private void Awake()
        {
            if (_activeSheaths == null)
                _activeSheaths = new List<Transform>();

            _knifeRotatePairs = new List<TimerPair>();

            for (int i = 0; i < _sheaths.Count; i++)
            {
                var timer = new Timer();
                timer.CustomDeltaTimeSource = DeltaTimeRelater;
                _knifeRotatePairs.Add(new TimerPair(timer, null));
            }
        }

        private void Update()
        {
            transform.Rotate(_rotation * RotationScale * TimeManager.PlayerDeltaTime, Space.Self);
            UpdateSheathsPosition();
            RotateKnives();
        }

        private void OnDestroy()
        {
            for (int i = 0; i < _knifeRotatePairs.Count; i++)
            {
                _knifeRotatePairs[i].Key.Dispose();
            }
        }

        private float DeltaTimeRelater()
        {
            return TimeManager.PlayerDeltaTime;
        }

        private void RotateKnives()
        {
            for (int i = 0; i < _knifeRotatePairs.Count; i++)
            {
                if (_knifeRotatePairs[i].Value == null) continue;

                var percentage = 1 - _knifeRotatePairs[i].Key.PassedPercentage;
                var rotateSpeed
                    = (_knifeRotateSpeed
                    + Random.Range(-_knifeRandomRotateSpeed, _knifeRandomRotateSpeed))
                    * percentage
                    * _knifeRotateAcceleration.Evaluate(percentage)
                    * TimeManager.DeltaTime;

                _knifeRotatePairs[i].Value.Rotate(0, 0, rotateSpeed, Space.Self);
            }
        }

        private void UpdateSheathsPosition()
        {
            if (_activeSheaths.Count <= 0) return;

            var targetAngle = 360f / _activeSheaths.Count;
            var floatOffset = Mathf.PI / _sheaths.Count;
            var dynamicRadiusOffset =
                Mathf.Sin(_radiusDynamicSpeed * TimeManager.Time) * _radiusDynamic;
            var radius = (_radius + dynamicRadiusOffset) * RadiusScale;
            var basePoint = radius * Vector3.forward;

            for (int i = 0; i < _activeSheaths.Count; i++)
            {
                var targetPosition = basePoint.Rotate(0, targetAngle * i, 0);
                var bobbleTime = _floatFreq * TimeManager.Time + floatOffset * i;
                var bobble = Mathf.Sin(bobbleTime) * _floatDist * FloatScale * Vector3.up;

                bobble = transform.InverseTransformDirection(bobble);
                targetPosition += bobble;
                targetPosition = transform.TransformPoint(targetPosition);

                Debug.DrawLine(transform.position, targetPosition);

                _activeSheaths[i].position =
                    Vector3.MoveTowards(
                        _activeSheaths[i].position,
                        targetPosition,
                        _followUpSpeed * TimeManager.PlayerDeltaTime);
            }
        }

        public bool IsReady(Transform sheath)
        {
            if (!_sheaths.Contains(sheath))
            {
                return false;
            }

            return _knifeRotatePairs.Find((x) => { return x.Value == sheath; }).Key.IsReachedTime();
        }

        public void ActiveSheath(Transform sheath, float timeToReachFullRotate)
        {
            if (!_sheaths.Contains(sheath))
            {
                Debug.LogWarning("Not included in sheaths.");
                return;
            }

            var pair = _knifeRotatePairs.Find((x) => { return x.Value == null; });
            pair.Key.Start(timeToReachFullRotate);
            pair.Value = sheath;

            _activeSheaths.Add(sheath);
        }

        public void DeactiveSheath(Transform sheath)
        {
            if (!_sheaths.Contains(sheath))
            {
                Debug.LogWarning("Not included in sheaths.");
                return;
            }

            _knifeRotatePairs.Find(
                (x) => { return x.Value == sheath; }
                ).Value = null;

            _activeSheaths.Remove(sheath);

            if (_activeSheaths.Count <= 0) return;

            var toFirstSheath = _activeSheaths[0].position - this.transform.position;
            toFirstSheath.y = 0;
            transform.forward = toFirstSheath;
        }
    }
}

