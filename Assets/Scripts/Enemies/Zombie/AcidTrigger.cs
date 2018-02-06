using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidTrigger : MonoBehaviour {

    [SerializeField] [Range(1.0f, 5.0f)] private float acidTriggerLife;

    private Zombie zombieParent;
    private ZombieBeserker zombieBeserkerParent;
    private float currentAcidTriggerLife = 0.0f;
    

    private void Update()
    {
        currentAcidTriggerLife += Time.deltaTime;

        if (currentAcidTriggerLife > acidTriggerLife)
        {
            gameObject.SetActive(false);
            currentAcidTriggerLife = 0.0f;
        }
        
    }


    public void SetZombieParent(Zombie newZombieParent)
    {
        zombieParent = newZombieParent;
    }

    public void SetZombieParent(ZombieBeserker newZombieParent)
    {
        zombieBeserkerParent = newZombieParent;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (zombieParent)
        {
            if (other.gameObject.CompareTag(Strings.Tags.PLAYER))
            {
                zombieParent.DamageAttackTarget();
            }
        }
        else if (zombieBeserkerParent)
        {
            if (other.gameObject.CompareTag(Strings.Tags.PLAYER))
            {
                zombieBeserkerParent.DamageAttackTarget();
            }
        }

    }
}
