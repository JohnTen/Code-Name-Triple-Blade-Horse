using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TripleBladeHorse.Animation;

public class TestStateMachine : MonoBehaviour
{
	[SerializeField] StateMachine machine;

	private void Update()
	{
		machine.SetFloat("XSpeed", Input.GetAxis("Horizontal"));
		machine.SetBool("Jump", Input.GetKey(KeyCode.G));
	}
}
