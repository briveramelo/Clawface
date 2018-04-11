#region Using Statements

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            return settings.Resolution;
        }
        set
        {
            settings.Resolution = value;
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

    public bool Vibration
    {
        get
        {
            return settings.vibration;
        }
        set
        {
            settings.vibration = value;
        }
    }

    public bool CursorLock
    {
        get
        {
            return settings.cursorLock;
        }
        set
        {
            settings.cursorLock = value;
        }
    }

    public Difficulty Difficulty
    {
        get
        {
            return settings.difficulty;
        }
        set
        {
            settings.difficulty = value;
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

    public bool TutorialEncounteredInGameplay
    {
        get
        {
            return PlayerPrefs.GetInt(Strings.PlayerPrefStrings.TUTORIAL_ENCOUNTERED_IN_GAMEPLAY, 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(Strings.PlayerPrefStrings.TUTORIAL_ENCOUNTERED_IN_GAMEPLAY, value ? 1 : 0);
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

    protected override void Start()
    {
        base.Start();
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
        settings = Settings.GetDefaults();
    }

    #endregion

    #region Types (Internal)

    [Serializable]
    internal class Settings
    {
        #region Constants (Private)

        private const string SETTINGS = "[ClawFace] (SettingsManager) Settings";

        #endregion

        #region Accessors (Public)

        public Resolution Resolution
        {
            set
            {
                resolutionData = new ResolutionData(value.width, value.height);
            }
            get
            {
                List<Resolution> resolutions = (from resolution in Screen.resolutions
                                           where resolution.width == resolutionData.width
                                           where resolution.height == resolutionData.height
                                           orderby resolution.refreshRate descending
                                           select resolution).ToList();
                if (resolutions.Count==0) {
                    return new Resolution();
                }                
                return resolutions[0];
            }
        }

        #endregion

        #region Fields (Public)

        //// Graphics Quality Settings

        [HideInInspector]
        public int qualityLevel = QualitySettings.GetQualityLevel();
        
        [HideInInspector]
        public bool fullscreen = Screen.fullScreen;

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

        public bool vibration;

        public bool cursorLock;

        [Header("GamePlay")]
        public Difficulty difficulty;

        public bool tutorial;

        #endregion

        #region Fields (Private)
        
        [HideInInspector]
        [SerializeField]
        private ResolutionData resolutionData;

        private static int defaultQualityLevel;

        private static Resolution defaultResolution;

        private static bool defaultFullscreen;

        #endregion

        #region Constructors (Public)

        static Settings()
        {
            defaultQualityLevel = QualitySettings.GetQualityLevel();
            defaultResolution = Screen.currentResolution;
            defaultFullscreen = Screen.fullScreen;
        }

        public Settings(Settings other = null)
        {
            if (other != null)
            {
                qualityLevel = other.qualityLevel;
                resolutionData = other.resolutionData;
                fullscreen = other.fullscreen;
                goreDetail = other.goreDetail;

                music = other.music;
                sfx = other.sfx;

                fireMode = other.fireMode;
                mouseAimMode = other.mouseAimMode;
                snapLook = other.snapLook;
                vibration = other.vibration;
                cursorLock = other.cursorLock;

                difficulty = other.difficulty;
                tutorial = other.tutorial;
            }
        }

        #endregion

        #region Interface (Public)

        public static Settings GetDefaults()
        {
            Settings defaults = new Settings(Instance.defaultSettings);
            defaults.qualityLevel = defaultQualityLevel;
            defaults.Resolution = defaultResolution;
            defaults.fullscreen = defaultFullscreen;
            return defaults;
        }

        public static Settings ReadSettings()
        {
            Settings toReplace = GetDefaults();
            string settings = PlayerPrefs.GetString(SETTINGS);
            JsonUtility.FromJsonOverwrite(settings, toReplace);
            return toReplace;
        }

        public void ApplySettings(AudioMixer musicMixer, AudioMixer sfxMixer)
        {
            // We really only apply the settings that need to be applied.
            // Most others will be queried by other components in the program.
            QualitySettings.SetQualityLevel(qualityLevel);
            Screen.SetResolution(resolutionData.width, resolutionData.height, fullscreen);
            MusicManager.Instance.SetMusicAudioLevel(music);
            SFXManager.Instance.SetSFXAudioLevel(sfx);
            Instance.StartCoroutine(DoCursorLockStateChange());
        }

        public void WriteSettings()
        {
            string settings = JsonUtility.ToJson(this);
            PlayerPrefs.SetString(SETTINGS, settings);
        }

        #endregion

        #region Interface (Private)

        private IEnumerator DoCursorLockStateChange()
        {
            // This fix is to handle issues where changing the screen aspect cause the lock state to get stuck
            Cursor.lockState = CursorLockMode.None;

            // skip two frames to make absolutely sure there was enough time to update
            yield return null;
            yield return null; 

            Cursor.lockState = cursorLock ? CursorLockMode.Confined : CursorLockMode.None;
        }

        #endregion

        #region Types (Internal)

        [Serializable]
        internal struct ResolutionData
        {
            #region Fields (Public)

            public int width;

            public int height;

            #endregion

            #region Constructors (Public)

            public ResolutionData(int width, int height)
            {
                this.width = width;
                this.height = height;
            }

            #endregion
        }

        #endregion
    }

    #endregion
}