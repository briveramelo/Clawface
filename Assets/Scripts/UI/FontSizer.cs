using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FontSizer : MonoBehaviour {

    private Text MyText {
        get {
            if(myText==null) {
                myText = GetComponent<Text>();
            }
            return myText;
        }
    }

    [SerializeField] private Text myText;
    [SerializeField] private TextStyle style;    
    
    public void Apply() {
        MyText.fontSize = UIStandards.GetFontSize(style);
    }
}

[System.Serializable] 
public class FontAdapter{
    public TextStyle textStyle;
    public int fontSize;
}

public enum TextStyle {
    Title,
    Subtitle,
    Button,
    Description,
    PopUp_Title,
    PopUp_Subtitle,
    PopUp_Button,
    PopUp_Description
}
