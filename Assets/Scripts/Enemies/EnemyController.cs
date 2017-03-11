using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController
{
    public EnemyController()
    {
        if(m_Enemys == null)
        {
            m_Enemys = new List<IEnemy>();
        }
    }

    private static List<IEnemy> m_Enemys = null;

    public void AddEnemy(IEnemy i_Enemy)
    {
        m_Enemys.Add(i_Enemy);
    }

    public void RemoveEnemy(IEnemy i_Enemy)
    {
        m_Enemys.Remove(i_Enemy);
    }

    public int GetEnemyCount()
    {
        return m_Enemys.Count;
    }

    public void Update()
    {
        UpdateAI();
    }


    private void UpdateAI()
    {
        foreach(IEnemy enemy in m_Enemys)
        {
            enemy.UpdateAI();
        }
    }

}
