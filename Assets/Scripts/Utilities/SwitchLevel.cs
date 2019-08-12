using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace JTUtility
{
    public class SwitchLevel : MonoBehaviour
    {
        [System.Serializable] class AsyncOperationEvent : UnityEvent<AsyncOperation> { }
        [SerializeField] AsyncOperationEvent onLoadingScene;

        public void LoadScene(string levelName)
        {
            SceneManager.LoadScene(levelName);
        }

        public void LoadSceneAsync(string levelName)
        {
            onLoadingScene.Invoke(SceneManager.LoadSceneAsync(levelName));
        }

        public void LoadSceneAdditive(string levelName)
        {
            SceneManager.LoadScene(levelName, LoadSceneMode.Additive);
        }
        public void LoadSceneAsyncAdditive(string levelName)
        {
            onLoadingScene.Invoke(SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive));
        }

        public void UnloadSceneAsync(string levelName)
        {
            SceneManager.UnloadSceneAsync(levelName);
        }

        public void ReloadCurrentScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public float Timescale
        {
            get { return Time.timeScale; }
            set
            {
                Time.timeScale = value;
                Time.fixedDeltaTime = value * 0.02f;
            }
        }
    }
}
