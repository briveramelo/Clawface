//Garin

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPad : MonoBehaviour {
    PlayerMovement pm;

    //[SerializeField]
    //float cooldown = 2.0f;

    //float cooldownTimer;

    Vector3 direction;
    Vector3 pushVector;

    [SerializeField]
    float force = 5.0f;

    bool activated = false;
    void OnTriggerStay(Collider col)
    {

            pm = col.gameObject.GetComponent<PlayerMovement>();
            if (pm)
            {
                PushPlayer(pushVector,pm);

            }
            
        
    }

    //private void FixedUpdate()
    //{
    //    if (activated)
    //    {
    //        cooldownTimer -= Time.fixedDeltaTime;
    //        if(cooldownTimer <= 0f)
    //        {
    //            activated = false;
    //            cooldownTimer = cooldown;
    //        }
    //    }
    //}

    private void Awake()
    {
        direction = gameObject.transform.forward;
        pushVector = direction;
        pushVector.z += force;
        //cooldownTimer = cooldown;
    }

    void PushPlayer(Vector3 i_force, PlayerMovement pm)
    {
        //if (!activated)
        //{
        //    activated = true;

        //    pm = null;
        //}
        pm.AddExternalForce(pushVector);
    }

    
}
