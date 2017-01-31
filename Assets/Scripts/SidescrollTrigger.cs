using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidescrollTrigger : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            SendMessageUpwards("SidescrollTriggerEnter");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            SendMessageUpwards("SidescrollTriggerStay");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            SendMessageUpwards("SidescrollTriggerExit");
        }
    }
}
