using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackMove<T, F> : ICanProcess<T, F> where F : struct, System.IConvertible
{
    
}
