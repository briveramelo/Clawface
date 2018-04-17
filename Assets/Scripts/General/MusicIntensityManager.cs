﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.Audio;
using ModMan;
using UnityEngine.Events;

public class MusicIntensityManager : EventSubscriber {

    #region private fields
    private bool isChangingTrack;
    private List<AudioClip> Clips { get { return loopingAudioSet.clips; } }
    [SerializeField] AudioMixerGroup musicMixerGroup;
    #endregion

    #region serialized fields
    [SerializeField] private LoopingAudioSet loopingAudioSet;
    [SerializeField] private List<string> musicEventsList;
    [SerializeField] private float blendSpeed;
    #endregion

    #region Event Subscriptions
    protected override LifeCycle SubscriptionLifecycle { get { return LifeCycle.StartDestroy; } }
    protected override Dictionary<string, UnityAction<object[]>> EventSubscriptions {
        get {
            Dictionary<string, UnityAction<object[]>> dic = new Dictionary<string, UnityAction<object[]>>() { };
            for (int i = 0; i < musicEventsList.Count; i++) {
                dic.Add(musicEventsList[i], ChangeTrack);
            }
            return dic;
        }
    }
    #endregion
    

    #region unity lifecycle
    protected override void Awake() {
        base.Awake();
        EventSystem.Instance.TriggerEvent(Strings.Events.MUSIC_INTENSITY_STARTED);
    }
    protected override void Start () {
        base.Start();        
        loopingAudioSet.Initialize(transform, musicMixerGroup);
    }

    #endregion    

    #region public functions

    public void AddMusicTransitionEvent(string eventName, AudioClip clip)
    {
        musicEventsList.Add(eventName);
        Clips.Add(clip);
    }

    public void ClearAll()
    {
        Clips.Clear();
        musicEventsList.Clear();
    }

#if UNITY_EDITOR
    public AudioClip GetAudioClipByEventName(string eventName)
    {
        AudioClip result = null;
        int index = musicEventsList.IndexOf(eventName);
        if(index > -1 && Clips.Count > index)
        {
            result = Clips[index];
        }
        return result;
    }
#endif

    #endregion

    #region private functions
    private void ChangeTrack(params object[] parameters)
    {
        isChangingTrack = true;
        StopAllCoroutines();
        StartCoroutine(PanVolume());
    }

    private IEnumerator PanVolume() {
        isChangingTrack = loopingAudioSet.BeginMovingToNextClip();
        float currentProgress = 0f;
        while (isChangingTrack) {
            currentProgress += blendSpeed * Time.deltaTime;
            if (loopingAudioSet.UpdateActiveAndNextSourceVolumes(currentProgress)) {
                isChangingTrack = false;
            }
            yield return null;
        }
    }    
    #endregion

}

[System.Serializable]
public class LoopingAudioSet {
    [HideInInspector] public List<AudioSource> sources = new List<AudioSource>();
    public List<AudioClip> clips = new List<AudioClip>();
    [HideInInspector] public Transform parent;
    public int activeSourceIndex;
    public int ActiveSourceIndex {
        get { return Mathf.Clamp(activeSourceIndex, 0, sources.Count-1); }
        set {
            int newIndex = value;
            if (newIndex >= sources.Count) {
                newIndex = 0;
            }
            activeSourceIndex = newIndex;
        }
    }
    public int NextSourceIndex {
        get {
            int nextIndex = ActiveSourceIndex + 1;
            if (nextIndex >= sources.Count) {
                nextIndex = sources.Count - 1;
            }
            return nextIndex;
        }
    }
    public int PreviousSourceIndex{
        get {
            int previousIndex = ActiveSourceIndex - 1;
            if (previousIndex< 0) {
                previousIndex = sources.Count-1;
            }
            return previousIndex;
        }
    }

    public AudioSource PreviousSource { get { if (ActiveSourceIndex - 1 < 0) return null; return sources[PreviousSourceIndex]; } }
    public AudioSource ActiveSource { get { return sources[ActiveSourceIndex]; } }
    public AudioSource NextSource { get { if (ActiveSourceIndex + 1 >= sources.Count) return null; return sources[NextSourceIndex]; } }    

    public void Initialize(Transform parent, AudioMixerGroup mixerGroup) {
        this.parent = parent;
        sources.Clear();
        foreach (AudioClip clip in clips) {
            AudioSource newSource = parent.gameObject.AddComponent<AudioSource>();
            newSource.loop = true;
            newSource.outputAudioMixerGroup = mixerGroup;
            newSource.clip = clip;
            newSource.volume = 0f;
            newSource.Play();
            newSource.bypassReverbZones = true;
            newSource.reverbZoneMix = 0.0f;
            sources.Add(newSource);
        }

        if (sources.Count>0) {
            sources[0].volume = 1f;
        }        
    }

    public bool BeginMovingToNextClip() {
        if (PreviousSource!=null) {
            PreviousSource.volume = 0f;
        }
        bool isNextClipDifferentFromCurrent = false;
        if (NextSource!=null) {
            isNextClipDifferentFromCurrent = ActiveSource.clip != NextSource.clip;
            if (!isNextClipDifferentFromCurrent) {                
                GameObject.Destroy(NextSource);
                sources.RemoveAt(NextSourceIndex);
                clips.RemoveAt(NextSourceIndex);
            }
        }
        return isNextClipDifferentFromCurrent;
    }


    /// <summary>
    /// Returns true on completion
    /// </summary>
    /// <param name="transitionProgress"></param>
    /// <returns></returns>
    public bool UpdateActiveAndNextSourceVolumes(float transitionProgress) {
        float nextSourceVolume = Mathf.Clamp01(transitionProgress.TransformToExp());

        ActiveSource.volume = 1.0f - nextSourceVolume;
        NextSource.volume = nextSourceVolume;

        if (nextSourceVolume == 1f) {
            ActiveSourceIndex++;
            return true;
        }
        return false;
    }
}

public static class MusicHelper {
    public static float TransformToExp(this float volume) {
        return Mathf.Pow(2f, volume) - 1f;
    }
}