using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTAI;

public class EnemyAI : MonoBehaviour
{
    // Start is called before the first frame update
    Root aiRoot = BT.Root();




    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        aiRoot.Tick();
    }

}
