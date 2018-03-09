using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : Singleton<MusicManager>
{
    protected MusicManager() { }
    private Dictionary<MusicType, MusicTrack> musicDictionary;

    #region Serialized Unity Fields
    [SerializeField] private GameObject MainMenuTrack;
    [SerializeField] private AudioMixer musicMixer;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        musicDictionary = new Dictionary<MusicType, MusicTrack>() {
            {MusicType.MainMenu_Track, new MusicTrack(Instantiate(MainMenuTrack)) }
        };
        foreach (KeyValuePair<MusicType, MusicTrack> kp in musicDictionary)
        {
            kp.Value.SetParent(transform);
        }
    }
    #endregion

    #region Public Interface

    public void PlayMusic(MusicType i_type, Vector3 i_position)
    {
        if (musicDictionary.ContainsKey(i_type))
        {
            musicDictionary[i_type].Play(i_position);
            return;
        }

        string message = "No Music Track Found for " + i_type + ". Please add.";
        Debug.LogFormat("<color=#0000FF>" + message + "</color>");
    }

    public void Stop(MusicType i_type)
    {
        if (musicDictionary.ContainsKey(i_type))
        {
            musicDictionary[i_type].Stop();
            return;
        }
        string message = "No Music Track Found for " + i_type + ". Please add.";
        Debug.LogFormat("<color=#0000FF>" + message + "</color>");
    }

    public void SetMusicAudioLevel(float i_newlevel)
    {
        i_newlevel = Mathf.Clamp(i_newlevel, 0.0f, 1.0f);
        musicMixer.SetFloat("Volume", LinearToDecibel(i_newlevel));
    }
    

    #endregion

    #region Private Interface
    private float LinearToDecibel(float linear)
    {
        float dB;
        if (linear != 0)
        {
            dB = 40F * Mathf.Log10(linear);
        }
        else
        {
            dB = -80F;
        }
        return dB;
    }

    #endregion
}
