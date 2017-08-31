using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour {

    [SerializeField]
    private int sequenceToFire = 0;

    [SerializeField]
    private TutorialMenu tutorialCanvas;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.gameObject.tag == Strings.Tags.PLAYER)
        {
            triggered = true;
            tutorialCanvas.ShowImages(sequenceToFire);
        }
    }

}
