using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GibEmitter : MonoBehaviour
{
    [SerializeField] GameObject[] gibPrefabs;
    [SerializeField] float[] probabilities;
    [SerializeField] int[] gibCounts;
    [SerializeField] float rotationVelocity;
    [SerializeField] float velocity;

    public void Emit ()
    {
        for (int gibIndex = 0; gibIndex < gibPrefabs.Length; gibIndex++)
        {
            GameObject gibPrefab = gibPrefabs[gibIndex];
            int emittedCount = Random.Range(0, gibCounts[gibIndex] + 1);
            if (Random.value <= probabilities[gibIndex])
            {
                for (int i = 0; i < emittedCount; i++)
                {
                    GameObject gibInstance = Instantiate(gibPrefab);
                    gibInstance.transform.position = transform.position + new Vector3(0.0f, 3.0f, 0.0f);
                    gibInstance.transform.rotation = Quaternion.Euler(Random.insideUnitSphere * 360.0f);
                    Rigidbody rb = gibInstance.GetComponentInChildren<Rigidbody>();
                    rb.AddForce (Random.insideUnitSphere * velocity, ForceMode.VelocityChange);
                    rb.AddTorque (Random.insideUnitSphere * rotationVelocity, ForceMode.VelocityChange);
                    gibInstance.AddComponent<EmittedGib>();
                }
            }
        }
    }
}
