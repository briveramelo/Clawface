using UnityEngine;
using UnityEngine.UI;

using ModMan; // WAT...

public class MainMenu : Menu
{
    #region Public Fields

    public override Button InitialSelection
    {
        get
        {
            return mainButton;
        }
    }

    public GameObject ObjectToTrack
    {
        set
        {
            objectToTrack = value;
        }
    }

    #endregion

    #region Serialized Unity Fields

    [SerializeField]
    private Button mainButton;

    #endregion

    #region Private Fields
    
    private GameObject objectToTrack = null;

    #endregion

    #region Unity Lifecycle Methods

    private void Update()
    {
        if (objectToTrack != null)
        {
            if (objectToTrack.IsDestroyed())
            {
                objectToTrack = null;
                return;
            }

            Transform self = gameObject.transform;
            Transform track = objectToTrack.transform;

            self.position = track.position;
            self.rotation = track.rotation;

            // this might be funny...
            self.localScale = track.lossyScale;
        }
    }

    #endregion

    #region Public Interface

    public MainMenu() : base(Strings.MenuStrings.MAIN) {}

    //// Actions used by Buttons on this Menu
    public void StartAction()
    {
        MusicManager.Instance.Stop(MusicType.MainMenu_Track);

        Menu pMenu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.PAUSE);
        PauseMenu pauseMenu = pMenu as PauseMenu;
        pauseMenu.CanPause = true;

        Menu lMenu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LOAD);
        LoadMenu loadMenu = lMenu as LoadMenu;
        loadMenu.TargetScene = Strings.Scenes.Arena;

        MenuManager.Instance.DoTransition(loadMenu, Transition.SHOW,
            new Effect[] { Effect.EXCLUSIVE });
    }

    public void CreditsAction()
    {
        MenuManager.Instance.DoTransition(Strings.MenuStrings.CREDITS,
            Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
    }

    public void SettingsAction()
    {
        MenuManager.Instance.DoTransition(Strings.MenuStrings.SETTINGS,
            Transition.SHOW, new Effect[] { Effect.FADE });
    }

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

    #endregion
}
