using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;
using System.Linq;
using ModMan;
public class MenuMusicManager : RoutineRunner {

    [SerializeField] AudioSource intro, looping;
    [SerializeField] bool playOnStart;
    static bool isPlaying;

    void Start() {
        EventSystem.Instance.RegisterEvent(Strings.Events.WEAPONSSELECT_FROM_STAGEOVER, Play);
        EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_STARTED, Stop);
        EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_RESTARTED, Stop);
        if (playOnStart) {            
            Play();
        }
    }

    void OnDestroy() {
        if (EventSystem.Instance) {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.WEAPONSSELECT_FROM_STAGEOVER, Play);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.LEVEL_STARTED, Stop);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.LEVEL_RESTARTED, Stop);
        }        
    }

    void Play(params object[] items) {
        Stop();
        if (!isPlaying) {
            isPlaying = true;
            StartCoroutine(GoToLoop());
        }
    }

    void Stop(params object[] items) {
        if (intro.isPlaying || looping.isPlaying && intro && looping) {            
            isPlaying = false;
            intro.Stop();
            looping.Stop();
        }
    }

    IEnumerator GoToLoop() {        
        intro.Play();
        while (!(intro.time - intro.clip.length).AboutEqual(0f, 0.01f)) {
            yield return null;
        }
        looping.Play();
    }
}
