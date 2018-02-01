using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLEBlockUnit : MonoBehaviour {

    #region Public Fields

    

    #endregion

    #region Private Fields

    private bool occupied;
    [SerializeField] private int blockID = 0;
    [SerializeField] public Transform spawnPos;
    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        occupied = false;
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
        return spawnPos.position;
    }
    #endregion

}
