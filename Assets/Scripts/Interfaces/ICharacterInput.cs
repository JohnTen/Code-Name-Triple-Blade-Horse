using JTUtility;
using UnityEngine;

namespace TripleBladeHorse
{
    public interface ICharacterInput<T> where T : struct, System.IConvertible
    {
        bool DelayInput { get; set; }
        bool BlockInput { get; set; }

        event Action<InputEventArg<T>> OnReceivedInput;

        Vector2 GetMovingDirection();
        Vector2 GetAimingDirection();
    }
}

