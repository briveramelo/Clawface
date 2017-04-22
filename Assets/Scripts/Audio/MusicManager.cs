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
        //musicDictionary = new Dictionary<MusicType, MusicTrack>()
        //{
        //   {MusicType.MainMenu_Track, new SoundEffect(Instantiate(MainMenuTrack)) }
        //};
    }
}
