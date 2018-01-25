using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DeathcamSpin : MonoBehaviour
{

    #region Serialized
    [SerializeField] private Transform deathCamera;
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    void Start()
    {
        EventSystem.Instance.RegisterEvent(Strings.Events.PLAYER_KILLED, OnDeath);
    }

    private void OnDestroy()
    {
        if (EventSystem.Instance)
        {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLAYER_KILLED, OnDeath);
        }
    }
    #endregion


    #region Public


    #endregion

    #region Private
    public void OnDeath(params object[] items)
    {
        deathCamera.gameObject.SetActive(true);
    }
    #endregion
}
