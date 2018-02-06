using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseGenerator : MonoBehaviour
{

    [SerializeField] private GameObject pulseProjectile;
    [SerializeField] [Range(1, 10)] private int maxPulses;
    [SerializeField] [Range(0f, 10f)] private float pulseRate;

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
    }

    private void Update()
    {
        currentRateValue += Time.deltaTime;

        if (currentRateValue >= pulseRate && currentPulse < maxPulses)
        {
            GameObject pulse = Instantiate(pulseProjectile, gameObject.transform.position, Quaternion.identity);
            currentRateValue = 0.0f;
            currentPulse++;
        }
        if (currentPulse >= maxPulses)
        {
            donePulsing = true;
        }
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
