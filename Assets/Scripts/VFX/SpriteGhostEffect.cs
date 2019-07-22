using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TripleBladeHorse.VFX
{
	public class SpriteGhostEffect : MonoBehaviour
	{
		[SerializeField] private Transform _root;
		[SerializeField] private float distanceBtwSpawns;
		[SerializeField] private float _existTime;
		[SerializeField] private Color color;
		[SerializeField] SpriteGhostInstance _ghostPrefab;

		[Header("Debug")]
		[SerializeField] private bool spawning;
		[SerializeField] private Vector3 startPositionBtwSpawns;

		public Color Color
		{
			get => color;
			set => color = value;
		}

		public void StartDraw()
		{
			enabled = true;
			startPositionBtwSpawns = this.transform.position;
		}

		public void StopDraw()
		{
			enabled = false;
		}

		private void Spawn(Vector3 position)
		{
			var instance = Instantiate(_ghostPrefab.gameObject).GetComponent<SpriteGhostInstance>();
			instance.transform.position = position;
			instance.transform.rotation = _root.rotation;
			instance.Initialize(_root, color);
			Destroy(instance.gameObject, _existTime);
		}

		private void Update()
		{
			while (Vector3.Distance(this.transform.position, startPositionBtwSpawns) > distanceBtwSpawns)
			{
				Vector3 spawnDirection = (this.transform.position - startPositionBtwSpawns).normalized;
				startPositionBtwSpawns += distanceBtwSpawns * spawnDirection;
				Spawn(startPositionBtwSpawns);
			}
		}
	}
}

