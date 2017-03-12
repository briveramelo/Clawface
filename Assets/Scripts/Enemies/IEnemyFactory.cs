using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IEnemyFactory
{
    public abstract IEnemy CreateEnemy(GameObject m_GameObject, Enemy_Type i_type);
}
