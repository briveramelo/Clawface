using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FontSizer : MonoBehaviour {

    [SerializeField] private Text text;
    [SerializeField] private TextStyle style;    

    public void TrySetTextFromTextComponent() {
        if (text==null) {
            text = GetComponent<Text>();
        }
    }
    public void Apply() {
        text.fontSize = UIStandards.GetFontSize(style);
    }
}

[System.Serializable] 
public class FontAdapter{
    public TextStyle textStyle;
    public int fontSize;
}

public enum TextStyle {
    Title,
    MapTitle,
    Button,
    Description,
    PopUp_Title,
    PopUp_Button,
    PopUp_Description
}
