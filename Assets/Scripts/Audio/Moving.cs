﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving : MonoBehaviour
{
    public float speed = 10.0f;
//    SFXManager m_SFX;

    // Use this for initialization
    void Start ()
    {
 //       m_SFX = GetComponent<SFXManager>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(speed * Vector3.forward * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(speed * Vector3.back * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(speed * Vector3.left * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(speed * Vector3.right * Time.deltaTime);
        }

        if(Input.GetKeyDown(KeyCode.S))
        {
            SFXManager.Instance.PlayFollowObject(SFXType.TankTreads_Attack, transform);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            SFXManager.Instance.Play(SFXType.BlasterCharge, transform.position);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            SFXManager.Instance.Play(SFXType.TankTreads_Attack, transform.position);
        }
    }
}
