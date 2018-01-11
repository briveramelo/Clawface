using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

public class ZombieCelebrateState : AIState {


    public override void OnEnter()
    {
        List<int> celebrations = new List<int>() { 0, 1, 2, 3, 4 };
        animator.SetInteger("VictoryDanceIndex", celebrations.GetRandom());
        navAgent.enabled = false;
        navObstacle.enabled = true;
    }
    public override void Update()
    {
    }
    public override void OnExit()
    {
        navObstacle.enabled = false;
        navAgent.enabled = true;
    }
}
