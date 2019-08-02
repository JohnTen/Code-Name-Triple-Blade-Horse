using System.Collections.Generic;
using UnityEngine;

namespace TripleBladeHorse.VFX
{
    public class SpriteGhostInstance : MonoBehaviour
    {
        [SerializeField] float _fadingSpeed;
        [SerializeField] private Material _customMaterial;

        private List<SpriteRenderer> _renderers = new List<SpriteRenderer>();

        public void Initialize(Transform root, Color color)
        {
            var renderList = new List<SpriteRenderer>();
            _renderers.Clear();
            root.GetComponentsInChildren(renderList);

            foreach (var renderer in renderList)
            {
                var go = new GameObject(renderer.name);
                var ghost = go.AddComponent<SpriteRenderer>();

                ghost.sprite = renderer.sprite;
                ghost.sortingLayerID = renderer.sortingLayerID;
                ghost.sortingOrder = renderer.sortingOrder;
                ghost.transform.SetParent(this.transform);
                ghost.transform.localPosition = renderer.transform.localPosition;
                ghost.transform.localRotation = renderer.transform.localRotation;
                ghost.transform.localScale = renderer.transform.lossyScale;
                ghost.color = color;

                _renderers.Add(ghost);
            }
        }

        private void Update()
        {
            foreach (var renderer in _renderers)
            {
                Color color = renderer.color;
                color.a -= _fadingSpeed * TimeManager.PlayerDeltaTime;
                color.a = Mathf.Clamp01(color.a);
                renderer.color = color;
            }
        }
    }
}

