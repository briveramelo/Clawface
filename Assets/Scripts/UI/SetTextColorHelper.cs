using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using PlayerLevelEditor;

public class SetTextColorHelper : MonoBehaviour
{

    #region Serialized Unity Fields

    [SerializeField] private Text floorText;
    [SerializeField] private Text propsText;
    [SerializeField] private Text spawnsText;
    [SerializeField] private Text wavesText;
    [SerializeField] private Text testText;
    [SerializeField] private Text saveText;
    [SerializeField] private Text loadText;
    [SerializeField] private Text helpText;
    [SerializeField] private Text exitText;

    [SerializeField] private LevelEditor editorInstance;

    [SerializeField] private Color selectedColor;
    [SerializeField] private Color defaultColor;
     
    #endregion

    #region Private Fields

    private Text myTex;

    #endregion

    public void SetTextColor()
    {
        floorText.color = defaultColor;
        propsText.color = defaultColor;
        spawnsText.color = defaultColor;
        wavesText.color = defaultColor;
        testText.color = defaultColor;
        saveText.color = defaultColor;
        loadText.color = defaultColor;
        helpText.color = defaultColor;
        exitText.color = defaultColor;

        switch (editorInstance.currentDisplayedMenu)
        {

            case PLEMenu.FLOOR:
                floorText.color = selectedColor;
                break;
            case PLEMenu.PROPS:
                propsText.color = selectedColor;
                break;
            case PLEMenu.SPAWN:
                spawnsText.color = selectedColor;
                break;
            case PLEMenu.WAVE:
                wavesText.color = selectedColor;
                break;
            case PLEMenu.TEST:
                testText.color = selectedColor;
                break;
            case PLEMenu.SAVE:
                saveText.color = selectedColor;
                break;
            case PLEMenu.LEVELSELECT:
                loadText.color = selectedColor;
                break;
            case PLEMenu.HELP:
                helpText.color = selectedColor;
                break;
            case PLEMenu.NONE:
                floorText.color = selectedColor;
                break;
        }

    }
}
