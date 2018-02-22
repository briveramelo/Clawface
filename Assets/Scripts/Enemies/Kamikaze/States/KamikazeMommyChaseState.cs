﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamikazeMommyChaseState : AIState {

    private float currentSpawnRate = 0.0f;

    public override void OnEnter()
    {
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.Walk);
        controller.AttackTarget = controller.FindPlayer();
        navAgent.speed = myStats.moveSpeed;
    }
    public override void Update()
    {
        Chase();

        currentSpawnRate += Time.deltaTime;

        if (currentSpawnRate > properties.spawnRate)
        {
            SpawnKamikazeMinion();
            currentSpawnRate = 0.0f;
        }

    }

    private void SpawnKamikazeMinion()
    {

        float probabilitySum = properties.kamikazeProbability + properties.kamikazePulserProbability;
        float randomRoll = Random.Range(0.0f, probabilitySum);

        //Check random roll case

        if (randomRoll <= properties.kamikazeProbability)
        {
            GameObject kamikaze = ObjectPool.Instance.GetObject(PoolObjectType.Kamikaze);
            if (kamikaze)
            {
                kamikaze.transform.position = controller.transform.position;
            }
        }
        else if (randomRoll > properties.kamikazeProbability && randomRoll <= probabilitySum)
        {
            GameObject kamikaze = ObjectPool.Instance.GetObject(PoolObjectType.KamikazePulser);
            if (kamikaze)
            {
                kamikaze.transform.position = controller.transform.position;
            }
        }
    }

    public override void OnExit()
    {

    }

    private void Chase()
    {
        if (navAgent.enabled && navAgent.isOnNavMesh)
            navAgent.SetDestination(controller.AttackTarget.position);
    }
}