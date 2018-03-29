using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraAverage : EventSubscriber {

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

    [SerializeField]
    private float enemyRadius = 1;

    [SerializeField]
    private float enemyBaseWeight = 1;

    #region Event Subscriptions
    protected override LifeCycle SubscriptionLifecycle { get { return LifeCycle.StartDestroy; } }
    protected override Dictionary<string, FunctionPrototype> EventSubscriptions {
        get {
            return new Dictionary<string, FunctionPrototype>() {
                {Strings.Events.ENEMY_SPAWNED, OnEnemySpawned },
                {Strings.Events.DEATH_ENEMY, OnEnemyKilled},
            };
        }
    }
    #endregion

    // Use this for initialization
    protected override void Start() {
        base.Start();
        activeEnemies = new List<GameObject>();
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

        SetCinemachineTargetGroup();
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

        SetCinemachineTargetGroup();
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
            targetGroup.m_Targets[i + 1].radius = enemyRadius;
            targetGroup.m_Targets[i + 1].weight = enemyBaseWeight;
        }
    }


}
