using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class FadeToBlackNWhite : MonoBehaviour
{
	[SerializeField] Transform object1;
	[SerializeField] Transform object2;
	[SerializeField] PostProcessProfile profile;
	[SerializeField] AnimationCurve blackOutCurve;
	[SerializeField] float maxDistance;

	float originalSaturation;
	ColorGrading colorGrading;


	private void OnEnable()
	{
		profile.TryGetSettings(out colorGrading);
		originalSaturation = colorGrading.saturation;
	}

	private void OnDisable()
	{
		colorGrading.saturation.value = originalSaturation;
	}

	private void Update()
	{
		var distance = Vector3.Distance(object1.position, object2.position);
		colorGrading.saturation.value = blackOutCurve.Evaluate(Mathf.Lerp(1, 0, distance / maxDistance)) * -100;
	}
}
