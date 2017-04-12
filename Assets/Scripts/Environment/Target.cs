using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour, IDamageable {

    [SerializeField] GameObject explosion;

    Vector3 startPosition;
    float phaseShift;
    float period = 4f;
    float amplitude = 0.25f;

    void Awake() {
        startPosition = transform.position;
        phaseShift = Random.Range(0, 2f);
    }

    void IDamageable.TakeDamage(Damager damager) {
        Instantiate(explosion, null, true);
        //AudioManager.Instance.PlaySFX(SFXType.TargetBreak);
        Destroy(gameObject);
    }

    float IDamageable.GetHealth()
    {
        return 1;
    }

    void Update() {        
        transform.position = startPosition + Vector3.up * amplitude * Mathf.Sin(phaseShift + (Mathf.PI * 2 * Time.time / period));
    }
}
