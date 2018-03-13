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

    #region Unity Lifecycle
    
    private void Start()
    {
        EventSystem.Instance.RegisterEvent(Strings.Events.SCENE_LOADED, PlayMusicInSceneContext);
        musicDictionary = new Dictionary<MusicType, AudioClip>();
        foreach (GameTrack t in gameTracks) {
            musicDictionary.Add(t.type, t.trackClip);
        }
        if (!SceneTracker.IsCurrentSceneMovie) {
            PlayMusicInSceneContext();
        }
    }

    private void OnDestroy()
    {
        if(EventSystem.Instance)
        {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.SCENE_LOADED, PlayMusicInSceneContext);
        }
    }

    #endregion

    #region Public Interface
   

    public void SetMusicAudioLevel(float i_newlevel) {
        i_newlevel = Mathf.Clamp(i_newlevel, 0.0f, 1.0f);
        musicMixer.SetFloat("Volume", LinearToDecibel(i_newlevel));
    }


    #endregion

    #region Private Interface

    private void PlayMusicInSceneContext(params object[] i_params)
    {
        if (SceneTracker.IsCurrentSceneMovie || SceneTracker.IsCurrentSceneMain || SceneTracker.IsCurrentSceneEditor) {
            StartCoroutine(PlayMainMenuMusic());
        }
        else if(SceneTracker.IsCurrentScenePlayerLevels) {
            StopSources();
            PlayRandomGameTrack();
        }
        else if (SceneTracker.IsCurrentScene80sShit) {
            StopSources();
        }
    }
    private IEnumerator PlayMainMenuMusic() {
        AudioClip firstToPlay = musicDictionary[MusicType.MainMenu_Intro];
        AudioClip toLoop = musicDictionary[MusicType.MainMenu_Loop];
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
