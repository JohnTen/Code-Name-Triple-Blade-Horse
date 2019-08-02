using System.Collections;
using TripleBladeHorse.Movement;
using UnityEngine;
using UnityEngine.UI;

namespace TripleBladeHorse
{
    public class BossDeathHandler : MonoBehaviour, ICanHandleDeath
    {
        [SerializeField] Image _killed;
        [SerializeField] Shadow _killedShadow;
        [SerializeField] float phrase1 = 1f;
        [SerializeField] float delay = 2f;
        [SerializeField] float phrase2 = 1f;
        [SerializeField] float targetScale = 0.8f;
        [SerializeField] Transform[] _weakpoints;
        [SerializeField] bool playing;

        public void OnDeath(CharacterState state)
        {
            if (playing) return;
            playing = true;
            foreach (var points in _weakpoints)
            {
                points.GetComponent<Climbable>().enabled = false;
                points.gameObject.SetActive(false);
            }

            this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            this.GetComponent<BossMover>().enabled = false;
            foreach (var collider in this.GetComponentsInChildren<Collider2D>())
            {
                collider.enabled = false;
            }

            StartCoroutine(DisplayKill());
        }

        IEnumerator DisplayKill()
        {
            var timer = phrase1;
            var startScale = _killed.rectTransform.sizeDelta;
            var targetScale = startScale * this.targetScale;

            while (timer > 0)
            {
                timer -= TimeManager.DeltaTime;
                var percent = timer / phrase1;
                var color = _killed.color;
                var shadowColor = _killedShadow.effectColor;
                _killed.rectTransform.sizeDelta = Vector2.Lerp(startScale, targetScale, percent);
                color.a = 1 - percent;
                shadowColor.a = 1 - percent;
                _killed.color = color;
                _killedShadow.effectColor = shadowColor;
                yield return null;
            }

            yield return new WaitForSeconds(delay);

            timer = phrase2;
            while (timer > 0)
            {
                timer -= TimeManager.DeltaTime;
                var percent = timer / phrase1;
                var color = _killed.color;
                var shadowColor = _killedShadow.effectColor;
                color.a = percent;
                shadowColor.a = percent;
                _killed.color = color;
                _killedShadow.effectColor = shadowColor;
                yield return null;
            }

            this.gameObject.SetActive(false);
        }
    }
}
