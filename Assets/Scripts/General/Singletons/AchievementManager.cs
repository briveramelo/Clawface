//Brandon Rivera-Melo
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;

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

            ResetAchievements();

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

    #region private methods
    private void ResetAchievements()
    {
#if UNITY_EDITOR
        Strings.AchievementNames names = new Strings.AchievementNames();
        System.Reflection.FieldInfo[] fields = names.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        foreach(System.Reflection.FieldInfo field in fields)
        {
            string achievementName = (string)field.GetValue(null);
            if (achievementName != null)
            {
                SteamUserStats.ClearAchievement(achievementName);
            }
        }
#endif
    }
    #endregion

}
