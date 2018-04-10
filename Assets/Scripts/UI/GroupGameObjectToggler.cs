using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupGameObjectToggler : MonoBehaviour {

    [SerializeField] private List<GameObjectToggler> gameObjectTogglers;

    public void ShowExclusive(GameObject selectedToggler) {
        gameObjectTogglers.ForEach(toggler => { toggler.SetState(toggler.gameObject==selectedToggler); });
    }
}
