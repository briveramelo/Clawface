using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class MusicIntensityManager : RoutineRunner {

    #region private fields
    private int currentTrack;
    private bool isSource2Active;
    private bool isChangingTrack;
    #endregion

    #region serialized fields
    [SerializeField]
    private AudioSource source1;
    [SerializeField]
    private AudioSource source2;
    [SerializeField]
    private List<AudioClip> clips;
    [SerializeField]
    private List<string> musicEventsList;
    [SerializeField]
    private float blendSpeed = 0.1f;
    bool stopped;
    #endregion

    #region unity lifecycle
    // Use this for initialization
    void Start () {
		for (int i=0;i< musicEventsList.Count; i++)
        {
            EventSystem.Instance.RegisterEvent(musicEventsList[i], ChangeTrack);         
        }
        currentTrack = -1;
        isSource2Active = false;
    }

    private void OnDestroy()
    {
        if (EventSystem.Instance)
        {
            for (int i = 0; i < musicEventsList.Count; i++)
            {
                EventSystem.Instance.UnRegisterEvent(musicEventsList[i], ChangeTrack);                
            }
        }
    }
    #endregion

    #region public functions
    public void Stop(params object[] items) {
        stopped = true;
        source1.Stop();
        source2.Stop();
    }

    public void Play(params object[] items) {
        source1.Play();
        source2.Play();
    }

    public void AddMusicTransitionEvent(string eventName, AudioClip clip)
    {
        musicEventsList.Add(eventName);
        clips.Add(clip);
    }

    public void ClearAll()
    {
        clips.Clear();
        musicEventsList.Clear();
    }

    #if UNITY_EDITOR
    public AudioClip GetAudioClipByEventName(string eventName)
    {
        AudioClip result = null;
        int index = musicEventsList.IndexOf(eventName);
        if(index > -1 && clips.Count > index)
        {
            result = clips[index];
        }
        return result;
    }
#endif

    #endregion

    #region private functions
    private void ChangeTrack(params object[] parameters)
    {
        if (!stopped) {
            isChangingTrack = true;
            Timing.KillCoroutines(coroutineName);
            Timing.RunCoroutine(ChangeTrack(), coroutineName);        
        }
    }

    private IEnumerator<float> ChangeTrack()
    {
        if (!stopped) {
            currentTrack++;
            if (clips.Count > currentTrack) {
                AudioClip newClip = clips[currentTrack];
                if (isSource2Active) {
                    PlayNew(source1, newClip);
                }
                else {
                    PlayNew(source2, newClip);
                }
                while (isChangingTrack) {
                    if (isSource2Active) {
                        BlendSources(source2, source1);
                    }
                    else {
                        BlendSources(source1, source2);
                    }
                    yield return 0f;
                }
            }
        }
    }

    void PlayNew(AudioSource source, AudioClip newClip) {
        source.clip = newClip;
        source.Play();
    }

    void BlendSources(AudioSource firstSource, AudioSource secondSource) {
        float volume = firstSource.volume;
        volume -= blendSpeed;
        volume = Mathf.Clamp01(volume);
        firstSource.volume = volume;
        secondSource.volume = 1.0f - volume;
        if (volume == 0.0f) {
            firstSource.Stop();
            isChangingTrack = false;
        }
    }
    #endregion

}
