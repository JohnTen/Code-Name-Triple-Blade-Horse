using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TripleBladeHorse.UI
{
    public class LoadingUI : MonoBehaviour
    {
        [SerializeField] Image icon;
        [SerializeField] float speed;
        [SerializeField] Sprite[] sprites;

        public void StartLoading(AsyncOperation operation)
        {
            StartCoroutine(PlayLoadingAnimation(operation));
        }

        IEnumerator PlayLoadingAnimation(AsyncOperation asyncOperation)
        {
            float time = 0;
            int index = 0;

            while (!asyncOperation.isDone)
            {
                if (time <= 0)
                {
                    icon.sprite = sprites[index];
                    index++;
                    index %= sprites.Length;
                    time += 1 / speed;
                }

                time -= Time.unscaledDeltaTime;
                yield return null;
            }
        }
    }
}
