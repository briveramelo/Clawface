using UnityEngine;
using UnityEngine.Video;

// This class is simply used to coordinate actions on the
// main scene involving menus.
public class MainMenuCoordinator : MonoBehaviour {

    #region Unity Serialization Fields

    [SerializeField]
    private CanvasFader fadeCanvas;

    [SerializeField]
    private GameObject[] mayhem;

    [SerializeField]
    private VideoPlayer projector;

    [SerializeField]
    private VideoClip staticLoop;

    [SerializeField]
    private CameraTrack track;

    #endregion

    #region Private Fields

    private bool mayhemRevealed = false;

    #endregion

    #region Unity Lifecycle Methods

    private void Start()
    {
        // Disable input
        MenuManager.Instance.EnableEventSystem(false);

        // Set Up tracking.
        Menu menu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.MAIN);
        MainMenu mainMenu = menu as MainMenu;
        mainMenu.ObjectToTrack = Camera.main.gameObject;

        // Disable Pausing
        menu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.PAUSE);
        PauseMenu pauseMenu = menu as PauseMenu;
        pauseMenu.CanPause = false;

        // Disable ModManager assign from pool
        ModManager.assignFromPool = false;
    }

    private void Update()
    {
        if (!mayhemRevealed && InputManager.Instance.AnyKey())
        {
            Blink(ShowMenu, 0.15F);
        }
    }

    #endregion

    #region Public Interface

    public void DoStartUp()
    {
        fadeCanvas.DoHide(1.0F, null);

        MusicManager.Instance.PlayMusic(MusicType.MainMenu_Track,
            Camera.main.gameObject.transform.position);
    }

    public void ShowMenu()
    {
        MenuManager.Instance.EnableEventSystem(true);
        MenuManager.Instance.DoTransition(Strings.MenuStrings.MAIN, Menu.Transition.SHOW,
            new Menu.Effect[] { Menu.Effect.EXCLUSIVE });
    }

    public void Blink()
    {
        Blink(null);
    }

    #endregion

    #region Private Interface

    private void Blink(Callback callback, float duration = 1.0F)
    {
        mayhemRevealed = true;
        fadeCanvas.DoShow(duration, () =>
        {
            track.JumpToPosition(4);
            RevealMayhem();
            if (callback != null)
            {
                callback();
            }
            fadeCanvas.DoHide(duration, null);
        });
    }

    private void RevealMayhem()
    {
        // Show the Objects
        foreach (GameObject obj in mayhem)
        {
            obj.SetActive(true);
        }

        // Reset the video player
        projector.clip = staticLoop;
        projector.isLooping = true;
    }

    #endregion

    #region Types

    private delegate void Callback();

    #endregion
}
