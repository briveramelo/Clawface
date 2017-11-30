using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraLerp : MonoBehaviour
{

    [SerializeField]
    private CinemachineVirtualCamera cam;

    [SerializeField]
    private float minEnemyDistanceForMinCamSize = 10f;

    [SerializeField]
    private float camSizeGoesUpBy1ForEveryXUnits = 2.5f;

    [SerializeField]
    private float minCameraSize;

    [SerializeField]
    private float maxCameraSize;

    [SerializeField]
    private float lerpMultiplier = 3f;

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private List<GameObject> activeEnemies = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        cam.gameObject.SetActive(true);

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

    // Update is called once per frame
    void Update()
    {
        float greatestDistance = 0f;

        if (activeEnemies.Count == 0)
        {
            cam.m_Lens.OrthographicSize = Mathf.Lerp(cam.m_Lens.OrthographicSize, minCameraSize, Time.deltaTime * lerpMultiplier);
            return;
        }

        foreach (GameObject go in activeEnemies)
        {
            float distanceFromPlayer = Vector3.Distance(player.transform.position, go.transform.position);
            if (distanceFromPlayer > greatestDistance) greatestDistance = distanceFromPlayer;
        }

        float adjustedDistance = (greatestDistance - minEnemyDistanceForMinCamSize) / camSizeGoesUpBy1ForEveryXUnits;
        if (minCameraSize + adjustedDistance < minCameraSize) adjustedDistance = 0f;
        if (minCameraSize + adjustedDistance > maxCameraSize) adjustedDistance = maxCameraSize - minCameraSize;

        cam.m_Lens.OrthographicSize = Mathf.Lerp(cam.m_Lens.OrthographicSize, minCameraSize + adjustedDistance, Time.deltaTime * lerpMultiplier);
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
