//Garin

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPad : MonoBehaviour {

    IMovable im;
    Vector3 direction;
    Vector3 pushVector;

    [SerializeField, Range(5,15)]
    float force;

    bool activated = false;
    void OnTriggerStay(Collider col)
    {
   
        im = col.gameObject.GetComponent<IMovable>();

        if (im != null)
        {
            im.AddDecayingForce(pushVector);
        }
        
    }

    

    private void Awake()
    {
        direction = gameObject.transform.forward;
        pushVector = direction;
        pushVector.z += force;
    }


    
}
