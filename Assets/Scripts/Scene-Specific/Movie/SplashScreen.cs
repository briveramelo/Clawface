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
    [SerializeField] private SteamLevelLoader steamLevelLoader;
    #endregion

    #region Private Fields

    private bool fadeTriggered;
    private PauseMenu menu;

    #endregion

    #region Unity LifeCycle

    private void Start()
    {
        menu = (PauseMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.PAUSE);
        menu.CanPause = false;
        Invoke("LoadWorkshopLevels", .1f);
    }

    // Update is called once per frame
    void Update () {
		if (!player.isPlaying && !fadeTriggered)
        {
            fadeTriggered = true;
            blackScreen.Play();
        }
        
        if ((InputManager.Instance.AnyKey() || !player.isPlaying) && steamLevelLoader.IsLevelsLoaded)
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

    private void LoadWorkshopLevels() {
        try
        {
            steamLevelLoader.LoadSteamworkshopFiles();
        }
        catch
        {
            Debug.LogError("Must be launched from steam client to load Steam Workshop Files");
        }
        
    }
    #endregion
}
