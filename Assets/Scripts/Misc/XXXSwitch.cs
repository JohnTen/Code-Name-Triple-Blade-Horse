using JTUtility;
using System.Collections;
using UnityEngine;

public class XXXSwitch : MonoBehaviour
{
    #region Fields
    [SerializeField] float interactiveRange;
    [SerializeField] Transform playerActor;

    [Space(10)]
    [SerializeField] float leverRotateDuration;
    [SerializeField] Vector3 leverRotateAngleFacter;
    [SerializeField] AnimationCurve leverRotateCurve;
    [SerializeField] Transform lever;

    [Space(10)]
    [SerializeField] BoolEvent onEnterInteractDistance;
    [SerializeField] BoolEvent onSwitch;

    bool leverRotating;
    bool inInteractiveRange;
    #endregion

    #region Properties
    public bool Activated { get; private set; }
    #endregion

    #region Unity Messages
    private void OnEnable()
    {
        if (!playerActor)
        {
            Debug.LogError("Lever requires a player object to work");
            enabled = false;
        }

        if (!lever)
        {
            Debug.LogError("Lever requires a lever object to work");
            enabled = false;
        }
    }

    private void Update()
    {
        if (inInteractiveRange && Input.GetKey(KeyCode.E))
            ToggleActivateStatus();

        if (!inInteractiveRange
         && Vector3.Distance(playerActor.position, transform.position) < interactiveRange)
        {
            inInteractiveRange = true;
            onEnterInteractDistance.Invoke(true);
        }

        if (inInteractiveRange
         && Vector3.Distance(playerActor.position, transform.position) > interactiveRange)
        {
            inInteractiveRange = false;
            onEnterInteractDistance.Invoke(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform != playerActor) return;

        inInteractiveRange = true;
        onEnterInteractDistance.Invoke(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform != playerActor) return;

        inInteractiveRange = false;
        onEnterInteractDistance.Invoke(false);
    }
    #endregion

    #region Public Methods
    public void ToggleActivateStatus()
    {
        if (leverRotating) return;

        Activated = !Activated;
        onSwitch.Invoke(Activated);

        StartCoroutine(RotateLever(Activated));
    }
    #endregion

    #region Private Methods
    IEnumerator RotateLever(bool turnOn)
    {
        leverRotating = true;

        float time = 0;
        float factor = 0;
        Vector3 angle;

        while (time < leverRotateDuration)
        {
            time += Time.deltaTime;

            factor = time / leverRotateDuration;
            factor = turnOn ? factor : 1 - factor;
            angle = leverRotateAngleFacter * leverRotateCurve.Evaluate(factor);

            lever.localEulerAngles = angle;

            yield return null;
        }

        leverRotating = false;
    }
    #endregion
}
