using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IEnemyFactory
{
    public abstract void CreateEnemy(Enemy_Type i_type, GameObject m_GameObject);
}
