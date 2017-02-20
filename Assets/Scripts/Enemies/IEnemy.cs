using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IEnemy : ICharacter
{
    public IEnemy(GameObject i_Target) : base(i_Target)
    {

    }

    protected int type = 0;
}
