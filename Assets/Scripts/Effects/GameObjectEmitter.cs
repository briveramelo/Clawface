using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectEmitter : MonoBehaviour {

    [SerializeField] protected bool playOnAwake = true;
    [SerializeField] protected bool emitWithInterval = true;
    [SerializeField] protected bool playOnEnable = true;
    [SerializeField] protected bool burstOnPlay = true;
    [SerializeField] protected IntRange burstOnPlayCount;
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
    float emissionbaseSpeed=1f;
    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, emissionSphereRadius);
    }

    private void OnEnable()
    {
        if (playOnEnable) Play();
    }

    protected void Awake()
    {
        if (playOnAwake) Play();
    }

    public void Play ()
    {
        playing = true;
        if (burstOnPlay) {
            int randomNum = burstOnPlayCount.GetRandomValue();
            for (int i = 0; i < randomNum; i++)
            {
                Emit();
            }
        }
        timer = spawnInterval;
    }

    protected void Update ()
    {
        if (emitWithInterval && playing)
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
        
        Vector3 force = Random.insideUnitSphere * Random.Range(forceMin, forceMax)  + Vector3.forward*emissionbaseSpeed;
        Rigidbody rigbod = instance.GetComponent<Rigidbody>();
        rigbod.velocity = force;
        Vector3 torque = Random.insideUnitSphere * Random.Range(rotationalVelocityMin, rotationalVelocityMax);
        rigbod.AddTorque(torque, ForceMode.VelocityChange);

        instance.gameObject.SetActive(true);

        return instance;
    }

    public void SetBaseEmissionSpeed(float baseSpeed)
    {
        emissionbaseSpeed = baseSpeed;
    }
}
