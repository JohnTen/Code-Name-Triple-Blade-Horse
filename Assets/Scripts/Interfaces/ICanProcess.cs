using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanProcess<T>
{
	T Process(T target);
}
