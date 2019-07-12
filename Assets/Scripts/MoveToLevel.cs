﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveToLevel : MonoBehaviour
{

    [SerializeField]
    string nextSceneName;
    [SerializeField]
    string triggerName;
    bool Entry = false;

    void Update()
    {
        // Press the space key to start coroutine
        if (Entry)
        {
            StartCoroutine(LoadYourAsyncScene());
            Entry = false;
        }
        //if(Input.GetKeyDown(KeyCode.L))
        //{
        //    StartCoroutine(LoadYourAsyncScene());
        //}
    }

    IEnumerator LoadYourAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.name == triggerName)
        {
            Entry = true;
        }
    }
}
