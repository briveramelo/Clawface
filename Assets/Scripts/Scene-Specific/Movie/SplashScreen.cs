using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System;

public class SplashScreen : MonoBehaviour {

    [SerializeField]
    private VideoPlayer player;

    [SerializeField]
    private string levelToLoad;

    [SerializeField]
    private Animation blackScreen;

    private bool fadeTriggered;
    private bool levelsLoaded = false;

    private PauseMenu menu;

    private void Start()
    {
        menu = (PauseMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.PAUSE);
        menu.CanPause = false;

        LoadSteamworkshopFiles();
    }

    // Update is called once per frame
    void Update () {
		if (!player.isPlaying && !fadeTriggered)
        {
            fadeTriggered = true;
            blackScreen.Play();
        }

        if (InputManager.Instance.AnyKey() && levelsLoaded)
        {
            LoadLevel();
        }
	}

    public void LoadLevel()
    {
        
        menu.CanPause = true;
        SceneManager.LoadScene(levelToLoad);
        EventSystem.Instance.TriggerEvent(Strings.Events.SCENE_LOADED);
    }

    #region Private Interface

    private void LoadSteamworkshopFiles()
    {
        SteamAdapter.LoadSteamLevelData(OnAllLevelsLoaded);
    }

    private void OnAllLevelsLoaded()
    {
        print("all levels loaded");
        levelsLoaded = true;
    }


    #endregion
}
