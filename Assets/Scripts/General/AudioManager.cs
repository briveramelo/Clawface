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

    public void PlaySFX(int i)
    {
        if(index == audioSources.Count)
        {
            index = 0;
        }
        audioSources[index].Stop();
        audioSources[index].clip = clips[i];
        audioSources[index].Play();
    }
}
