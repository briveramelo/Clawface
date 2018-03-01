using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IUIGroupable {
    void OnGroupSelectChanged(int selectedIndex);
}

public class ColorChangingUI : MonoBehaviour, IUIGroupable {
    [SerializeField] private Image outline;
    [SerializeField] private Sprite selectedSprite, unselectedSprite;
    private int uiIndex;
    public void SetUIIndex(int uiIndex) {
        this.uiIndex = uiIndex;
    }
    public void OnGroupSelectChanged(int selectedIndex) {
        outline.sprite = selectedIndex == uiIndex ? selectedSprite : unselectedSprite;
    }
}
