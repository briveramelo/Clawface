using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsTrigger : MonoBehaviour {

    #region Serialized
    [SerializeField]
    private string eventName;

    #endregion

    #region Privates
    private bool hasBeenTriggered;

    #endregion

    #region Unity Methods

    // Use this for initialization
    void Start () {
        AnalyticsManager.Instance.AddLevelTrigger(eventName);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasBeenTriggered)
        {
            if (other.tag == Strings.Tags.PLAYER)
            {
                AnalyticsManager.Instance.ActivateLevelTrigger(eventName);
                hasBeenTriggered = true;
            }
        }
    }


    #endregion
}
