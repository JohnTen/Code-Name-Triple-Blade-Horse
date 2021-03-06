﻿using UnityEngine;
using UnityEngine.UI;

namespace TripleBladeHorse.UI
{
    public class HpSliderUpdater : MonoBehaviour
    {
        [SerializeField] CharacterState _state;
        [SerializeField] Slider _hpSplider;

        private void Start()
        {
            _hpSplider.maxValue = _state._hitPoints.Base;
        }

        void Update()
        {
            _hpSplider.value = _state._hitPoints.Current;
        }
    }
}