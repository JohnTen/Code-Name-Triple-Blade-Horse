using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TripleBladeHorse;

public class Rotate_VFX : MonoBehaviour
{
    public float speed_x = 0;
    public float speed_y = 0;
    public float speed_z = 0;




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * TimeManager.DeltaTime * speed_x);
        transform.Rotate(Vector3.right * TimeManager.DeltaTime * speed_y);
        transform.Rotate(Vector3.forward * TimeManager.DeltaTime * speed_z);
    }
}
