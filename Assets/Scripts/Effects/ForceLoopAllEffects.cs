using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Turing.VFX;

public class ForceLoopAllEffects : MonoBehaviour
{

    [SerializeField]
    float interval = 2.0f;

    VFXOneOff[] effects;

    private void Start()
    {
        effects = GetComponentsInChildren<VFXOneOff>();
        StartCoroutine(LoopCoroutine());
    }

    IEnumerator LoopCoroutine()
    {
        while (true)
        {

            foreach (VFXOneOff vfx in effects) vfx.Play();

            yield return new WaitForSeconds(interval);
        }
    }
}
