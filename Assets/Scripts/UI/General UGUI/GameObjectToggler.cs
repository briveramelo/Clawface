using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectToggler : MonoBehaviour, IUIGroupable {

    private int uiIndex;

    public void OnGroupSelectChanged(int selectedIndex) {
        gameObject.SetActive(uiIndex == selectedIndex);
    }
    public void SetState(bool isOn) {
        gameObject.SetActive(isOn);
    }

    public void SetUIIndex(int uiIndex) {
        this.uiIndex = uiIndex;
    }
}
