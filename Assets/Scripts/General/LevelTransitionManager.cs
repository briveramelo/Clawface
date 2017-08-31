using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransitionManager : MonoBehaviour {

    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField]
    private float checkFrequency = 10.0f;

    [SerializeField]
    private GameObject endLevelCanvas;
    #endregion

    #region Private Fields
    private Spawner[] spawners;    
    private bool levelIsComplete;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        levelIsComplete = false;
        endLevelCanvas.SetActive(false);
    }

    // Use this for initialization
    void Start () {
        spawners = FindObjectsOfType<Spawner>();        
        InvokeRepeating("CheckForLevelCompletion", checkFrequency, checkFrequency);
    }

    private void Update()
    {
        if (levelIsComplete)
        {
            if (InputManager.Instance.AnyKey())
            {
                SceneManager.LoadScene(0);
            }
        }
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    private void CheckForLevelCompletion()
    {
        bool isDone = true;
        foreach(Spawner spawner in spawners)
        {
            if (spawner.IsLastWave() && spawner.IsAllEnemyClear())
            {
                isDone = true;
            }
            else
            {
                isDone = false;
                break;
            }
        }
        if (isDone)
        {
            CancelInvoke();
            Debug.Log("Level done");
            
            levelIsComplete = true;
            endLevelCanvas.SetActive(true);
        }
    }
    #endregion

    #region Public Structures
    #endregion

    #region Private Structures
    #endregion

}
