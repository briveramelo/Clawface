using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitTrigger : MonoBehaviour {

    [SerializeField] private Bouncer bouncerParent;
    private bool triggerDamage = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(Strings.Tags.PLAYER) && triggerDamage)
        {
            bouncerParent.DamageAttackTarget();
        }
    }

    public void ActivateTriggerDamage()
    {
        triggerDamage = true;
    }

    public void DeactivateTriggerDamage()
    {
        triggerDamage = false;
    }
}
