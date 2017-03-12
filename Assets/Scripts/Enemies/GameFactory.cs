using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameFactory
{
    private static IEnemyFactory m_EnemyFactory = null;

    public static IEnemyFactory GetEnemyFactory()
    {
        if (m_EnemyFactory == null)
            m_EnemyFactory = new EnemyFactory();

        return m_EnemyFactory;
    }

}
