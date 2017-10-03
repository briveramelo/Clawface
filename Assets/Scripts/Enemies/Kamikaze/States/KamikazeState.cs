using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class KamikazeState : State {

    public KamikazeProperties properties;
    public KamikazeController controller;
    public VelocityBody velBody;
    public Animator animator;
    public Stats myStats;
    public NavMeshAgent navAgent;

    public void Initialize(
        KamikazeProperties properties,
        KamikazeController controller,
        VelocityBody velBody,
        Animator animator,
        Stats myStats,
        NavMeshAgent navAgent      
        )
    {

        this.properties = properties;
        this.controller = controller;
        this.velBody = velBody;
        this.animator = animator;
        this.myStats = myStats;
        this.navAgent = navAgent;
    }
}
