using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;
public class WeaponSelectMusicPlayer : RoutineRunner {

    [SerializeField] AudioSource intro, looping;
    [SerializeField] bool playOnStart;    

    void Start() {
        EventSystem.Instance.RegisterEvent(Strings.Events.WEAPONSSELECT_FROM_STAGEOVER, Play);
        EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_STARTED, Stop);
        EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_RESTARTED, Stop);
        if (playOnStart) {
            Play();
        }
    }

    private void OnDestroy() {
        if (EventSystem.Instance) {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.WEAPONSSELECT_FROM_STAGEOVER, Play);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.LEVEL_STARTED, Stop);
            EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_RESTARTED, Stop);
        }
    }

    void Play(params object[] items) {
        Stop();
        Timing.RunCoroutine(GoToLoop(), coroutineName);
    }

    void Stop(params object[] items) {
        intro.Stop();
        looping.Stop();
    }

    IEnumerator<float> GoToLoop() {        
        intro.Play();
        while (intro.isPlaying || intro.time < intro.clip.length-.1f) {
            yield return 0f;
        }
        looping.Play();
    }
}
