using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleTrigger : MonoBehaviour {

    [SerializeField] private Zombie zombieParent;
    [SerializeField] private Collider childCollider;
    private bool triggerDamage = false;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(Strings.Tags.PLAYER) && triggerDamage)
        {
            zombieParent.DamageAttackTarget();
        }
    }

    public void ActivateTriggerDamage()
    {
        triggerDamage = true;
        childCollider.enabled = false;
    }

    public void DeactivateTriggerDamage()
    {
        triggerDamage = false;
        childCollider.enabled = true;
    }

}
