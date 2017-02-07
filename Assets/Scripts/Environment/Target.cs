using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour, IDamageable {

    [SerializeField] GameObject explosion;

    void IDamageable.TakeDamage(float damage) {
        Instantiate(explosion, null, false);
        AudioManager.instance.PlaySFX(SFXType.TargetBreak);
        Destroy(gameObject);
    }
}
