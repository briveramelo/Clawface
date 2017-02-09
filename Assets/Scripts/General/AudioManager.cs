using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public static AudioManager instance;

    [SerializeField] AudioClip[] clips;
    List<AudioSource> audioSources;
    int index;

	// Use this for initialization
	void Start () {
        instance = this;
        index = 0;
        audioSources = new List<AudioSource>();
        GetComponents<AudioSource>(audioSources);
	}

    public bool PlaySFX(SFXType type)
    {
        bool result = false;
        if ((int)type < clips.Length) {
            if ((int)type < clips.Length)
            {
                if (index == audioSources.Count)
                {
                    index = 0;
                }
                audioSources[index].Stop();
                audioSources[index].clip = clips[(int)type];
                audioSources[index].Play();
                result = true;
            }
        }
        return result;
    }
}
