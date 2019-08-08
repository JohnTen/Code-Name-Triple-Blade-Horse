using UnityEngine;

namespace TripleBladeHorse
{
    public class BreakablePlatform : MonoBehaviour, ICanHandleRespawn
    {
        [SerializeField] float breakTime = 2;
        [SerializeField] float reappearTime = 2;
        [SerializeField] ParticleSystem rubble;
        public AudioSource breakSound;
        public void Respawn()
        {
            if (IsInvoking())
            {
                CancelInvoke();
            }

            Reappear();
        }

        public void StartBreaking()
        {
            if (!IsInvoking())
                Invoke("Break", breakTime);
        }

        private void Break()
        {
            breakSound.Play();
            breakSound.pitch = Random.Range(0.75f, 1.25f);
            this.gameObject.SetActive(false);
            RubblePlay();
            Invoke("Reappear", reappearTime);
        }

        private void Reappear()
        {
            this.gameObject.SetActive(true);
            rubble.Stop();
        }

        private void RubblePlay()
        {
            rubble.Play();
        }
    }
}

