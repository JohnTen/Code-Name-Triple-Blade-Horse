using System;
using UnityEngine;

namespace JTUtility
{
    [Serializable]
    public struct ModifiableValue
    {
        [SerializeField] private float @base;
        [SerializeField] private float addition;
        [SerializeField] private float multiplier;
        [SerializeField] private float current;

        public float Base
        {
            get { return @base; }
            set
            {
                @base = value;
                RecalculateCurrentValue();
            }
        }

        public float Current
        {
            get { return current; }
        }

        public ModifiableValue(float baseValue)
        {
            @base = baseValue;
            addition = 0;
            multiplier = 1;
            current = baseValue;
        }

        public void ResetValue()
        {
            addition = 0;
            multiplier = 1;
            current = @base;
        }

        private void RecalculateCurrentValue()
        {
            current = @base * multiplier + addition;
        }

        public static implicit operator float(ModifiableValue value)
        {
            return value.current;
        }

        public static ModifiableValue operator +(ModifiableValue v1, float v2)
        {
            v1.addition += v2;
            v1.RecalculateCurrentValue();
            return v1;
        }

        public static ModifiableValue operator -(ModifiableValue v1, float v2)
        {
            v1.addition -= v2;
            v1.RecalculateCurrentValue();
            return v1;
        }

        public static ModifiableValue operator *(ModifiableValue v1, float v2)
        {
            v1.multiplier *= v2;
            v1.RecalculateCurrentValue();
            return v1;
        }

        public static ModifiableValue operator /(ModifiableValue v1, float v2)
        {
            v1.multiplier /= v2;
            v1.RecalculateCurrentValue();
            return v1;
        }

        public override string ToString()
        {
            return "Base: " + @base + ", current: " + current + ", add: " + addition + ", mul: " + multiplier;
        }
    }
}
