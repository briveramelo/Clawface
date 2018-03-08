﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class PlayOnVideoFinish : MonoBehaviour {

    [SerializeField]
    private VideoPlayer player;

    [SerializeField]
    private string levelToLoad;

    [SerializeField]
    private Animation blackScreen;

    private bool fadeTriggered;

    private PauseMenu menu;

    private void Start()
    {
        menu = (PauseMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.PAUSE);
        menu.CanPause = false;
    }

    // Update is called once per frame
    void Update () {
		if (!player.isPlaying && !fadeTriggered)
        {
            fadeTriggered = true;
            blackScreen.Play();
        }

        if (InputManager.Instance.QueryAction(Strings.Input.UI.SUBMIT, ButtonMode.DOWN) || InputManager.Instance.QueryAction(Strings.Input.Actions.PAUSE, ButtonMode.DOWN) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
        {
            LoadLevel();
        }
	}

    public void LoadLevel()
    {
        MenuMusicManager.Instance.Play();
        menu.CanPause = true;
        SceneManager.LoadScene(levelToLoad);
    }
}
