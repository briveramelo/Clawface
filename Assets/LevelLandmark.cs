using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLandmark : MonoBehaviour
{
    [SerializeField] GameObject vfx;
    ParticleSystem[] particleSystems;

    private void Start()
    {
        if (Application.isPlaying)
        {
            EventSystem.Instance.RegisterEvent(Strings.Events.CALL_NEXT_WAVE, HideVFX);
            EventSystem.Instance.RegisterEvent(Strings.Events.WAVE_COMPLETE, ShowVFX);
        }
        particleSystems = vfx.GetComponentsInChildren<ParticleSystem>();
        HideVFX();
    }

    private void OnDestroy()
    {
        if (Application.isPlaying)
        {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.WAVE_COMPLETE, ShowVFX);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.CALL_NEXT_WAVE, HideVFX);
        }
    }

    void ShowVFX (params object[] parameters)
    {
        vfx.SetActive(true);
        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Play();
        }
    }

    void HideVFX (params object[] paramters)
    {
        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Stop();
        }
        vfx.SetActive(false);
    }
}
