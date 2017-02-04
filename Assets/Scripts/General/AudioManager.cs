using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    List<AudioSource> audioSources;
    int index;
    AudioClip[] clips;

	// Use this for initialization
	void Start () {
        index = 0;
        audioSources = new List<AudioSource>();
        GetComponents<AudioSource>(audioSources);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool PlaySFX(SFXType type)
    {
        bool result = false;
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
        return result;
    }
}
