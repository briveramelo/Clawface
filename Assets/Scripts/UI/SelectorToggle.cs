using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class SelectorToggle : MonoBehaviour {

    #region Fields (Unity Serialization)

    [Header("Font Colors")]
    [SerializeField]
    private Color colorOff = new Color(247F / 255, 249F / 255, 118F / 255);

    [SerializeField]
    private Color colorOn = new Color(231F / 255, 0F, 205F / 255);

    [SerializeField]
    private Color colorDisabled = new Color(0.1F, 0.1F, 0.1F);

    [Header("Sprite Backings")]
    [SerializeField]
    private Sprite spriteOff;

    [SerializeField]
    private Sprite spriteOn;

    [Header("References")]
    [SerializeField]
    private Selectable selectable;

    [SerializeField]
    private Text text;

    #endregion

    #region Fields (Private)

    private bool selected;

    private bool hovered;

    private bool interactable;

    #endregion

    #region Interface (Unity Lifecycle)

    private void Awake()
    {
        Assert.IsNotNull(selectable);
        Assert.IsNotNull(text);

        // Events
        EventTrigger.Entry select = new EventTrigger.Entry();
        select.eventID = EventTriggerType.Select;
        select.callback.AddListener(ButtonOnSelect);

        EventTrigger.Entry deselect = new EventTrigger.Entry();
        deselect.eventID = EventTriggerType.Deselect;
        deselect.callback.AddListener(ButtonOnDeselect);

        EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener(ButtonOnPointerEnter);

        EventTrigger.Entry pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener(ButtonOnPointerExit);

        EventTrigger trigger = selectable.gameObject.AddComponent<EventTrigger>();
        trigger.hideFlags = HideFlags.HideInInspector;
        trigger.triggers.AddRange(new EventTrigger.Entry[]
        {
            select,
            deselect,
            pointerEnter,
            pointerExit
        });

        interactable = selectable.interactable;
    }

    private void Update()
    {
        if (interactable != selectable.interactable)
        {
            interactable = selectable.interactable;
            UpdateDisplay();
        }
    }
    #endregion

    #region Interface (Private)

    private void ButtonOnSelect(BaseEventData data)
    {
        selected = true;
        UpdateDisplay();
    }

    private void ButtonOnDeselect(BaseEventData data)
    {
        selected = false;
        UpdateDisplay();
    }

    private void ButtonOnPointerEnter(BaseEventData data)
    {
        hovered = true;
        UpdateDisplay();
    }

    private void ButtonOnPointerExit(BaseEventData data)
    {
        hovered = false;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        Sprite sprite;
        Color color;
        if (interactable)
        {
            sprite = (selected || hovered) ? spriteOn : spriteOff;
            color = (selected || hovered) ? colorOn : colorOff;
        } else
        {
            sprite = spriteOff;
            color = colorDisabled;
        }

        // Set values.
        selectable.image.sprite = sprite;
        text.color = color;
    }

    #endregion
}
