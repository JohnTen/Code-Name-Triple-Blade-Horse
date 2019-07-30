using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TripleBladeHorse.UI
{
	[RequireComponent(typeof(CanvasGroup))]
	public class UIFading : MonoBehaviour
	{
		[SerializeField] float fadingTime;
		[SerializeField] bool appear;
		[Header("Debug")]
		[SerializeField] bool fading;

		CanvasGroup group;

		private void Awake()
		{
			group = GetComponent<CanvasGroup>();
		}

		public bool Appear
		{
			get => appear;
			set
			{
				if (appear == value) return;

				appear = value;
				StopAllCoroutines();
				var from = appear ? 0 : 1;
				var to = appear ? 1 : 0;
				StartCoroutine(Fade(from, to));
			}
		}

		IEnumerator Fade(float from, float to)
		{
			var timer = 0f;
			fading = true;
			while (timer < 1)
			{
				timer += TimeManager.DeltaTime / fadingTime;

				group.alpha = Mathf.Lerp(from, to, timer);
				yield return null;
			}
			fading = false;
		}
	}
}
