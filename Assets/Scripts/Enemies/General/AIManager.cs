using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : Singleton<AIManager> {

    #region 0. Private fields
    private List<AIEnemyData> enemyData;
    private float separationDistance = 10;
    private bool removingEnemy = false;
    private bool isPlayerDead = false;
    #endregion

    #region 1. UnityLifeCycle
    public void OnEnable()
    {
        EventSystem.Instance.RegisterEvent(Strings.Events.PLAYER_KILLED, SetPlayerDead);
        EventSystem.Instance.RegisterEvent(Strings.Events.PLAYER_BIRTHED, SetPlayerNotDead);
    }

    public new void Awake()
    {
        base.Awake();
        enemyData = new List<AIEnemyData>();
    }

    public void OnDisable()
    {
        if (EventSystem.Instance) {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLAYER_KILLED, SetPlayerDead);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLAYER_BIRTHED, SetPlayerNotDead);
        }
    }
    #endregion

    #region 2. Public methods
    public bool AssignPosition(AIEnemyData enemy)
    {
        if (!removingEnemy)
        {
            //Check if the list is empty
            if (enemyData.Count < 1)
            {
                enemyData.Add(enemy);
                return true;
            }

            //Enemy in the list
            else if (enemyData.Count >= 1)
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
        return false;
    }

    public void Remove(AIEnemyData enemy)
    {
        removingEnemy = true;
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
        removingEnemy = false;
    }
    public bool GetPlayerDead()
    {
        return isPlayerDead;
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

    private void SetPlayerDead(object[] parameters)
    {
        isPlayerDead = true;
    }

    private void SetPlayerNotDead(object[] parameters)
    {
        isPlayerDead = false;
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

