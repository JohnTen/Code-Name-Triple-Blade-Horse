using UnityEngine;
using UnityEngine.Events;

namespace JTUtility
{
    public class PhysicsTrigger : MonoBehaviour
    {
        [SerializeField]
        bool triggerOnlyOnce = false;

        [SerializeField]
        string[] triggerableTags = new string[0];

        [SerializeField]
        UnityEvent onTrigged = new UnityEvent();

        bool triggeed = false;

        [SerializeField] ColliderEvent OnEnterTrigger = new ColliderEvent();
        [SerializeField] ColliderEvent OnStayTrigger = new ColliderEvent();
        [SerializeField] ColliderEvent OnExitTrigger = new ColliderEvent();

        [SerializeField] CollisionEvent OnStartCollision = new CollisionEvent();
        [SerializeField] CollisionEvent OnStayCollision = new CollisionEvent();
        [SerializeField] CollisionEvent OnExitCollision = new CollisionEvent();

        [SerializeField] Collider2DEvent OnEnterTrigger2D = new Collider2DEvent();
        [SerializeField] Collider2DEvent OnStayTrigger2D = new Collider2DEvent();
        [SerializeField] Collider2DEvent OnExitTrigger2D = new Collider2DEvent();

        [SerializeField] Collision2DEvent OnStartCollision2D = new Collision2DEvent();
        [SerializeField] Collision2DEvent OnStayCollision2D = new Collision2DEvent();
        [SerializeField] Collision2DEvent OnExitCollision2D = new Collision2DEvent();

        void OnTriggerEnter(Collider collider)
        {
            if (!IsTriggerable(collider)) return;

            triggeed = true;
            if (OnEnterTrigger != null)
            {
                OnEnterTrigger.Invoke(collider);
            }
        }

        void OnTriggerStay(Collider collider)
        {
            if (!IsTriggerable(collider)) return;

            triggeed = true;
            if (OnStayTrigger != null)
            {
                OnStayTrigger.Invoke(collider);
            }
        }

        void OnTriggerExit(Collider collider)
        {
            if (!IsTriggerable(collider)) return;

            triggeed = true;
            if (OnExitTrigger != null)
            {
                OnExitTrigger.Invoke(collider);
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (!IsTriggerable(collision.collider)) return;

            triggeed = true;
            if (OnStartCollision != null)
            {
                OnStartCollision.Invoke(collision);
            }
        }

        void OnCollisionStay(Collision collision)
        {
            if (!IsTriggerable(collision.collider)) return;

            triggeed = true;
            if (OnStayCollision != null)
            {
                OnStayCollision.Invoke(collision);
            }
        }

        void OnCollisionExit(Collision collision)
        {
            if (!IsTriggerable(collision.collider)) return;

            triggeed = true;
            if (OnExitCollision != null)
            {
                OnExitCollision.Invoke(collision);
            }
        }


        void OnTriggerEnter2D(Collider2D collider)
        {
            if (!IsTriggerable(collider)) return;

            triggeed = true;
            if (OnEnterTrigger2D != null)
            {
                OnEnterTrigger2D.Invoke(collider);
            }
        }

        void OnTriggerStay2D(Collider2D collider)
        {
            if (!IsTriggerable(collider)) return;

            triggeed = true;
            if (OnStayTrigger2D != null)
            {
                OnStayTrigger2D.Invoke(collider);
            }
        }

        void OnTriggerExit2D(Collider2D collider)
        {
            if (!IsTriggerable(collider)) return;

            triggeed = true;
            if (OnExitTrigger2D != null)
            {
                OnExitTrigger2D.Invoke(collider);
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (!IsTriggerable(collision.collider)) return;

            triggeed = true;
            if (OnStartCollision2D != null)
            {
                OnStartCollision2D.Invoke(collision);
            }
        }

        void OnCollisionStay2D(Collision2D collision)
        {
            if (!IsTriggerable(collision.collider)) return;

            triggeed = true;
            if (OnStayCollision2D != null)
            {
                OnStayCollision2D.Invoke(collision);
            }
        }

        void OnCollisionExit2D(Collision2D collision)
        {
            if (!IsTriggerable(collision.collider)) return;

            triggeed = true;
            if (OnExitCollision2D != null)
            {
                OnExitCollision2D.Invoke(collision);
            }
        }

        bool IsTriggerable(Collider collider)
        {
            if (triggeed && triggerOnlyOnce) return false;

            if (triggerableTags.Length == 0) return true;

            foreach (string s in triggerableTags)
            {
                if (s == collider.tag) return true;
            }

            return false;
        }

        bool IsTriggerable(Collider2D collider)
        {
            if (triggeed && triggerOnlyOnce) return false;

            if (triggerableTags.Length == 0) return true;

            foreach (string s in triggerableTags)
            {
                if (s == collider.tag) return true;
            }

            return false;
        }
    }
}

