using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager> {

    protected AudioManager() { }

    [SerializeField] AudioClip[] clips;
    [SerializeField] List<AudioSource> audioSources;
    int index=0;

    public bool PlaySFX(SFXType type)
    {
        bool result = false;        
        if ((int)type < clips.Length)
        {
            if (index == audioSources.Count)
            {
                index = 0;
            }
            if (audioSources[index] != null)
            {
                audioSources[index].Stop();
                audioSources[index].clip = clips[(int)type];
                audioSources[index].Play();
                result = true;
            }
        }        
        return result;
    }
}
