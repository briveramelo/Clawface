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
    [SerializeField] private AudioSource oneShotMusicSource;
    [SerializeField] private AudioSource loopMusicSource;
    [SerializeField] private AudioMixer musicMixer;
    #endregion


    #region Event Subscriptions
    protected override LifeCycle SubscriptionLifecycle { get { return LifeCycle.StartDestroy; } }
    protected override Dictionary<string, FunctionPrototype> EventSubscriptions {
        get {
            return new Dictionary<string, FunctionPrototype>() {
                { Strings.Events.SCENE_LOADED, PlayMusicInSceneContext },
                { Strings.Events.MUSIC_INTENSITY_STARTED, PlayMusicInSceneContext},
            };
        }
    }
    #endregion

    #region Unity Lifecycle

    protected override void Start()
    {
        base.Start();
        musicDictionary = new Dictionary<MusicType, AudioClip>();
        foreach (GameTrack t in gameTracks) {
            musicDictionary.Add(t.type, t.trackClip);
        }
        if (!SceneTracker.IsCurrentSceneMovie) {
            PlayMusicInSceneContext();
        }
    }

    #endregion

    #region Public Interface
   

    public void SetMusicAudioLevel(float i_newlevel) {
        i_newlevel = Mathf.Clamp01(i_newlevel);
        musicMixer.SetFloat("Volume", i_newlevel.ToDecibel());
    }


    #endregion

    #region Private Interface

    private void PlayMusicInSceneContext(params object[] i_params)
    {
        if (SceneTracker.IsCurrentSceneMovie || SceneTracker.IsCurrentSceneMain) {
            AudioClip firstToPlay = musicDictionary[MusicType.MainMenu_Intro];
            AudioClip toLoop = musicDictionary[MusicType.MainMenu_Loop];
            StartCoroutine(PlayIntroToLoopMusic(firstToPlay, toLoop));
        }
        else if (SceneTracker.IsCurrentScene80sShit || SceneTracker.IsCurrentScenePlayerLevels) {
            StopSources();
        }
        else if (SceneTracker.IsCurrentSceneEditor) {
            AudioClip firstToPlay = musicDictionary[MusicType.LevelEditor_Intro];
            AudioClip toLoop = musicDictionary[MusicType.LevelEditor_Loop];
            StartCoroutine(PlayIntroToLoopMusic(firstToPlay, toLoop));
        }
    }

    private IEnumerator PlayIntroToLoopMusic(AudioClip firstToPlay, AudioClip toLoop) {
        bool isAlreadyPlayingMenuMusic = ((oneShotMusicSource.clip == firstToPlay && oneShotMusicSource.isPlaying) || (loopMusicSource.clip == toLoop && loopMusicSource.isPlaying));
        if (!isAlreadyPlayingMenuMusic) {
            oneShotMusicSource.clip = firstToPlay;
            loopMusicSource.clip = toLoop;
            oneShotMusicSource.Play();
            while (!(oneShotMusicSource.time - oneShotMusicSource.clip.length).AboutEqual(0f, 0.01f))
            {
                yield return null;
            }
            loopMusicSource.Play();
            oneShotMusicSource.Stop();
        }
    }

    private void StopSources() {
        oneShotMusicSource.Stop();
        loopMusicSource.Stop();
    }

    private void PlayRandomGameTrack() {
        AudioClip toPlay = musicDictionary[MusicType.Hathos_Lo];
        int sel = (int)UnityEngine.Random.Range(1, 4);
        switch(sel)
        {
            case 2:
                toPlay = musicDictionary[MusicType.Hathos_Med];
                break;
            case 3:
                toPlay = musicDictionary[MusicType.Hathos_Hi];
                break;
        }
        loopMusicSource.clip = toPlay;
        loopMusicSource.Play();
    }
    #endregion
}
