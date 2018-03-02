//Brandon Rivera-Melo
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class AchievementManager : Singleton<AchievementManager> {

    #region Private variables
    private bool isInitialized;
    private CSteamID userId;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        if (SteamManager.Initialized)
        {
            isInitialized = SteamUserStats.RequestCurrentStats();
            userId = SteamUser.GetSteamID();
        }
    }
    #endregion

    #region Public methods
    public bool SetAchievement(string achievementName)
    {
        bool result = false;
        result = SteamUserStats.SetAchievement(achievementName);
        if (result)
        {
            result = SteamUserStats.StoreStats();
        }
        return result;
    }

    public bool UpdateAchievementProgress(string achievementName, uint currentProgress, uint maxProgress)
    {
        bool result = false;
        result = SteamUserStats.IndicateAchievementProgress(achievementName, currentProgress, maxProgress);
        return result;
    }
    #endregion

}
