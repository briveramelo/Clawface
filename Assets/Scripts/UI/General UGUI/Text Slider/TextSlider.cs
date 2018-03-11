using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextSlider : Selectable {

    #region Accessors (Public)

    public TextSliderDataSource DataSource
    {
        get
        {
            return dataSource;
        }
    }

    #endregion

    #region Fields (Unity Serialization)

    [Header("Text Slider Data Source")]

    [SerializeField]
    private TextSliderDataSource dataSource;

    [Header("UI References")]

    [SerializeField]
    private GameObject leftArrow;

    [SerializeField]
    private GameObject rightArrow;

    [SerializeField]
    private Text text;
    
    [SerializeField]
    private ValueChangedEvent OnValueChanged;

    #endregion

    #region Fields (Private)

    private bool selected = false;

    private bool hovered = false;

    private bool wasLeft = false;

    private bool wasRight = false;

    #endregion

    #region Interface (Unity Lifecycle)

    protected override void Awake()
    {
        base.Awake();

        // Add Events to detect selection
        EventTrigger.Entry select = new EventTrigger.Entry();
        select.eventID = EventTriggerType.Select;
        select.callback.AddListener((data) => { selected = true; UpdateArrowsDisplayed(); });

        EventTrigger.Entry deselect = new EventTrigger.Entry();
        deselect.eventID = EventTriggerType.Deselect;
        deselect.callback.AddListener((data) => { selected = false; UpdateArrowsDisplayed(); });

        EventTrigger.Entry enter = new EventTrigger.Entry();
        enter.eventID = EventTriggerType.PointerEnter;
        enter.callback.AddListener((data) => { hovered = true; UpdateArrowsDisplayed(); });

        EventTrigger.Entry exit = new EventTrigger.Entry();
        exit.eventID = EventTriggerType.PointerExit;
        exit.callback.AddListener((data) => { hovered = false; UpdateArrowsDisplayed(); });

        EventTrigger trigger = gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = gameObject.AddComponent<EventTrigger>();
        }
        trigger.triggers.AddRange(new EventTrigger.Entry[]
        {
            select,
            deselect,
            enter,
            exit
        });

        dataSource.OnDataSourceForcedUpdate += (_) => { DoUpdate(dataSource.Selected); };
    }

    protected override void Start()
    {
        // Finally, do a simple update
        DoUpdate(dataSource.Selected);
    }

    private void Update()
    {
        if (selected)
        {
            Vector2 nav = InputManager.Instance.QueryAxes(Strings.Input.UI.NAVIGATION);
            bool isLeft = Mathf.Approximately(nav.x, -1);
            bool isRight = Mathf.Approximately(nav.x, 1);

            int index = dataSource.Selected;
            if (!wasLeft && isLeft)
            {
                ButtonLeft();
            }
            else if (!wasRight && isRight)
            {
                ButtonRight();
            }

            wasLeft = isLeft;
            wasRight = isRight;
        }
    }

    #endregion

    #region Interface (Public)

    public void ButtonLeft()
    {
        int index = dataSource.Selected;
        if (index > 0)
        {
            DoUpdate(index - 1);
        }
    }

    public void ButtonRight()
    {
        int index = dataSource.Selected;
        if (index < dataSource.Count - 1)
        {
            DoUpdate(index + 1);
        }
    }

    #endregion

    #region Interface (Private)

    private void DoUpdate(int value)
    {
        // Update DataSource
        dataSource.Selected = value;

        // Update UI
        UpdateArrowsDisplayed();
        text.text = dataSource.Text;

        // Event
        if (OnValueChanged != null) {
            OnValueChanged.Invoke(dataSource, value);
        }
    }

    private void UpdateArrowsDisplayed()
    {
        if (selected || hovered)
        {
            int value = dataSource.Selected;
            leftArrow.SetActive(value > 0);
            rightArrow.SetActive(value < dataSource.Count - 1);
        } else
        {
            leftArrow.SetActive(false);
            rightArrow.SetActive(false);
        }
    }

    #endregion

    #region Types (Public)

    [Serializable]
    public class ValueChangedEvent : UnityEvent<TextSliderDataSource, int> { }

    #endregion
}
