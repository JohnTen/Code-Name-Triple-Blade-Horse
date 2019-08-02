using System.Collections.Generic;
using UnityEngine;

namespace JTUtility.Interactables
{
    public class PressPlate : MonoInteractable
    {
        [SerializeField]
        LayerMask affetcableLayer = new LayerMask();

        [SerializeField]
        List<GameObject> pressingObjects = new List<GameObject>();

        public override void StartInteracting() { }

        public override void StopInteracting() { }

        protected override void Update()
        {
            if (pressingObjects.Count > 0)
            {
                if (!isInteracting)
                {
                    InvokeStartInteracting();
                    InvokeActivated();

                    if (onActivated != null)
                        onActivated.Invoke();
                }

                isActivated = true;
                isInteracting = true;

                InvokeKeepInteracting();
            }
            else
            {
                if (isInteracting)
                {
                    InvokeStopInteracting();
                    InvokeDeactivated();

                    if (onDeactivated != null)
                        onDeactivated.Invoke();
                }

                isActivated = false;
                isInteracting = false;
            }
        }

        protected virtual void OnTriggerEnter(Collider collider)
        {
            if (((1 << collider.gameObject.layer) & affetcableLayer) != 0 &&
                !pressingObjects.Contains(collider.gameObject))
            {
                pressingObjects.Add(collider.gameObject);
            }
        }

        protected virtual void OnTriggerExit(Collider collider)
        {
            if (((1 << collider.gameObject.layer) & affetcableLayer) != 0 &&
                pressingObjects.Contains(collider.gameObject))
            {
                pressingObjects.Remove(collider.gameObject);
            }
        }
    }
}
