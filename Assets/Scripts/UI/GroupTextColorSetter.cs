using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupTextColorSetter : MonoBehaviour {

    [SerializeField] private List<TextToggle> textToggles;
    [SerializeField] private Color selectedColor, unSelectedColor;

    private void OnEnable() {
        SetColor();
    }

    public void SetColor() {
        textToggles.ForEach(textToggle => textToggle.text.color = unSelectedColor);
        textToggles.Find(textToggle => textToggle.toggle.isOn).text.color = selectedColor;
    }
}

[System.Serializable]
class TextToggle {    
    public Toggle toggle;
    public Text text;
}
