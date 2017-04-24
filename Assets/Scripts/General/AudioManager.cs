using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Turing.Audio;

public class AudioManager : Singleton<AudioManager> {

    protected AudioManager() { }

    //[SerializeField] AudioClip[] clips;
    [SerializeField] List<GameObject> prefabs;
    List<AudioGroup> audioGroups = new List<AudioGroup>();

    //[SerializeField] List<AudioSource> audioSources;
    int index=0;

    new private void Awake() {
        base.Awake();

        foreach (var prefab in prefabs) {
            GameObject audioObject = Instantiate(prefab);
            audioGroups.Add (audioObject.GetComponent<AudioGroup>());
            audioObject.transform.parent = transform;
        }
    }

    public bool PlaySFX(SFXType type)
    {
        bool result = false;
        if ((int)type < audioGroups.Count) {
            var audioGroup = audioGroups[(int)type];
            if (audioGroup != null) audioGroup.Play();
        }
        return result;
    }
}
