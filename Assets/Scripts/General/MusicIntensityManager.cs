using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.Audio;

public class MusicIntensityManager : MonoBehaviour {

    #region private fields
    private int currentTrack;
    private bool isChangingTrack;
    private List<AudioClip> Clips { get { return loopingAudioSet.clips; } }
    [SerializeField] AudioMixerGroup musicMixerGroup;
    private bool stopped;
    #endregion

    #region serialized fields
    [SerializeField] private LoopingAudioSet loopingAudioSet;
    [SerializeField] private List<string> musicEventsList;
    [SerializeField] private float blendSpeed;

    public bool removeMe;
    #endregion

    #region unity lifecycle
    // Use this for initialization
    void Start () {
		for (int i = 0; i< musicEventsList.Count; i++)
        {
            //EventSystem.Instance.RegisterEvent(musicEventsList[i], ChangeTrack);         
        }
        loopingAudioSet.Initialize(transform, musicMixerGroup);
        currentTrack = -1;
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

    private void Update() {
        if (removeMe) {
            removeMe = false;
            ChangeTrack();
        }
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
        if (!stopped) {
            isChangingTrack = true;
            StopAllCoroutines();
            StartCoroutine(PanVolume());
        }
    }

    private IEnumerator PanVolume()
    {
        if (!stopped) {
            currentTrack++;
            if (loopingAudioSet.clips.Count > currentTrack) {

                loopingAudioSet.MoveToNextClip();

                while (isChangingTrack) {
                    if (loopingAudioSet.UpdateBlendSources(blendSpeed)) {
                        isChangingTrack = false;
                    }
                    yield return null;
                }
            }
        }
    }    
    #endregion

}

[System.Serializable]
public class LoopingAudioSet {
    [HideInInspector] public List<AudioSource> sources = new List<AudioSource>();
    public List<AudioClip> clips = new List<AudioClip>();
    [HideInInspector] public Transform parent;
    [HideInInspector] public int activeSourceIndex;
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
            sources.Add(newSource);
        }

        if (sources.Count>0) {
            sources[0].volume = 1f;
        }
    }

    public void MoveToNextClip() {
        if (PreviousSource!=null) {
            PreviousSource.volume = 0f;
        }
        ActiveSourceIndex++;
    }


    /// <summary>
    /// Returns true on completion
    /// </summary>
    /// <param name="blendSpeed"></param>
    /// <returns></returns>
    public bool UpdateBlendSources(float blendSpeed) {
        float activeSourceVolume = ActiveSource.volume;
        activeSourceVolume += blendSpeed * Time.deltaTime;
        activeSourceVolume = Mathf.Clamp01(activeSourceVolume);

        if (PreviousSource!=null) {
            PreviousSource.volume = 1.0f - activeSourceVolume;
        }
        ActiveSource.volume = activeSourceVolume;

        if (activeSourceVolume == 1f) {
            return true;
        }
        return false;
    }
}