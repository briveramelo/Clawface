using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectEmitter : MonoBehaviour {

    [SerializeField] protected bool playOnAwake = true;
    [SerializeField] protected GameObject spawnPrefab;
    [SerializeField] protected float spawnInterval = 0.5f;
    [SerializeField] protected float forceMin = 1.0f;
    [SerializeField] protected float forceMax = 2.0f;
    [SerializeField] protected float emissionSphereRadius = 1.0f;
    [SerializeField] protected float rotationalVelocityMin = 1.0f;
    [SerializeField] protected float rotationalVelocityMax = 2.0f;
    [SerializeField] protected float duration = 2.0f;
    [SerializeField] protected AnimationCurve scaleCurve;

    bool playing = false;
    float timer = 0.0f;

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, emissionSphereRadius);
    }

    protected void Awake()
    {
        if (playOnAwake) Play();
    }

    protected void Play ()
    {
        playing = true;
        timer = spawnInterval;
    }

    protected void Update ()
    {
        if (playing)
        {
            timer -= Time.deltaTime;
            if (timer <= 0.0f)
            {
                Emit();
                timer = spawnInterval;
            }
        }
	}

    protected virtual GameObject Emit ()
    {
        GameObject instance = Instantiate(spawnPrefab);
        instance.transform.position = transform.position + Random.insideUnitSphere * emissionSphereRadius;
        instance.transform.rotation = Quaternion.Euler(Random.insideUnitSphere * 360.0f);

        EmittedGameObject emitted = instance.GetComponent<EmittedGameObject>();
        emitted.Init(duration, scaleCurve);

        Rigidbody instanceRB = instance.GetComponent<Rigidbody>();
        Vector3 force = Random.insideUnitSphere * Random.Range(forceMin, forceMax);
        
        instanceRB.velocity = force;
        Vector3 torque = Random.insideUnitSphere * Random.Range(rotationalVelocityMin, rotationalVelocityMax);
        instanceRB.AddTorque(torque, ForceMode.VelocityChange);

        instance.gameObject.SetActive(true);

        return instance;
    }
}
