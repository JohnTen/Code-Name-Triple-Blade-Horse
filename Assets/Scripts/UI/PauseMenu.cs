using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TripleBladeHorse;

public class PauseMenu : MonoBehaviour, IInputModelPlugable
{
	[SerializeField] Canvas canvas;
	[SerializeField] GameObject panel;
	bool controller;
    IInputModel input;

	void Start()
	{
		InputManager.Instance.RegisterPluggable(0, this);
    }
	
	void Update()
	{
		if (input.GetButtonDown("Menu"))
		{
			canvas.enabled = !canvas.enabled;
			panel.gameObject.SetActive(canvas.enabled);
			if (canvas.enabled)
			{
				TimeManager.Instance.Pause();
			}
			else
			{
				TimeManager.Instance.Unpause();
			}
		}
	}

	public void ContinueGame()
	{
        canvas.enabled = false;
		panel.gameObject.SetActive(canvas.enabled);
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


