using UnityEngine;

public class LevelTransitionManager : MonoBehaviour
{

    #region Serialized Unity Inspector fields
    [SerializeField]
    private float checkFrequency = 10.0f;
    #endregion

    #region Private Fields
    private Spawner[] spawners;
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    void Start()
    {
        spawners = FindObjectsOfType<Spawner>();
        InvokeRepeating("CheckForLevelCompletion", checkFrequency, checkFrequency);
    }
    #endregion

    #region Private Methods
    private void CheckForLevelCompletion()
    {
        bool isDone = true;
        foreach (Spawner spawner in spawners)
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

            MenuManager.Instance.DoTransition(Strings.MenuStrings.STAGE_OVER,
                Menu.Transition.SHOW, new Menu.Effect[] { Menu.Effect.EXCLUSIVE });
        }
    }
    #endregion

}
