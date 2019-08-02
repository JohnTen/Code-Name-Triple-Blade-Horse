using TripleBladeHorse;
using UnityEngine;

public class Rotate_VFX : MonoBehaviour
{
    public float speed_x = 0;
    public float speed_y = 0;
    public float speed_z = 0;


    void Update()
    {
        transform.Rotate(Vector3.up * TimeManager.DeltaTime * speed_x);
        transform.Rotate(Vector3.right * TimeManager.DeltaTime * speed_y);
        transform.Rotate(Vector3.forward * TimeManager.DeltaTime * speed_z);
    }
}
