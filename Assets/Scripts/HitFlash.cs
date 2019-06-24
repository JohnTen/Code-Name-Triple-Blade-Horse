﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

public class HitFlash : MonoBehaviour
{
	[SerializeField] Color _flashColor;
	[SerializeField] float _flashDuration;
	[SerializeField, MinMaxSlider(0, 5)] Vector2 _flashRed;

	[Header("Debug")]
	[SerializeField] bool _flashing;
	[SerializeField] List<Renderer> _renderers;
	[SerializeField] List<Color> _defaultColor;

	void Awake()
    {
		_renderers = new List<Renderer>();
		_defaultColor = new List<Color>();
		GetComponentsInChildren(_renderers);
		_defaultColor.Capacity = _renderers.Count;
	}
	
    public void Flash()
	{
		if (!_flashing)
		{
			_defaultColor.Clear();
			foreach (var renderer in _renderers)
			{
				_defaultColor.Add(renderer.material.color);
			}
		}
		else
		{
			StopAllCoroutines();
		}

		StartCoroutine(BeenHit());
	}

	IEnumerator BeenHit()
	{
		float time = _flashDuration;
		bool colored = false;

		_flashing = true;

		while (time > 0)
		{
			time -= Time.deltaTime;
			if (_flashRed.IsIncluded(_flashDuration - time))
			{
				if (!colored)
				{
					colored = !colored;
					foreach (var renderer in _renderers)
					{
						renderer.material.color = _flashColor;
					}
				}
			}
			else if (colored)
			{
				colored = !colored;
				for (int i = 0; i < _renderers.Count; i++)
				{
					_renderers[i].material.color = _defaultColor[i];
				}
			}
			yield return null;
		}

		_flashing = false;
	}
}