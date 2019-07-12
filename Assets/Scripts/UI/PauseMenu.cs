using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TripleBladeHorse;


public class PauseMenu : MonoBehaviour

{
    [SerializeField] GameObject pausemenu;
    // Start is called before the first frame update
    void Start()
    {
        pausemenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pausemenu.SetActive(true);
            TimeManager.Pause();
        }
    }

    public void ContinueGame()
    {
        pausemenu.SetActive(false);
        TimeManager.Unpause();

    }
    public void ExitGame()
    {
        Application.Quit();
    }
}


