/**
 *  @author Cornelia Schultz
 */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadMenu : Menu {

    #region Constants
    private static readonly float LOADING_TIME = 3.0F; // seconds
    #endregion

    #region Public Fields
    public string TargetScene
    {
        get
        {
            return target;
        }
        set
        {
            target = value;
        }
    }
    public bool Fast
    {
        get
        {
            return fast;
        }
        set
        {
            fast = value;
        }
    }

    public override Button InitialSelection
    {
        get
        {
            return null;
        }
    }

    #endregion

    #region Serialized Unity Fields
    [SerializeField]
    private Scrollbar loadingBar;
    [SerializeField]
    private Text loadingText;
    #endregion

    #region Private Fields
    private bool loaded = false;
    private bool fast = false;
    private string target = "";
    #endregion

    #region Unity Lifecycle Methods
    void Update()
    {
        if (loaded && (InputManager.Instance.AnyKey() || fast))
        {
            fast = false;
            loaded = false;
            loadingText.text = "Starting...";
            MenuManager.Instance.DoTransition(this, Transition.HIDE, new Effect[] { });
            EventSystem.Instance.TriggerEvent(Strings.Events.LEVEL_DISPLAYED, target);
            EventSystem.Instance.TriggerEvent(Strings.Events.CALL_NEXTWAVEENEMIES);
        }
    }
    #endregion

    #region Public Interface
    public LoadMenu() : base(Strings.MenuStrings.LOAD) {}
    #endregion

    #region Protected Interface

    protected override void DefaultShow(Transition transition, Effect[] effects)
    {
        Fade(transition, effects);
    }

    protected override void DefaultHide(Transition transition, Effect[] effects)
    {
        Fade(transition, effects);
    }

    protected override void ShowComplete()
    {
        base.ShowComplete();
        StartCoroutine(LoadingCoroutine());
    }
    protected override void HideComplete()
    {
        base.HideComplete();
        loadingBar.size = 0.0F;
        loadingText.text = "Loading";
    }

    #endregion

    #region Private Interface

    private IEnumerator LoadingCoroutine()
    {
        MovementEffects.Timing.KillCoroutines();
        ObjectPool.Instance.ResetPools();
        loadingBar.size = 0.0F;
        loaded = false;

        AsyncOperation async = SceneManager.LoadSceneAsync(target);
        while (!async.isDone)
        {
            loadingBar.size = Mathf.Lerp(0.0F, 1.0F, async.progress);
            yield return new WaitForEndOfFrame();
        }

        loadingBar.size = 1.0F;
        loadingText.text = "Press any key to continue...";
        loaded = true;
    }

    #endregion
}
