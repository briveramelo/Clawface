using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System;

public class SplashScreen : MonoBehaviour {

    #region Unity Serialized Fields

    [SerializeField] private VideoPlayer player;
    [SerializeField] private Animation blackScreen;

    #endregion

    #region Private Fields

    private bool fadeTriggered;
    private bool levelsLoaded = false;
    private PauseMenu menu;

    #endregion

    #region Unity LifeCycle

    private void Start()
    {
        menu = (PauseMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.PAUSE);
        menu.CanPause = false;
        Invoke("LoadSteamworkshopFiles", 0.1f);
    }

    // Update is called once per frame
    void Update () {
		if (!player.isPlaying && !fadeTriggered)
        {
            fadeTriggered = true;
            blackScreen.Play();
        }

        //if level has loaded and player has hit a key, or if the levels have been loaded and the player is done playing
        if (InputManager.Instance.AnyKey() && levelsLoaded || (levelsLoaded && !player.isPlaying))
        {
            LoadLevel();
        }
	}

    #endregion

    #region Public Interface

    public void LoadLevel()
    {
        
        menu.CanPause = true;
        SceneManager.LoadScene(Strings.Scenes.SceneNames.MainMenu);
        EventSystem.Instance.TriggerEvent(Strings.Events.SCENE_LOADED);
    }

    #endregion 

    #region Private Interface

    private void LoadSteamworkshopFiles()
    {
        SteamAdapter.LoadSteamLevelData(OnAllLevelsLoaded);
    }

    private void OnAllLevelsLoaded()
    {
        levelsLoaded = true;
    }
    #endregion
}
