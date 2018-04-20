using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GibEmitter : MonoBehaviour
{
    const float ROTATION_VELOCITY = 360.0f;
    const float VELOCITY = 10.0f;

    [SerializeField] GameObject[] gibPrefabs;
    [SerializeField] float[] probabilities;
    [SerializeField] int[] gibCounts;

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
                    gibInstance.transform.position = transform.position + new Vector3(0.0f, 5.0f, 0.0f);
                    gibInstance.transform.rotation = Quaternion.Euler(Random.insideUnitSphere * 360.0f);
                    Rigidbody rb = gibInstance.GetComponentInChildren<Rigidbody>();
                    rb.AddForce (Random.insideUnitSphere * VELOCITY, ForceMode.VelocityChange);
                    rb.AddTorque (Random.insideUnitSphere * ROTATION_VELOCITY, ForceMode.VelocityChange);
                    gibInstance.AddComponent<EmittedGib>();
                }
            }
        }
    }
}
