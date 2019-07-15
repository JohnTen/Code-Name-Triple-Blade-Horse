using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class Tutorial  : MonoBehaviour, IInputModelPlugable
{

    [SerializeField] Image Toturailimage;
    [SerializeField] Image Toturailimage2;
    [SerializeField] float Showspeed;
    bool stay = false;
    bool controller;

	public bool Stay
	{
		get => stay;
		set => stay = value;
	}

    // Start is called before the first frame update
    void Start()
    {
        InputManager.Instance.RegisterPluggable(0,this);
    }

    // Update is called once per frame
    void Update()
    {
        if(controller == false)
        {
            if (stay == true)
            {
                Color color = Toturailimage.color;
                color.a += Time.deltaTime * Showspeed;
                color.a = Mathf.Clamp01(color.a);
				Toturailimage2.color = new Color(color.r, color.g, color.b, 0);
				Toturailimage.color = new Color(color.r, color.g, color.b, color.a);

            }

            if (stay == false)
            {
                Color color = Toturailimage.color;
                color.a -= Time.deltaTime * Showspeed;
                color.a = Mathf.Clamp01(color.a);
				Toturailimage2.color = new Color(color.r, color.g, color.b, 0);
				Toturailimage.color = new Color(color.r, color.g, color.b, color.a);
            }
        }
        else
        {
          
            if (stay == true)
            {
                Color color1 = Toturailimage2.color;
                color1.a += Time.deltaTime * Showspeed;
                color1.a = Mathf.Clamp01(color1.a);
				Toturailimage.color = new Color(color1.r, color1.g, color1.b, 0);
				Toturailimage2.color = new Color(color1.r, color1.g, color1.b, color1.a);
            }

            if (stay == false)
            {
                Color color1 = Toturailimage2.color;
                color1.a -= Time.deltaTime * Showspeed;
                color1.a = Mathf.Clamp01(color1.a);
				Toturailimage.color = new Color(color1.r, color1.g, color1.b, 0);
				Toturailimage2.color = new Color(color1.r, color1.g, color1.b, color1.a);
            }
        }
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            print("yingyingying");
            stay = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            print("hahahaha");
            stay = false;
        }
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

    }

}
