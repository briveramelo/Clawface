using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerSFXManager : MonoBehaviour
{
    public GameObject BlasterCharge_Object;
    SFX_BlasterCharge BlasterCharge = null;

    private void Start()
    {
        BlasterCharge = new SFX_BlasterCharge(BlasterCharge_Object);
    }

    public void Play_BlasterCharge()
    {
        BlasterCharge.Play();
    }
}
