using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicIntensityManager : MonoBehaviour {

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
        isChangingTrack = true;
        StartCoroutine(ChangeTrack());
    }

    private IEnumerator ChangeTrack()
    {
        currentTrack++;
        if (clips.Count > currentTrack)
        {
            AudioClip newClip = clips[currentTrack];
            if (isSource2Active)
            {
                source1.clip = newClip;
                source1.Play();
            }
            else
            {
                source2.clip = newClip;
                source2.Play();
            }
            while (isChangingTrack)
            {
                if (isSource2Active)
                {
                    float volume = source2.volume;
                    volume -= blendSpeed;
                    volume = Mathf.Clamp(volume, 0.0f, 1.0f);
                    source2.volume = volume;
                    source1.volume = 1.0f - volume;
                    if (volume == 0.0f)
                    {
                        source2.Stop();
                        isChangingTrack = false;
                    }
                }
                else
                {
                    float volume = source1.volume;
                    volume -= blendSpeed;
                    volume = Mathf.Clamp(volume, 0.0f, 1.0f);
                    source1.volume = volume;
                    source2.volume = 1.0f - volume;
                    if (volume == 0.0f)
                    {
                        source1.Stop();
                        isChangingTrack = false;
                    }
                }
                yield return null;
            }
        }
    }
    #endregion

}
