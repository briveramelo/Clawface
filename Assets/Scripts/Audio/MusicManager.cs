using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : Singleton<MusicManager>
{
    protected MusicManager() { }
    private Dictionary<MusicType, MusicTrack> musicDictionary;

    #region MusicObject
    [SerializeField]
    private GameObject MainMenuTrack;
    #endregion

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
}
