using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Turing.VFX;
using UnityEngine.Events;

public class LevelLandmark : EventSubscriber
{
    [SerializeField] VFXOneOff vfx;
    [SerializeField] LineRenderer line;
    [SerializeField] Material idleMaterial;
    [SerializeField] float idleWidth = 1.0f;
    [SerializeField] Material activeMaterial;
    [SerializeField] float activeWidth = 2.0f;
    [SerializeField] float timeActive = 1.0f;
    float maxHeight = 50.0f;

    ParticleSystem[] particleSystems;

    #region Event Subscriptions
    protected override LifeCycle SubscriptionLifecycle { get { return LifeCycle.StartDestroy; } }
    protected override Dictionary<string, UnityAction<object[]>> EventSubscriptions {
        get {
            return new Dictionary<string, UnityAction<object[]>>() {
                { Strings.Events.WAVE_COMPLETE, DoShowEffects },
            };
        }
    }
    #endregion

    protected override void Start()
    {
        base.Start();
        particleSystems = vfx.GetComponentsInChildren<ParticleSystem>();
        HideVFX();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        StopAllCoroutines();
    }

    void ShowVFX ()
    {
        vfx.gameObject.SetActive(true);
        //foreach (ParticleSystem ps in particleSystems)
        //{
        //    ps.Play();
        //}
        vfx.Play();
        line.widthMultiplier = activeWidth;
        line.material = activeMaterial;

        SFXManager.Instance.Play(SFXType.LandmarkBlast, transform.position);
    }

    void HideVFX ()
    {
        //foreach (ParticleSystem ps in particleSystems)
        //{
        //    ps.Stop();
        //}
        //vfx.gameObject.SetActive(false);
        line.widthMultiplier = idleWidth;
        line.material = idleMaterial;
    }

    void DoShowEffects (params object[] parameters)
    {
        StartCoroutine (ShowEffects());
    }

    IEnumerator ShowEffects ()
    {
        ShowVFX();

        float h = 0.0f;
        float t = GetComponentInChildren<Turing.VFX.VFXOneOff>().EffectDuration;
        while (h < maxHeight)
        {
            h += Time.deltaTime / t * maxHeight;
            line.SetPosition (0, Vector3.zero);
            line.SetPosition (1, new Vector3 (0.0f, h, 0.0f));

            yield return null;
        }

        HideVFX();
    }
}
