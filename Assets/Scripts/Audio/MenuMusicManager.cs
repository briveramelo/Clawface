using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using System.Linq;
using ModMan;
public class MenuMusicManager : Singleton<MenuMusicManager> {

    [SerializeField] AudioSource intro, looping;
    static bool isPlaying;

    void Start() {
        EventSystem.Instance.RegisterEvent(Strings.Events.WEAPONS_SELECT_FROM_STAGE_OVER, Play);
        EventSystem.Instance.RegisterEvent(Strings.Events.SCENE_LOADED, CheckSceneToPlay);
        if (SceneTracker.IsCurrentSceneMain || SceneTracker.IsCurrentSceneEditor) {
            Play();
        }
    }

    public void OnDestroy() {
        if (EventSystem.Instance) {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.WEAPONS_SELECT_FROM_STAGE_OVER, Play);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.SCENE_LOADED, CheckSceneToPlay);
        }
    }

    void CheckSceneToPlay(params object[] items) {
        if (SceneTracker.IsCurrentSceneMain) {
            Play();
        }
        else if (SceneTracker.IsCurrentScene80sShit || SceneTracker.IsCurrentScenePlayerLevels) {
            Stop();
        }
    }

    void Play(params object[] items) {
        if (!isPlaying) {
            isPlaying = true;
            StartCoroutine(GoToLoop());
        }
    }

    void Stop(params object[] items) {
        isPlaying = false;
        intro.Stop();
        looping.Stop();
    }

    IEnumerator GoToLoop() {        
        intro.Play();
        while (!(intro.time - intro.clip.length).AboutEqual(0f, 0.01f)) {
            yield return null;
        }
        intro.Stop();
        looping.Play();
    }
}
