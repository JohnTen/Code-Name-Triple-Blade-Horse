using TripleBladeHorse.Movement;
using UnityEngine;

namespace TripleBladeHorse.VFX
{
    public class InstanceDashEcho : MonoBehaviour
    {
        [SerializeField] private GameObject DashEchoPrefab;
        [SerializeField] private GameObject armature;
        [SerializeField] private float DistanceBtwSpawns;
        [SerializeField] private float destoryTime;
        [SerializeField] private Color color;

        private Vector3 startPositionBtwSpawns;
        private bool isDash = false;
        private ICanDash mover;

        public Color Color
        {
            get => color;
            set => color = value;
        }

        private void Start()
        {
            mover = this.GetComponent<ICanDash>();
            mover.OnBeginDashingInvincible += DashBeginHandler;
            mover.OnStopDashingInvincible += DashEndHandler;
        }

        private void DashBeginHandler()
        {
            isDash = true;
            startPositionBtwSpawns = this.transform.position;
        }

        private void DashEndHandler()
        {
            isDash = false;
        }

        private void Update()
        {
            if (isDash)
            {
                if (Vector3.Distance(this.transform.position, startPositionBtwSpawns) > DistanceBtwSpawns)
                {
                    Vector3 spawnDirection = (this.transform.position - startPositionBtwSpawns).normalized;
                    startPositionBtwSpawns += DistanceBtwSpawns * spawnDirection;
                    GameObject instance;
                    instance =
                        (GameObject)Instantiate(
                            DashEchoPrefab,
                            startPositionBtwSpawns,
                            Quaternion.identity);
                    instance.GetComponent<CopyPlayerMesh>().SetEchoMesh(armature, color);
                    Destroy(instance, destoryTime);
                }
            }
        }
    }
}

