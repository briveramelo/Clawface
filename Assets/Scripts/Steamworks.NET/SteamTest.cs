using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;

public class SteamTest : MonoBehaviour {
    protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;
    private CallResult<NumberOfCurrentPlayers_t> m_NumberOfCurrentPlayers;

	// Use this for initialization
	void Start () {
        if (SteamManager.Initialized)
        {
            string name = SteamFriends.GetPersonaName();
            print(name);
        }
	}

    private void OnEnable()
    {
        if (SteamManager.Initialized)
        {
            m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
            m_NumberOfCurrentPlayers = CallResult<NumberOfCurrentPlayers_t>.Create(OnNumberOfCurrentPlayers);
        }
    }

    private void OnNumberOfCurrentPlayers(NumberOfCurrentPlayers_t param, bool bIOFailure)
    {
        if (bIOFailure || param.m_bSuccess != 1)
        {
            print("Error getting number of players");
        }
        else
        {
            print("Number of current players " + param.m_cPlayers);
        }
    }

    private void OnGameOverlayActivated(GameOverlayActivated_t param)
    {
        if(param.m_bActive != 0)
        {
            print("Steam overlay has been activated");
        }
        else
        {
            print("Steam overlay has been closed");
        }
    }

    // Update is called once per frame
    void Update () {
        if (SteamManager.Initialized)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SteamAPICall_t handle = SteamUserStats.GetNumberOfCurrentPlayers();
                m_NumberOfCurrentPlayers.Set(handle);
                print("Called GetNumberOfCurrentPlayers");
            }
        }
	}
}
