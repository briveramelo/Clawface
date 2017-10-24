using UnityEngine;

public class LevelTransitionManager : MonoBehaviour
{

    #region Unity Lifecycle
    // Use this for initialization
    void OnEnable()
    {
        EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_COMPLETED, LevelComplete);
    }

    void OnDisable() {
        if (EventSystem.Instance) {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.LEVEL_COMPLETED, LevelComplete);
        }
    }
    #endregion

    #region Private Methods
    private void LevelComplete(params object[] parameter)
    {       
        MenuManager.Instance.DoTransition(Strings.MenuStrings.STAGE_OVER,
            Menu.Transition.SHOW, new Menu.Effect[] { Menu.Effect.EXCLUSIVE });
    }
    #endregion

}
