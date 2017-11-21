using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraAverage : MonoBehaviour {

    [SerializeField]
    private CinemachineTargetGroup targetGroup;

    [SerializeField]
    private List<GameObject> activeEnemies;

    [SerializeField]
    private Transform player;

    [SerializeField]
    private float playerRadius;

    [SerializeField]
    private float playerBaseWeight;

    

    // Use this for initialization
    void Start () {
        activeEnemies = new List<GameObject>();

        EventSystem.Instance.RegisterEvent(Strings.Events.ENEMY_SPAWNED, OnEnemySpawned);
        EventSystem.Instance.RegisterEvent(Strings.Events.DEATH_ENEMY, OnEnemyKilled);
    }

    private void OnDestroy()
    {
        if (EventSystem.Instance)
        {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.ENEMY_SPAWNED, OnEnemySpawned);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.DEATH_ENEMY, OnEnemyKilled);
        }
    }


    private void OnEnemySpawned(params object[] objects)
    {

        GameObject enemy;

        if (objects != null && objects[0] != null)
        {
            enemy = (GameObject) objects[0];
        }
        else
        {
            return;
        }

        activeEnemies.Add(enemy);

        // SetCinemachineTargetGroup();
    }

    void OnEnemyKilled(params object[] objects)
    {
        GameObject enemy;

        if (objects != null && objects[0] != null)
        {
            enemy = (GameObject)objects[0];
        }
        else
        {
            return;
        }

        activeEnemies.Remove(enemy);

        // SetCinemachineTargetGroup();
    }

    private void SetCinemachineTargetGroup()
    {
        targetGroup.m_Targets = new CinemachineTargetGroup.Target[activeEnemies.Count + 1];
        targetGroup.m_Targets[0].target = player;
        targetGroup.m_Targets[0].radius = playerRadius;
        targetGroup.m_Targets[0].weight = Mathf.Max(playerBaseWeight, playerBaseWeight * activeEnemies.Count);

        for (int i = 0; i < activeEnemies.Count; i++)
        {
            targetGroup.m_Targets[i + 1].target = activeEnemies[i].transform;
            targetGroup.m_Targets[i + 1].radius = 1;
            targetGroup.m_Targets[i + 1].weight = 1;
        }
    }


}
