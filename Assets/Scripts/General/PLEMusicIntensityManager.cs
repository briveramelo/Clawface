using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.Audio;
using ModMan;
public class PLEMusicIntensityManager : EventSubscriber {

    #region private fields
    private bool isChangingTrack;
    private List<AudioClip> Clips { get { return loopingAudioSet.clips; } }
    [SerializeField] AudioMixerGroup musicMixerGroup;
    #endregion

    #region serialized fields
    [SerializeField] private AudioClip lo, med, hi;
    [SerializeField] private LoopingAudioSet loopingAudioSet;
    [SerializeField] private float blendSpeed;
    #endregion

    #region Event Subscriptions
    protected override LifeCycle SubscriptionLifecycle { get { return LifeCycle.StartDestroy; } }
    protected override Dictionary<string, FunctionPrototype> EventSubscriptions {
        get {
            return new Dictionary<string, FunctionPrototype>() {
                { Strings.Events.PLE_ON_LEVEL_READY, InitializeAudio},
                { Strings.Events.WAVE_COMPLETE, ChangeTrack },
            };
        }
    }
    #endregion


    #region unity lifecycle
    #endregion    

    #region public functions

    #endregion

    #region private functions
    private bool isInitialized;
    private void InitializeAudio(params object[] parameters) {
        if (!isInitialized) {
            isInitialized = true;
            InitializeAudioClips();
            loopingAudioSet.Initialize(transform, musicMixerGroup);
            EventSystem.Instance.TriggerEvent(Strings.Events.MUSIC_INTENSITY_STARTED);
        }
    }
    private void InitializeAudioClips() {
        int maxWaveCount = PLESpawnManager.Instance.MaxWaveIndex + 1;
        int numHigh = Mathf.FloorToInt((maxWaveCount+2) /3f);
        int numMed = Mathf.FloorToInt((maxWaveCount+1) / 3f);
        int numLow = Mathf.FloorToInt((maxWaveCount) / 3f);

        Clips.Clear();
        Clips.Add(numLow, lo);
        Clips.Add(numMed, med);
        Clips.Add(numHigh, hi);
        if (true) { }
    }    
    private void ChangeTrack(params object[] parameters) {
        int infiniteWaveIndex = (int)parameters[0];
        if (infiniteWaveIndex != 0) {
            isChangingTrack = true;
            StopAllCoroutines();
            StartCoroutine(PanVolume());
        }
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
