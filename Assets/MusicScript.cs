using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using JTUtility;

public class MusicScript : MonoSingleton<MusicScript>
{
    private void Start()
    {
        this.transform.SetParent(GlobalObject.Instance.transform);
        SceneManager.sceneLoaded += OnNewSceneLoaded;
    }

    private void OnNewSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Start")
            Destroy(this.gameObject);
    }
}
