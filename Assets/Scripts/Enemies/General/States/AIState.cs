using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AIState : State
{
    public AIProperties properties;
    public AIController controller;
    public VelocityBody velBody;
    public Animator animator;
    public Stats myStats;
    public NavMeshAgent navAgent;
    public NavMeshObstacle navObstacle;
    public BulletHellPatternController bulletPatternController;

    public void Initialize(
        AIProperties properties,
        AIController controller,
        VelocityBody velBody,
        Animator animator,
        Stats myStats,
        NavMeshAgent navAgent,
        NavMeshObstacle navObstacle)
    {
        this.properties = properties;
        this.controller = controller;
        this.velBody = velBody;
        this.animator = animator;
        this.myStats = myStats;
        this.navAgent = navAgent;
        this.navObstacle = navObstacle;
    }


    public void Initialize(
        AIProperties properties,
        AIController controller,
        VelocityBody velBody,
        Animator animator,
        Stats myStats,
        NavMeshAgent navAgent,
        NavMeshObstacle navObstacle,
        BulletHellPatternController bulletPatternController)
    {
        this.properties = properties;
        this.controller = controller;
        this.velBody = velBody;
        this.animator = animator;
        this.myStats = myStats;
        this.navAgent = navAgent;
        this.navObstacle = navObstacle;
        this.bulletPatternController = bulletPatternController;
    }
}

[System.Serializable]
public abstract class AIProperties
{
    [HideInInspector] public float runMultiplier;
    [HideInInspector] public float hitRate;
    [HideInInspector] public float selfDestructTime;
    [HideInInspector] public float blastRadius;
    [HideInInspector] public float rotationSpeed;
    [HideInInspector] public int minBounces;
    [HideInInspector] public int maxBounces;
    [HideInInspector] public int minShots;
    [HideInInspector] public int maxShots;
    [HideInInspector] public int numberOfShots;
    [HideInInspector] public bool rotate;
    [HideInInspector] public float kamikazeProbability;
    [HideInInspector] public float kamikazePulserProbability;
    [HideInInspector] public int maxPulses;
    [HideInInspector] public float pulseRate;
    [HideInInspector] public float scaleRate;
    [HideInInspector] public float maxScale;
}