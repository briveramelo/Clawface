using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCulling : MonoBehaviour
{


    #region Serialized
    [SerializeField]
    private float margin = 0.1f;

    [SerializeField]
    private float timeTilCull = 5f;

    [SerializeField]
    private EnemyBase enemyBase;

    #endregion


    #region Privates
    private Camera mainCamera;

    private float zeroMinusMargin;
    private float onePlusMargin;

    private float cullTimer;
    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        zeroMinusMargin = 0 - margin;
        onePlusMargin = 1 + margin;
    }

    private void OnEnable()
    {
        mainCamera = Camera.main;
        cullTimer = 0f;
    }


    // Update is called once per frame
    void Update()
    {
        if (enemyBase.IsEatable()) {
            Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
            bool onScreen = screenPoint.z > zeroMinusMargin && screenPoint.x > zeroMinusMargin && screenPoint.x < onePlusMargin && screenPoint.y > zeroMinusMargin && screenPoint.y < onePlusMargin;

            if (!onScreen)
            {
                cullTimer += Time.deltaTime;

                if (cullTimer > timeTilCull)
                {
                    enemyBase.OnDeath();
                }
            }
            else
            {
                cullTimer = 0f;
            }
        }
    }



    #endregion

}
