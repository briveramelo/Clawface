#region Using Statements

using System;
using UnityEngine;
using UnityEngine.Audio;

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
            if (value < 0 || value > 5)
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

    public float SFXVolume
    {
        get
        {
            return settings.sfx;
        }
        set
        {
            if (value < 0 || value > 1)
            {
                Debug.LogWarning("Trying to set bad sfx volume level (ignoring): " + value);
            }
            settings.sfx = value;
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

    public MouseAimMode MouseAimMode
    {
        get
        {
            return settings.mouseAimMode;
        }
        set
        {
            settings.mouseAimMode = value;
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

    public bool Tutorial
    {
        get
        {
            return settings.tutorial;
        }
        set
        {
            settings.tutorial = value;
        }
    }

    #endregion

    #region Fields (Unity Serialization)

    [SerializeField]
    private Settings defaultSettings;

    [SerializeField]
    private AudioMixer musicMixer;

    [SerializeField]
    private AudioMixer sfxMixer;

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

    private void Start()
    {
        settings.ApplySettings(musicMixer, sfxMixer);
    }

    #endregion

    #region Interface (Public)

    public void ApplyChanges()
    {
        settings.ApplySettings(musicMixer, sfxMixer);
        settings.WriteSettings();
    }

    public void RevertChanges()
    {
        settings = Settings.ReadSettings();
    }

    public void SetDefault()
    {
        settings = Instance.defaultSettings;
    }

    #endregion

    #region Types (Internal)

    [Serializable]
    internal class Settings
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
        [Range(0, 5)]
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

        public MouseAimMode mouseAimMode;

        public bool snapLook;

        [Header("Other")]
        public bool tutorial;

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

        public void ApplySettings(AudioMixer musicMixer, AudioMixer sfxMixer)
        {
            // We really only apply the settings that need to be applied.
            // Most others will be queried by other components in the program.
            QualitySettings.SetQualityLevel(qualityLevel);
            Screen.SetResolution(resolution.width, resolution.height, fullscreen);
            musicMixer.SetFloat("Volume", LinearToDecibel(music));
            sfxMixer.SetFloat("Volume", LinearToDecibel(sfx));
        }

        public void WriteSettings()
        {
            string settings = JsonUtility.ToJson(this);
            PlayerPrefs.SetString(SETTINGS, settings);
        }

        #endregion

        #region Interface (Private)

        // Sourced from: https://answers.unity.com/questions/283192/
        private float LinearToDecibel(float linear)
        {
            float dB;
            if (linear != 0)
            {
                dB = 40F * Mathf.Log10(linear);
            } else
            {
                dB = -80F;
            }
            return dB;
        }

        #endregion
    }

    #endregion
}