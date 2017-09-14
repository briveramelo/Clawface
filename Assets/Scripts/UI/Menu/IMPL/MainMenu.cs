using UnityEngine;
using UnityEngine.UI;

using ModMan; // WAT...

public class MainMenu : Menu
{
    #region Public Fields

    public override bool Displayed
    {
        get
        {
            return displayed;
        }
    }

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

    private bool displayed = false;
    private GameObject objectToTrack = null;

    #endregion

    #region Unity Lifecycle Methods

    protected override void Start()
    {
        base.Start();
    }

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

    public override void DoTransition(Transition transition, Effect[] effects)
    {
        switch (transition)
        {
            case Transition.SHOW:
                if (displayed) return;
                OnTransitionStarted(transition, effects);
                StartCoroutine(MenuTransitionsCommon.FadeCoroutine(0.0F, 1.0F, 1.0F,
                    canvasGroup, () => { ShowComplete(); OnTransitionEnded(transition, effects); }));
                break;
            case Transition.HIDE:
                if (!displayed) return;
                OnTransitionStarted(transition, effects);
                StartCoroutine(MenuTransitionsCommon.FadeCoroutine(1.0F, 0.0F, 1.0F,
                    canvasGroup, () => { HideComplete(); OnTransitionEnded(transition, effects); }));
                break;
            case Transition.TOGGLE:
                DoTransition(displayed ? Transition.HIDE : Transition.SHOW, effects);
                return;
        }
    }

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

    #region Private Fields

    private void ShowComplete()
    {
        displayed = true;
    }

    private void HideComplete()
    {
        displayed = false;
    }

    #endregion
}
