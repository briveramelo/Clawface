using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deathcam : MonoBehaviour {

    [SerializeField]
    private Transform deathCam;

    private void Start()
    {
        EventSystem.Instance.RegisterEvent(Strings.Events.PLAYER_KILLED, Unparent);
    }

    private void OnDestroy()
    {
        if (EventSystem.Instance)
        {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLAYER_KILLED, Unparent);
        }
    }

    public void Unparent(params object[] items)
    {
        deathCam.gameObject.SetActive(true);
    }
}
