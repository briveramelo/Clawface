using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointManager : MonoBehaviour {

    #region Public Fields

    [HideInInspector] public Transform spawnPos;

    #endregion

    #region Private Fields

    private bool occupied;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        spawnPos = transform;
        occupied = false;
    }

    #endregion

    #region Public Interface

    public void SetOccupation(bool i_state)
    {
        occupied = i_state;
    }


    public bool GetOccupied()
    {
        return occupied;
    }

    #endregion

}
