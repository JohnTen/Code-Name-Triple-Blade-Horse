using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace JTUtility
{
public class PauseMenu : MonoBehaviour

{
        GameObject pausemenu;
    // Start is called before the first frame update
    void Start()
    {
            pausemenu = GameObject.Find ("PauseMenu") ;
            pausemenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                pausemenu.SetActive(true);
                Time.timeScale = 0;
            }
    }

   public void ContinueGame()
        {
            pausemenu.SetActive(false);
            Time.timeScale = 1;
        }
        public void ExitGame()
        {
            Application.Quit();
        }
    }

}
