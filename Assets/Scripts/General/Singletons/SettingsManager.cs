#region Using Statements

using System;
using UnityEngine;

#endregion

public class SettingsManager : Singleton<SettingsManager>
{
    #region Accessors (Public)

    public int QualityLevel
    {
        get
        {
            return settings.qualityLevel;
        }
        set
        {
            if (value < 0 || value > QualitySettings.names.Length)
            {
                Debug.LogWarning("Trying to set bad quality level (ignoring): " + value);
                return;
            }
            settings.qualityLevel = value;
        }
    }

    public Resolution Resolution
    {
        get
        {
            return settings.resolution;
        }
        set
        {
            settings.resolution = value;
        }
    }

    public bool FullScreen
    {
        get
        {
            return settings.fullscreen;
        }
        set
        {
            settings.fullscreen = value;
        }
    }

    public int GoreDetail
    {
        get
        {
            return settings.goreDetail;
        }
        set
        {
            if (value < 0 || value > 4)
            {
                Debug.LogWarning("Trying to set bad gore detail level (ignoring): " + value);
                return;
            }
            settings.goreDetail = value;
        }
    }

    public float MusicVolume
    {
        get
        {
            return settings.music;
        }
        set
        {
            if (value < 0 || value > 1)
            {
                Debug.LogWarning("Trying to set bad music volume level (ignoring): " + value);
                return;
            }
            settings.music = value;
        }
    }

    public float SFXVolumen
    {
        get
        {
            return settings.sfx;
        }
        set
        {
            if (value < 0 || value > 1)
            {
                Debug.LogWarning("Trying to set bad sfx volumen level (ignoring): " + value);
            }
        }
    }

    public FireMode FireMode
    {
        get
        {
            return settings.fireMode;
        }
        set
        {
            settings.fireMode = value;
        }
    }

    public bool SnapLook
    {
        get
        {
            return settings.snapLook;
        }
        set
        {
            settings.snapLook = value;
        }
    }

    #endregion

    #region Fields (Unity Serialization)

    [SerializeField]
    private Settings defaultSettings;

    #endregion

    #region Fields (Private)

    private Settings settings;

    #endregion

    #region Interface (Unity Lifecycle)

    protected override void Awake()
    {
        base.Awake();
        settings = Settings.ReadSettings();
    }

    #endregion

    #region Interface (Public)

    public void ApplyChanges()
    {
        settings.ApplySettings();
        settings.WriteSettings();
    }

    public void RevertChanges()
    {
        settings = Settings.ReadSettings();
    }

    #endregion

    #region Types (Internal)

    [Serializable]
    internal struct Settings
    {
        #region Constants (Private)

        private const string SETTINGS = "[ClawFace] (SettingsManager) Settings";

        #endregion

        #region Fields (Public)

        //// Graphics Quality Settings

        [HideInInspector]
        public int qualityLevel;
        
        [HideInInspector]
        public Resolution resolution;
        
        [HideInInspector]
        public bool fullscreen;

        [Header("Graphics")]
        [Range(0, 4)]
        public int goreDetail;

        //// Audio

        [Header ("Audio")]
        [Range(0, 1)]
        public float music;

        [Range(0, 1)]
        public float sfx;

        //// Controls

        [Header("Controls")]
        public FireMode fireMode;

        public bool snapLook;

        #endregion

        #region Interface (Public)

        public static Settings ReadSettings()
        {
            Settings toReplace = Instance.defaultSettings;
            toReplace.qualityLevel = QualitySettings.GetQualityLevel();
            toReplace.resolution = Screen.currentResolution;
            toReplace.fullscreen = Screen.fullScreen;

            string settings = PlayerPrefs.GetString(SETTINGS);
            JsonUtility.FromJsonOverwrite(settings, toReplace);
            return toReplace;
        }

        public void ApplySettings()
        {
            // We really only apply the settings that need to be applied.
            // Most others will be queried by other components in the program.
            QualitySettings.SetQualityLevel(qualityLevel);
            Screen.SetResolution(resolution.width, resolution.height, fullscreen);
            // TODO set audio settings.
        }

        public void WriteSettings()
        {
            string settings = JsonUtility.ToJson(this);
            PlayerPrefs.SetString(SETTINGS, settings);
        }

        #endregion
    }

    #endregion
}