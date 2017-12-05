using UnityEngine;

public class MainMenuController : MonoBehaviour {

    #region Fields (Unity Serialization)

    [SerializeField]
    private CanvasFader entranceFader;

    [SerializeField]
    private float fadeTimeSlow = 3F;

    [SerializeField]
    private float fadeTimeFast = 0.25F;

    #endregion

    #region Fields (Private)

    private static bool firstLaunch = true;

    #endregion

    #region Interface (Unity Lifecycle)   

    private void Start () {
        MenuManager.Instance.DoTransition(Strings.MenuStrings.MAIN, Menu.Transition.SHOW,
            new Menu.Effect[] { Menu.Effect.INSTANT, Menu.Effect.EXCLUSIVE });
        HideFader();
	}

    #endregion

    #region Interface (Private)

    private void HideFader()
    {
        float fadeTime = fadeTimeFast;
        if (firstLaunch)
        {
            firstLaunch = false;
            fadeTime = fadeTimeSlow;
        }

        entranceFader.DoHide(fadeTime, null);
    }

    #endregion
}
