using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SelectorToggle : MonoBehaviour {

    #region Accessors (Public)

    public bool Selected
    {
        set
        {
            selected = value;
            UpdateDisplay();
        }
    }

    public bool Active
    {
        set
        {
            active = value;
            UpdateDisplay();
        }
    }

    #endregion

    #region Fields (Unity Serialization)

    [Header("Font Colors")]
    [SerializeField]
    private Color colorOff = new Color(247F / 255, 249F / 255, 118F / 255);

    [SerializeField]
    private Color colorOn = new Color(231F / 255, 0F, 205F / 255);

    [Header("Sprite Backings")]
    [SerializeField]
    private Sprite spriteOff;

    [SerializeField]
    private Sprite spriteOn;

    [Header("References")]
    [SerializeField]
    private Button button;

    [SerializeField]
    private Text text;

    #endregion

    #region Fields (Private)

    private bool selected = false;
    private bool active = false;

    #endregion

    #region Interface (Unity Lifecycle)

    private void Awake()
    {
        Assert.IsNotNull(button);
        Assert.IsNotNull(text);
    }

    #endregion

    #region Interface (Private)

    private void UpdateDisplay()
    {
        // Font colors
        text.color = selected || active ? colorOn : colorOff;

        // Sprite backing
        button.image.sprite = active ? spriteOn : spriteOff;
    }

    #endregion
}
