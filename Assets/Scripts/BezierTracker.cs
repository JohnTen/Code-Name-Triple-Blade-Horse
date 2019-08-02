using UnityEngine;

namespace TripleBladeHorse
{
    [System.Serializable]
    public class BezierTracker
    {
        [Header("Debug")]
        [SerializeField] Vector3 _ctrlPosition;

        public void Initialize(Vector3 currentPosition)
        {
            _ctrlPosition = currentPosition;
        }

        public Vector3 Berp(Vector3 currentPosition, Vector3 targetPosition, float t)
        {
            _ctrlPosition = Vector3.Lerp(_ctrlPosition, targetPosition, t);
            currentPosition = Vector3.Lerp(currentPosition, _ctrlPosition, t);
            return currentPosition;
        }

        public Vector2 Berp(Vector2 currentPosition, Vector2 targetPosition, float t)
        {
            _ctrlPosition = Vector2.Lerp(_ctrlPosition, targetPosition, t);
            currentPosition = Vector2.Lerp(currentPosition, _ctrlPosition, t);
            return currentPosition;
        }
    }
}