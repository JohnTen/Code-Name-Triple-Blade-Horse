using JTUtility;
using System.Collections.Generic;
using UnityEngine;

namespace TripleBladeHorse.Movement
{
    public class GroundDetector : MonoBehaviour, ICanDetectGround
    {
        [SerializeField] LayerMask _groundMask;
        [SerializeField] UpdateType _updateType;

        [SerializeField] List<Collider2D> touchedObjects;

        private bool _onGround;

        public bool IsOnGround => touchedObjects.Count > 0;
        public event Action OnLanding;
        public event Action OnTakingOff;
        public event Action OnStayGround;
        public event Action<ICanDetectGround, LandingEventArgs> OnLandingStateChanged;

        private void Awake()
        {
            touchedObjects = new List<Collider2D>();
        }

        private void Update()
        {
            if (_updateType != UpdateType.Update) return;
            ManualUpdate();
        }

        private void LateUpdate()
        {
            if (_updateType != UpdateType.LateUpdate) return;
            ManualUpdate();
        }

        private void FixedUpdate()
        {
            if (_updateType != UpdateType.FixedUpdate) return;
            ManualUpdate();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if ((1 << collision.gameObject.layer & _groundMask) == 0) return;
            if (!TestContactPoints(collision.contacts)) return;

            touchedObjects.Add(collision.collider);
            if (touchedObjects.Count == 1)
                OnLandingStateChanged?.Invoke(this, new LandingEventArgs(LandingState.OnGround, LandingState.Airborne));
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if ((1 << collision.gameObject.layer & _groundMask) == 0) return;

            touchedObjects.Remove(collision.collider);
            if (touchedObjects.Count == 0)
                OnLandingStateChanged?.Invoke(this, new LandingEventArgs(LandingState.Airborne, LandingState.OnGround));
        }

        private void ManualUpdate()
        {
            if (touchedObjects.Count > 0)
                OnStayGround?.Invoke();
        }

        private bool TestContactPoints(ContactPoint2D[] contacts)
        {
            foreach (var contact in contacts)
            {
                if (Vector2.Dot(contact.normal, Vector2.up) > 0.7f)
                    return true;
            }

            return false;
        }
    }
}