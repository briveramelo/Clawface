using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IUIGroupable {
    void OnGroupSelectChanged(int selectedIndex);
}

public class ColorChangingUI : MonoBehaviour, IUIGroupable {
    [SerializeField] private Image outline;
    [SerializeField] private Color selectedColor, unselectedColor;
    private int uiIndex;
    public void SetUIIndex(int uiIndex) {
        this.uiIndex = uiIndex;
    }
    public void OnGroupSelectChanged(int selectedIndex) {
        outline.color = selectedIndex == uiIndex ? selectedColor : unselectedColor;
    }
}
