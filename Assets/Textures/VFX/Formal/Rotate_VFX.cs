using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        transform.Rotate(Vector3.up * Time.deltaTime * speed_x);
        transform.Rotate(Vector3.right * Time.deltaTime * speed_y);
        transform.Rotate(Vector3.forward * Time.deltaTime * speed_z);
    }
}
