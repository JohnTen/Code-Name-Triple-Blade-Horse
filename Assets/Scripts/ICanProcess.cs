using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanProcess<T, F> where F : struct, System.IConvertible
{
	T Process(T target);

	float GetFactor(F type);

	void SetFactor(F type, float value);
}
