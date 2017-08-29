using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockDoor : MonoBehaviour {

    private bool doorIsOpen = false;

    [SerializeField]
    private Door doorToOpen;

    private void OnTriggerEnter(Collider other)
    {

        if(other.tag == Strings.Tags.PLAYER && !doorToOpen.isOpen)
        {
            StartCoroutine(doorToOpen.openDoor());
        }
        

    }
}
