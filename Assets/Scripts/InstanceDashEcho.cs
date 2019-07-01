﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TripleBladeHorse.Movement;

public class InstanceDashEcho : MonoBehaviour
{
    [SerializeField]
    private GameObject DashEchoPrefab;
    [SerializeField]
    private GameObject armature;
    [SerializeField]
    private float DistanceBtwSpawns;
    [SerializeField]
    private float destoryTime;
    private Vector3 startPositionBtwSpawns;
    private bool isDash = false;
    private bool isDashOver = true;
    private PlayerMover playerMover;

    private void Start()
    {
        playerMover = this.GetComponent<PlayerMover>();
        playerMover.OnBeginDashingInvincible += DashBeginHandler;
        playerMover.OnStopDashingInvincible += DashEndHandler;
    }

    private void DashBeginHandler()
    {
        isDash = true;
        isDashOver = false;
        startPositionBtwSpawns = this.transform.position;
    }

    private void DashEndHandler()
    {
        isDash = false;
        isDashOver = true;
    }

    private void Update()
    {
        if(isDash)
        {
            if(Vector3.Distance(this.transform.position, startPositionBtwSpawns) > DistanceBtwSpawns )
            {
                Vector3 spawnDirection = (this.transform.position - startPositionBtwSpawns).normalized;
                startPositionBtwSpawns += DistanceBtwSpawns * spawnDirection;
                GameObject instance;
                instance =
                    (GameObject)Instantiate(
                        DashEchoPrefab,
                        startPositionBtwSpawns,
                        Quaternion.identity);
                instance.GetComponent<CopyPlayerMesh>().SetEchoMesh(armature);
                Destroy(instance, destoryTime);
            }
        }
    }

}
