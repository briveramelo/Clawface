using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using ModMan;

[Serializable]
public struct GameTrack
{
    public MusicType type;
    public AudioClip trackClip;
}

public class MusicManager : Singleton<MusicManager>
{
    protected MusicManager() { }

    #region Public Fields
    public GameTrack[] gameTracks;
    #endregion

    #region Private Fields
    private Dictionary<MusicType, AudioClip> musicDictionary;
    #endregion

    #region Serialized Unity Fields
    [SerializeField] private AudioSource mainMusicSource;
    [SerializeField] private AudioSource loopMusicSource;
    #endregion

    #region Unity Lifecycle
    
    private void Start()
    {
        EventSystem.Instance.RegisterEvent(Strings.Events.SCENE_LOADED, PlayMusic);
        musicDictionary = new Dictionary<MusicType, AudioClip>();
        foreach (GameTrack t in gameTracks)
        {
            musicDictionary.Add(t.type, t.trackClip);
        }
        
    }

    private void OnDestroy()
    {
        if(EventSystem.Instance)
        {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.SCENE_LOADED, PlayMusic);
        }
    }

    #endregion

    #region Public Interface
    

    public void PlayMusic(params object[] i_params)
    {
        if (SceneTracker.IsCurrentSceneMain || SceneTracker.IsCurrentSceneMovie)
        {
            ResetSource();
            StartCoroutine(PlayMainMenuMusic());
        }        
        else if(SceneTracker.IsCurrentScenePlayerLevels)
        {
            ResetSource();
            PlayRandomGameTrack();
        }
        else if(!SceneTracker.IsCurrentSceneEditor)
        {
            mainMusicSource.Stop();
            loopMusicSource.Stop();
        }
        
    }
    
    #endregion

    #region Private Interface

    private IEnumerator PlayMainMenuMusic()
    {
        
        AudioClip firstToPlay = musicDictionary[MusicType.MainMenu_Intro];
        AudioClip toLoop = musicDictionary[MusicType.MainMenu_Loop];

        mainMusicSource.clip = firstToPlay;
        loopMusicSource.clip = toLoop;
        mainMusicSource.Play();
        while (!(mainMusicSource.time - mainMusicSource.clip.length).AboutEqual(0f, 0.01f))
        {
            yield return null;
        }
        mainMusicSource.Stop();
        loopMusicSource.Play();
    }

    private void ResetSource()
    {
        mainMusicSource.loop = false;
        mainMusicSource.Stop();
    }

    private void PlayRandomGameTrack()
    {
        AudioClip toPlay = musicDictionary[MusicType.Hathos_Lo];
        int sel = (int)UnityEngine.Random.Range(1, 3);
        switch(sel)
        {
            case 2:
                toPlay = musicDictionary[MusicType.Hathos_Med];
                break;
            case 3:
                toPlay = musicDictionary[MusicType.Hathos_Hi];
                break;
        }
        mainMusicSource.clip = toPlay;
        mainMusicSource.loop = true;
        mainMusicSource.Play();
    }


    #endregion



}
