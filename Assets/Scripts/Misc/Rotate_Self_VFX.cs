using TripleBladeHorse;
using UnityEngine;

public class Rotate_Self_VFX : MonoBehaviour
{
    public float speed_x;
    public float speed_y;
    public float speed_z;

    void Update()
    {
        var speed = new Vector3(speed_x, speed_y, speed_z) * TimeManager.DeltaTime;
        transform.Rotate(speed, Space.Self);
        //transform.Rotate(speed_x, speed_y, speed_z, Space.Self);
    }
}
