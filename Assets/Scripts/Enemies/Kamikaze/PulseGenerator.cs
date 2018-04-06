using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseGenerator : MonoBehaviour
{

    [SerializeField] private GameObject pulseProjectile;

    static int pulseIndex = 0;
    const int maxPulseIndex = 15;

    private int maxPulses;
    private float pulseRate;
    private float scaleRate;
    private float maxScale;
    private float damage;

    private float currentRateValue;
    private int currentPulse;
    private bool donePulsing;

    private void OnEnable()
    {
        donePulsing = false;
    }


    private void ResetGenerator()
    {
        donePulsing = false;
        currentPulse = 0;
        currentRateValue = 0.0f;
    }

    private void Update()
    {
        currentRateValue += Time.deltaTime;

        if (currentRateValue >= pulseRate && currentPulse < maxPulses)
        {
            GameObject pulse = Instantiate(pulseProjectile, 
                gameObject.transform.position + new Vector3(0.0f, 0.01f * pulseIndex + 0.01f, 0.0f), 
                Quaternion.identity);
            PulseProjectile newPulseProjectile = pulse.GetComponent<PulseProjectile>();
            newPulseProjectile.SetPulseProjectileStats(scaleRate,maxScale,damage);
            currentRateValue = 0.0f;
            currentPulse++;
            newPulseProjectile.SetRenderQueue(3000 + pulseIndex);
            pulseIndex = (pulseIndex + 1) % maxPulseIndex;
            SFXManager.Instance.Play(SFXType.KamikazePulse, transform.position);
        }
        if (currentPulse >= maxPulses)
        {
            donePulsing = true;
        }
    }

    public void SetPulseGeneratorStats(int newMaxPulses, float newPulseRate, float newScaleRate,float newMaxScale,float newDamage)
    {
        maxPulses = newMaxPulses;
        pulseRate = newPulseRate;
        scaleRate = newScaleRate;
        maxScale = newMaxScale;
        damage = newDamage;
    }

    public bool DonePulsing()
    {
        return donePulsing;
    }

    private void OnDisable()
    {
        ResetGenerator();
    }

}
