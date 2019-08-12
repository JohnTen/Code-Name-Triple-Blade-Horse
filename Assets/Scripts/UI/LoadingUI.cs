using System.Collections;
using UnityEngine;

namespace TripleBladeHorse.UI
{
    public class LoadingUI : MonoBehaviour
    {
        [SerializeField] Transform icon;
        [SerializeField] float rotateSpeed;

        public void StartLoading(AsyncOperation operation)
        {
            StartCoroutine(PlayLoadingAnimation(operation));
        }

        IEnumerator PlayLoadingAnimation(AsyncOperation asyncOperation)
        {
            while (!asyncOperation.isDone)
            {
                icon.Rotate(0, 0, rotateSpeed * TimeManager.DeltaTime);

                yield return null;
            }
        }
    }
}
