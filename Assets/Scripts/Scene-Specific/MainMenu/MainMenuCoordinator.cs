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
    }

    private void Update()
    {
        if (!mayhemRevealed && InputManager.Instance.AnyKey())
        {
            Blink(ShowMenu);
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

    private void Blink(Callback callback)
    {
        mayhemRevealed = true;
        fadeCanvas.DoShow(1.0F, () =>
        {
            if (callback != null)
            {
                callback();
            }
            track.JumpToPosition(4);
            RevealMayhem();
            fadeCanvas.DoHide(1.0F, null);
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
