﻿using UnityEngine;

namespace TripleBladeHorse
{
    public class EnemySpawn : MonoBehaviour, ICanHandleRespawn
    {
        [SerializeField] GameObject _enemyPrefab;
        [SerializeField] float _range;
        [SerializeField] float _spawnDelay = 2;
        GameObject enemyReference;
        float spawnTimer;
        bool spawned;

        void Update()
        {
            spawnTimer -= TimeManager.DeltaTime;

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

        public void Respawn()
        {
            if (enemyReference)
                Destroy(enemyReference);
            enemyReference = null;
            spawned = false;

            spawnTimer = 0;
        }
    }
}
