using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
	[SerializeField] GameObject _enemyPrefab;
	[SerializeField] float _range;
	[SerializeField] float _spawnDelay = 2;
	GameObject enemyReference;
	float spawnTimer;
	bool spawned;
	
    void Update()
    {
		spawnTimer -= Time.deltaTime;

		if (enemyReference == null && spawnTimer < 0)
		{
			if (spawned)
			{
				spawnTimer = _spawnDelay;
				spawned = false;
			}
			else
			{
				Spawn();
				spawned = true;
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(transform.position, new Vector3(_range * 2, 1, 1));
	}

	void Spawn()
	{
		enemyReference = Instantiate(_enemyPrefab.gameObject, transform.position, transform.rotation);
		enemyReference.transform.position = this.transform.position + Random.Range(-_range, _range) * Vector3.right;
	}
}
