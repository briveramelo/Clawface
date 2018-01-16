﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : Singleton<AIManager> {

    #region 0. Private fields
    private List<AIEnemyData> enemyData;
    private float separationDistance = 10;
    #endregion

    #region 1. UnityLifeCycle
    public new void Awake()
    {
        base.Awake();
        enemyData = new List<AIEnemyData>();
    }
    #endregion

    #region 2. Public methods
    public bool Contains(AIEnemyData enemy)
    {
        //Enemy in the list
        if (enemyData.Contains(enemy))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public bool AssignPosition(AIEnemyData enemy)
    {
        //Check if the list is empty
        if (enemyData.Count < 1)
        {
            enemyData.Add(enemy);
            return true;
        }

        //Enemy in the list
        if (enemyData.Count >= 1)
        {
            bool enemyFound = false;
            for (int i = 0; i < enemyData.Count; i++)
            {
                if (enemyData[i].enemyId == enemy.enemyId)
                {
                    enemyData[i].targetPosition = enemy.targetPosition;
                    enemyFound = true;
                }
                
            }

            if (!enemyFound)
            {
                enemyData.Add(enemy);
            }

            return ComparePosition(enemy);
        }
        //Enemy is not in the list
        else
        {
            return true;
        }

    }

    public void Remove(AIEnemyData enemy)
    {
        if (enemyData.Count > 0)
        {
            for (int i = 0; i < enemyData.Count; i++)
            {
                if (enemyData[i].enemyId == enemy.enemyId)
                {
                    enemyData.RemoveAt(i);
                }

            }
        }
    }


    #endregion

    #region 3. Private methods
    private bool ComparePosition(AIEnemyData enemy)
    {
        for (int i = 0; i < enemyData.Count; i++)
        {
            if (enemyData[i].enemyId != enemy.enemyId)
            {
                if (Vector3.Distance(enemy.targetPosition, enemyData[i].targetPosition) < separationDistance)
                {
                    return false;
                }
            } 
        }
        return true;
    }
    #endregion
}

public class AIEnemyData
{
    public int enemyId;
    public Vector3 targetPosition;

    public AIEnemyData(int p_enemyId, Vector3 p_targetPosition)
    {
        this.enemyId = p_enemyId;
        this.targetPosition = p_targetPosition;
    }

    public AIEnemyData(int p_enemyId)
    {
        this.enemyId = p_enemyId;
    }

}
