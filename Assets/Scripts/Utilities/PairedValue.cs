using System;
using UnityEngine;

namespace JTUtility
{
    [Serializable]
    public abstract class PairedValue { }

    [Serializable]
    public class PairedValue<K, V> : PairedValue
    {
        [SerializeField] K key;
        [SerializeField] V value;

        public K Key
        {
            get { return key; }
            set { key = value; }
        }

        public V Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public PairedValue()
        {
            key = default;
            value = default;
        }

        public PairedValue(K key, V value)
        {
            this.key = key;
            this.value = value;
        }
    }

    [Serializable] public class StrIntPair : PairedValue<string, int> { }
    [Serializable] public class StrBoolPair : PairedValue<string, bool> { }
    [Serializable] public class StrFloatPair : PairedValue<string, float> { }
}