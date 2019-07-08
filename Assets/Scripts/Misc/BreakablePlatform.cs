using UnityEngine;

namespace TripleBladeHorse
{
	public class BreakablePlatform : MonoBehaviour
	{
		[SerializeField] float breakTime = 2;
		[SerializeField] float reappearTime = 2;
        [SerializeField] ParticleSystem rubble;

		public void StartBreaking()
		{
			if (!IsInvoking())
				Invoke("Break", breakTime);
		}

		private void Break()
		{
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

