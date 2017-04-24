//Brandon Rivera-Melo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MallCopState : State {

    public MallCopProperties properties;
    public MallCopController controller;
    public VelocityBody velBody;
    public Animator animator;
    public Stats myStats;

    public void Initialize(
        MallCopProperties properties,
        MallCopController controller,
        VelocityBody velBody,
        Animator animator,
        Stats myStats) {

        this.properties = properties;
        this.controller = controller;
        this.velBody = velBody;
        this.animator = animator;
        this.myStats = myStats;
    }
}
