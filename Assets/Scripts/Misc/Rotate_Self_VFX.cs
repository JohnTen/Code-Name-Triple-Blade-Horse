using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate_Self_VFX : MonoBehaviour
{
    public float speed_x;
    public float speed_y;
    public float speed_z;
	
    void Update()
    {
        transform.Rotate(speed_x, speed_y, speed_z, Space.Self);
    }
}
