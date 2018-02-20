using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLandmark : MonoBehaviour
{
    [SerializeField] GameObject vfx;
    ParticleSystem[] particleSystems;

    private void Start()
    {
        EventSystem.Instance.RegisterEvent(Strings.Events.ENEMY_SPAWNED, DoShowEffects);
        particleSystems = vfx.GetComponentsInChildren<ParticleSystem>();
        HideVFX();
    }

    private void OnDestroy()
    {
        EventSystem.Instance.UnRegisterEvent(Strings.Events.ENEMY_SPAWNED, DoShowEffects);
        StopAllCoroutines();
    }

    void ShowVFX ()
    {
        vfx.SetActive(true);
        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Play();
        }
    }

    void HideVFX ()
    {
        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Stop();
        }
        vfx.SetActive(false);
    }

    void DoShowEffects (params object[] parameters)
    {
        StartCoroutine (ShowEffects());
    }

    IEnumerator ShowEffects ()
    {
        ShowVFX();

        yield return new WaitForSecondsRealtime(3.0f);

        HideVFX();
    }
}
