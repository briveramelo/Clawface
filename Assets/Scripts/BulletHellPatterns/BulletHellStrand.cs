using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHellStrand{

    public Vector3 movementDirection;

    public BulletHellStrand(Vector3 movementDirection)
    {
        this.movementDirection = movementDirection;
    }

    public void SetMovement(Vector3 direction)
    {
        movementDirection = direction;
    }


}
