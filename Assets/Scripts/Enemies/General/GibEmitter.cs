using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GibEmitter : MonoBehaviour
{
    const float MINVELOCITY = 10.0f;
    const float VELOCITY = 20f;
    const float MINROTATION_VELOCITY = 13f;
    const float ROTATION_VELOCITY = 20f;

    [SerializeField] GameObject[] gibPrefabs;
    [SerializeField] float[] probabilities;
    [SerializeField] int[] gibCounts;
    [SerializeField, Range(0, 5)] int minGibs=3;

    public void Emit() {
        List<int> gibEmissionCounts = new List<int>(gibPrefabs.Length);
        int totalEmittedCount = 0;
        while (totalEmittedCount < minGibs) {
            gibEmissionCounts.Clear();
            totalEmittedCount = 0;
            for (int gibIndex = 0; gibIndex < gibPrefabs.Length; gibIndex++) {
                bool shouldCreate = Random.value <= probabilities[gibIndex];
                int count = shouldCreate ? Random.Range(0, gibCounts[gibIndex] + 1) : 0;
                gibEmissionCounts.Add(count);
                totalEmittedCount += gibEmissionCounts[gibIndex];
            }
        }

        for (int gibIndex = 0; gibIndex < gibPrefabs.Length; gibIndex++) {
            GameObject gibPrefab = gibPrefabs[gibIndex];
            int emittedCount = gibEmissionCounts[gibIndex];            
            for (int i = 0; i < emittedCount; i++) {
                GameObject gibInstance = Instantiate(gibPrefab);
                gibInstance.transform.position = transform.position + new Vector3(0.0f, 5.0f, 0.0f);
                gibInstance.transform.rotation = Quaternion.Euler(Random.insideUnitSphere * 360.0f);
                Rigidbody rb = gibInstance.GetComponentInChildren<Rigidbody>();
                rb.velocity = GetForce(.5f, MINVELOCITY, VELOCITY);
                rb.angularVelocity = GetRandom(MINROTATION_VELOCITY, ROTATION_VELOCITY);
                gibInstance.AddComponent<EmittedGib>();
            }            
        }    
    }

    private Vector3 GetForce(float percentageYComponent, float minMagnitude, float maxMagnitude) {
        Vector3 force = Random.insideUnitSphere * maxMagnitude;
        float originalMagnitude = Mathf.Clamp(force.magnitude, minMagnitude, maxMagnitude);
        Vector2 xy = new Vector2(force.x, force.z);
        float xyMag = xy.magnitude;
        force.y = Mathf.Clamp(Mathf.Abs(force.y), xyMag * 1f / percentageYComponent, Mathf.Infinity);
        force = force.normalized * originalMagnitude;
        return force;
    }

    private Vector3 GetRandom(float minMagnitude, float maxMagnitude) {
        Vector3 rand = Random.insideUnitSphere * maxMagnitude;
        float originalMagnitude = Mathf.Clamp(rand.magnitude, minMagnitude, maxMagnitude);
        rand = rand.normalized * originalMagnitude;
        return rand;
    }
}
