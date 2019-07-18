using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TripleBladeHorse;

public class PauseMenu : MonoBehaviour, IInputModelPlugable
{
	[SerializeField] GameObject pausemenu;
    bool controller;
    IInputModel input;
	// Start is called before the first frame update
	void Start()
	{
		pausemenu.SetActive(false);
        InputManager.Instance.RegisterPluggable(0, this);
    }

	// Update is called once per frame
	void Update()
	{
		if (input.GetButtonDown("Menu"))
		{
			pausemenu.SetActive(true);
			TimeManager.Instance.Pause();
		}
	}

	public void ContinueGame()
	{
		pausemenu.SetActive(false);
		TimeManager.Instance.Unpause();

	}
	public void ExitGame()
	{
		Application.Quit();
	}

    public void SetInputModel(IInputModel model)
    {
        if (model is ControllerInputModel)
        {
            controller = true;
        }
        else
        {
            controller = false;
        }

        input = model;
    }

}


