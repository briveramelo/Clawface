using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAIState : IAIState
{
    public RotateAIState() { }

    public override void Update()
    {
        Debug.Log("Rotation");
    }
}