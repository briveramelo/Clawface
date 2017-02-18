// Adam Kay

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverheadTrigger : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == Strings.PLAYER)
        {
            SendMessageUpwards("OverheadTriggerEnter");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == Strings.PLAYER)
        {
            SendMessageUpwards("OverheadTriggerStay");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == Strings.PLAYER)
        {
            SendMessageUpwards("OverheadTriggerExit");
        }
    }
}

