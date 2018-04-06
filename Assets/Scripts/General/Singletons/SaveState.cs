using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveState : Singleton<SaveState> {

    [SerializeField]
    private int maxNumberOfLevels = 12;

//#if UNITY_EDITOR
    [SerializeField]
    private bool blasterEnabled;
    [SerializeField]
    private bool boomerangEnabled;
    [SerializeField]
    private bool missileEnabled;
    [SerializeField]
    private bool geyserEnabled;
    [SerializeField]
    private bool lightningGunEnabled;
    [SerializeField]
    private bool spreadGunEnabled;
    //#endif

    #region Event Subscriptions
    protected override LifeCycle SubscriptionLifecycle { get { return LifeCycle.StartDestroy; } }
    protected override Dictionary<string, FunctionPrototype> EventSubscriptions {
        get {
            return new Dictionary<string, FunctionPrototype>() {
                { Strings.Events.UNLOCK_WEAPON, UnlockWeapon },
                { Strings.Events.UNLOCK_NEXT_LEVEL, UnlockNextLevel},
                { Strings.Events.SET_LEVEL_SCORE, SetScoreForLevel},
            };
        }
    }
    #endregion

    #region Unity  lifecycle
    protected override void Start()
    {
        Load();
        base.Start();
    }
    #endregion

    #region public functions
    public void Load()
    {
//#if UNITY_EDITOR
        SetBool(Strings.PlayerPrefStrings.BLASTER_ENABLED, blasterEnabled);
        SetBool(Strings.PlayerPrefStrings.BOOMERANG_ENABLED, boomerangEnabled);
        SetBool(Strings.PlayerPrefStrings.MISSILE_GUN_ENABLED, missileEnabled);
        SetBool(Strings.PlayerPrefStrings.GEYSER_GUN_ENABLED, geyserEnabled);
        SetBool(Strings.PlayerPrefStrings.LIGHTNING_GUN_ENABLED, lightningGunEnabled);
        SetBool(Strings.PlayerPrefStrings.SPREAD_GUN_ENABLED, spreadGunEnabled);
//#endif
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
        Save();
    }

    public string GetString(string key, string defaultValue)
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }

    public void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        Save();
    }

    public float GetFloat(string key, float defaultValue)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    public void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        Save();
    }

    public int GetInt(string key, int defaultValue)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        Save();
    }

    public bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    public void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
        Save();
    }

    public void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }

    public void Save()
    {
        PlayerPrefs.Save();
    }

    public int GetScoreForLevel(string levelName)
    {
        levelName = levelName + "_SCORE";
        return GetInt(levelName, 0);
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
                case ModType.Missile:
                    SetBool(Strings.PlayerPrefStrings.MISSILE_GUN_ENABLED, true);
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
        if(latestLevel > maxNumberOfLevels)
        {
            latestLevel = maxNumberOfLevels;
        }
        SetInt(Strings.PlayerPrefStrings.LATEST_UNLOCKED_LEVEL, latestLevel);
    }

    private void SetScoreForLevel(params object[] parameters)
    {
        if(parameters != null && parameters.Length == 2)
        {
            string levelName = parameters[0] as string;
            if (levelName != null)
            {
                levelName = levelName + "_SCORE";
                int score = (int) parameters[1];
                SetInt(levelName, score);
            }
        }
    }
    #endregion
}
