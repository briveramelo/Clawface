using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DisableOnPlayerDeath : MonoBehaviour
{
    private void Awake()
    {
        EventSystem.Instance.RegisterEvent(Strings.Events.PLAYER_KILLED, HandlePlayerDeath);
    }

    void HandlePlayerDeath (params object[] parameters)
    {
        gameObject.SetActive(false);
    }
}
