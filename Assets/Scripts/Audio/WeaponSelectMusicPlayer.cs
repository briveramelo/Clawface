using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelectMusicPlayer : MonoBehaviour {

    [SerializeField] AudioSource backgroundTrack;

    void Start() {
        EventSystem.Instance.RegisterEvent(Strings.Events.WEAPONSSELECT_FROM_STAGEOVER, Play);
        EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_STARTED, Stop); 
    }

    private void OnDestroy() {
        if (EventSystem.Instance) {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.WEAPONSSELECT_FROM_STAGEOVER, Play);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.LEVEL_STARTED, Stop);
        }
    }

    void Play(params object[] items) {
        backgroundTrack.Play();
    }

    void Stop(params object[] items) {
        backgroundTrack.Stop();
    }
}
