using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GrapplingBotState : State {

    public GrapplingBotProperties properties;
    public GrapplingBotController controller;
    public VelocityBody velBody;
    public Animator animator;
    public Stats myStats;

    public void Initialize(
        GrapplingBotProperties properties,
        GrapplingBotController controller,
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
