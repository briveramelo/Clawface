using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class CameraSize : EventSubscriber
{

    [SerializeField]
    private List<CinemachineVirtualCamera> shotList;

    [SerializeField]
    private List<float> enemyDistances;

    [SerializeField]
    private CinemachineVirtualCamera virtualCamOnStart;

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private List<GameObject> activeEnemies = new List<GameObject>();


    #region Event Subscriptions
    protected override LifeCycle SubscriptionLifecycle { get { return LifeCycle.StartDestroy; } }
    protected override Dictionary<string, UnityAction<object[]>> EventSubscriptions {
        get {
            return new Dictionary<string, UnityAction<object[]>>() {
                { Strings.Events.ENEMY_SPAWNED, OnEnemySpawned },
                {Strings.Events.DEATH_ENEMY, OnEnemyKilled},
            };
        }
    }
    #endregion

    // Use this for initialization
    protected override void Start() {
        foreach (CinemachineVirtualCamera cam in shotList) {
            cam.gameObject.SetActive(false);
        }

        virtualCamOnStart.gameObject.SetActive(true);
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        float greatestDistance = 0f;

        if (activeEnemies.Count == 0)
        {
            SetAllCamsInactiveButOne(0);
            return;
        }

        foreach (GameObject go in activeEnemies)
        {
            float distanceFromPlayer = Vector3.Distance(player.transform.position, go.transform.position);
            if (distanceFromPlayer > greatestDistance) greatestDistance = distanceFromPlayer;
        }

        for (int i = 0; i < shotList.Count; i++)
        {
            if (greatestDistance < enemyDistances[i])
            {
                SetAllCamsInactiveButOne(i);
                return;
            }
        }

        if (greatestDistance > enemyDistances[shotList.Count - 1])
        {
            SetAllCamsInactiveButOne(shotList.Count - 1);
        }
    }

    private void SetAllCamsInactiveButOne(int i)
    {
        foreach(CinemachineVirtualCamera cam in shotList)
        {
            cam.gameObject.SetActive(false);
        }

        shotList[i].gameObject.SetActive(true);
    }

    private void OnEnemySpawned(params object[] objects)
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

        activeEnemies.Add(enemy);
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
    }
}
