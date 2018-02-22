using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleTrigger : MonoBehaviour {

    [SerializeField] private Zombie zombieParent;
    [SerializeField] private ZombieBeserker zombieBeserkerParent;
    [SerializeField] private ZombieAcider zombieAciderParent;
    [SerializeField] private Collider childCollider;
    private bool triggerDamage = false;


    private void OnTriggerEnter(Collider other)
    {

        if (zombieParent)
        {
            if (other.gameObject.CompareTag(Strings.Tags.PLAYER) && triggerDamage)
            {
                zombieParent.DamageAttackTarget();
            }
        }
        else if (zombieBeserkerParent)
        {
            if (other.gameObject.CompareTag(Strings.Tags.PLAYER) && triggerDamage)
            {
                zombieBeserkerParent.DamageAttackTarget();
            }
        }
        else if (zombieAciderParent)
        {
            if (other.gameObject.CompareTag(Strings.Tags.PLAYER) && triggerDamage)
            {
                zombieAciderParent.DamageAttackTarget();
            }
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
