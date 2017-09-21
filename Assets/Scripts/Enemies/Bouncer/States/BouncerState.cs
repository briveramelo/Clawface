using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BouncerState : State {

    public BouncerProperties properties;
    public BouncerController controller;
    public VelocityBody velBody;
    public Animator animator;
    public Stats myStats;
    public NavMeshAgent navAgent;

    public void Initialize(
        BouncerProperties properties,
        BouncerController controller,
        VelocityBody velBody,
        Animator animator,
        Stats myStats,
        NavMeshAgent navAgent)
    {

        this.properties = properties;
        this.controller = controller;
        this.velBody = velBody;
        this.animator = animator;
        this.myStats = myStats;
        this.navAgent = navAgent;
    }
}
