using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverheadTrigger : MonoBehaviour {
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            SendMessageUpwards("OverheadTriggerEnter");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            SendMessageUpwards("OverheadTriggerStay");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            SendMessageUpwards("OverheadTriggerExit");
        }
    }
}

