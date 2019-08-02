using System.Collections;
using UnityEngine;
using JTUtility;

public class XXXDoor : MonoBehaviour
{
    #region Fields
    [SerializeField] float doorOpenDuration;
    [SerializeField] Vector3 doorMovingFacter;
    [SerializeField] AnimationCurve doorMovingCurve;
    [SerializeField] Transform door;

    [Space(10)]
    [SerializeField] BoolEvent onDoorOpen;

    bool doorOpening;
    #endregion

    #region Properties
    public bool Opened { get; private set; }
    #endregion

    #region Public Methods
    public void SetDoorOpenState(bool open)
    {
        if (open == Opened || doorOpening) return;

        Opened = open;
        StartCoroutine(OpenDoor(open));
    }
    #endregion

    #region Private Methods
    IEnumerator OpenDoor(bool open)
    {
        doorOpening = true;

        float time = 0;
        float factor = 0;
        Vector3 movement;

        while (time < doorOpenDuration)
        {
            time += Time.deltaTime;

            factor = time / doorOpenDuration;
            factor = open ? factor : 1 - factor;
            movement = doorMovingFacter * doorMovingCurve.Evaluate(factor);

            door.localPosition = movement;

            yield return null;
        }

        doorOpening = false;
    }
    #endregion
}
