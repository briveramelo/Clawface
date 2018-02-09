﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLEBlockUnit : MonoBehaviour
{
    #region Public Fields

    #endregion

    #region Private Fields

    private bool occupied;
    [SerializeField] private int blockID = 0;
    [SerializeField] public Transform spawnTrans;

    List<LevelUnitStates> LevelStates = new List<LevelUnitStates>();

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        occupied = false;
    }

    public void Start()
    {
        LevelStates.Add(LevelUnitStates.floor);
        LevelStates.Add(LevelUnitStates.floor);
        LevelStates.Add(LevelUnitStates.floor);

        if (EventSystem.Instance)
        {
            EventSystem.Instance.RegisterEvent(Strings.Events.PLE_ADD_WAVE, AddWave);
            EventSystem.Instance.RegisterEvent(Strings.Events.PLE_UPDATE_LEVELSTATE, UpdateDynamicLevelState);
        }
    }

    private void OnDestroy()
    {
        if (EventSystem.Instance)
        {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLE_ADD_WAVE, AddWave);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLE_UPDATE_LEVELSTATE, UpdateDynamicLevelState);
        }
    }

    #endregion

    #region Public Interface

    public void SetOccupation(bool i_state)
    {
        occupied = i_state;
    }

    public bool GetOccupation()
    {
        return occupied;
    }

    public void SetBlockID(int i_id)
    {
        blockID = i_id;
    }

    public int GetBlockID()
    {
        return blockID;
    }

    public Vector3 GetSpawnPosition()
    {
        return spawnTrans.position;
    }



    public void AddWave(params object[] parameters)
    {
        Debug.Log("Add Wave");
        //       LevelStates.Add(LevelUnitStates.floor);
    }

    public void UpdateDynamicLevelState(params object[] parameters)
    {
        Debug.Log("Update Wave");

        LevelUnit LU = GetComponent<LevelUnit>();
        LU.SetCurrentState(LevelStates[WaveSystem.currentWave]);
    }

    #endregion
}