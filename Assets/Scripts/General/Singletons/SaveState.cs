using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveState : Singleton<SaveState> {

    private static int MAX_NUMBER_OF_LEVELS = 12;

    #region Unity  lifecycle
    private void Start()
    {
        Load();
        EventSystem.Instance.RegisterEvent(Strings.Events.UNLOCK_WEAPON, UnlockWeapon);
        EventSystem.Instance.RegisterEvent(Strings.Events.UNLOCK_NEXT_LEVEL, UnlockNextLevel);
    }

    private new void OnDestroy()
    {        
        EventSystem.Instance.UnRegisterEvent(Strings.Events.UNLOCK_WEAPON, UnlockWeapon);
        EventSystem.Instance.UnRegisterEvent(Strings.Events.UNLOCK_NEXT_LEVEL, UnlockNextLevel);
        base.OnDestroy();
    }
    #endregion

    #region public functions
    public void Load()
    {

    }

    public bool GetBool(string key, bool defaultValue)
    {
        int defaultInt = defaultValue ? 1 : 0;        
        return PlayerPrefs.GetInt(key, defaultInt) == 1;
    }

    public void SetBool(string key, bool value)
    {
        int intValue = value ? 1 : 0;
        PlayerPrefs.SetInt(key, intValue);
    }

    public string GetString(string key, string defaultValue)
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }

    public void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    public float GetFloat(string key, float defaultValue)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    public void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }

    public int GetInt(string key, int defaultValue)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    public bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    public void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }

    public void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }

    public void Save()
    {
        PlayerPrefs.Save();
    }
    #endregion

    #region private functions
    private void UnlockWeapon(params object[] parameters)
    {
        if(parameters != null && parameters.Length > 0)
        {
            ModType weapon = (ModType)parameters[0];
            switch (weapon)
            {
                case ModType.Blaster:
                    SetBool(Strings.PlayerPrefStrings.BLASTER_ENABLED, true);
                    break;
                case ModType.Boomerang:
                    SetBool(Strings.PlayerPrefStrings.BOOMERANG_ENABLED, true);
                    break;
                case ModType.Dice:
                    SetBool(Strings.PlayerPrefStrings.DICE_GUN_ENABLED, true);
                    break;
                case ModType.Geyser:
                    SetBool(Strings.PlayerPrefStrings.GEYSER_GUN_ENABLED, true);
                    break;
                case ModType.LightningGun:
                    SetBool(Strings.PlayerPrefStrings.LIGHTNING_GUN_ENABLED, true);
                    break;
                case ModType.SpreadGun:
                    SetBool(Strings.PlayerPrefStrings.SPREAD_GUN_ENABLED, true);
                    break;
            }
        }
    }

    private void UnlockNextLevel(params object[] parameters)
    {
        int latestLevel = GetInt(Strings.PlayerPrefStrings.LATEST_UNLOCKED_LEVEL, 1);
        latestLevel++;
        if(latestLevel > MAX_NUMBER_OF_LEVELS)
        {
            latestLevel = MAX_NUMBER_OF_LEVELS;
        }
        SetInt(Strings.PlayerPrefStrings.LATEST_UNLOCKED_LEVEL, latestLevel);
    }
    #endregion
}
