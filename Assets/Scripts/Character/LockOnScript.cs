using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {        
        if(Input.GetAxis(Strings.RIGHTTRIGGER) < -0.5f){
            AcquireTarget();
        }
	}

    void AcquireTarget()
    {
        RaycastHit[] hits;
        Ray sphereRay = new Ray(transform.position, transform.forward);
        LayerMask mask = LayerMask.GetMask("Enemy");
        hits = Physics.SphereCastAll(sphereRay, 5f, 0f, mask);        
        foreach(RaycastHit hit in hits)
        {
            if (hit.transform.gameObject.tag == Strings.ENEMY)
            {
                print(hit.transform.gameObject.name);
            }
        }
    }
}
