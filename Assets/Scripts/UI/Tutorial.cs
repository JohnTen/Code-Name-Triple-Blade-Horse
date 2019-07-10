using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class Tutorial  : MonoBehaviour
{

    [SerializeField] Image Toturailimage;
    [SerializeField] float Showspeed;
    bool stay = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (stay == true)
        {
            Color color = Toturailimage.color;
            color.a += Time.deltaTime * Showspeed;
            Toturailimage.color = new Color(255, 255, 255, color.a);
        }

        if (stay == false)
        {
            Color color = Toturailimage.color;
            color.a -= Time.deltaTime * Showspeed;
            Toturailimage.color = new Color(255, 255, 255, color.a);
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
}
