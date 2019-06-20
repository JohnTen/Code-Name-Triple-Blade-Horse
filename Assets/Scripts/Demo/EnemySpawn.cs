using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
	[SerializeField] TempEnemyAI _enemyPrefab;
	[SerializeField] float _range;
	[SerializeField] float _spawnDelay = 2;
	TempEnemyAI enemyReference;
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
		enemyReference = Instantiate(_enemyPrefab.gameObject).GetComponent<TempEnemyAI>();
		enemyReference.transform.position = this.transform.position + Random.Range(-_range, _range) * Vector3.right;
		
		if (Random.value > 0.5f)
		{
			enemyReference.FirstTarget = transform.position - _range * Vector3.right;
			enemyReference.LastTarget = transform.position + _range * Vector3.right;
		}
		else
		{

			enemyReference.FirstTarget = transform.position + _range * Vector3.right;
			enemyReference.LastTarget = transform.position - _range * Vector3.right;
		}
	}
}
