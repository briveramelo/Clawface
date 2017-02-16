using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSplatter : MonoBehaviour {
    
    
    ParticleSystem ps;

    private void Awake()
    {
        //bloodSystem = bloodFountain.GetComponent<ParticleSystem>().emission;
        //bloodSystem.enabled = false;
        ps = GetComponent<ParticleSystem>();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ps.Play(false);
            //bloodSystem.enabled = true;
            Debug.Log("sploosh");
        }
    }
    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("collide");
    }


}
