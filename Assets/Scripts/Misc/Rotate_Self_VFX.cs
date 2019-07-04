using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate_Self_VFX : MonoBehaviour
{
    public float speed_x;
    public float speed_y;
    public float speed_z;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(speed_x, speed_y, speed_z), Space.Self);
    }
}
