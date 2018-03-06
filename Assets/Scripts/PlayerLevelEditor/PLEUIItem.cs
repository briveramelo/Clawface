using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PLEUIItem : ClickableBase, IUIGroupable {

    public GameObject registeredItem;
    private ScrollGroup scrollGroup;
    private ColorChangingUI colorChangingUI;
    public int ItemIndex { get; private set; }

    public override void OnPointerDown(PointerEventData eventData) {
        scrollGroup.SelectItem(ItemIndex);
    }

    public override void OnPointerEnter(PointerEventData eventData) {
        
    }

    public override void OnPointerExit(PointerEventData eventData) {
        
    }

    public void InitializeItem(int itemIndex, ScrollGroup scrollGroup) {
        this.ItemIndex = itemIndex;
        this.scrollGroup = scrollGroup;
        colorChangingUI = GetComponent<ColorChangingUI>();
        colorChangingUI.SetUIIndex(itemIndex);
    }
    public void OnGroupSelectChanged(int itemIndex) {
        colorChangingUI.OnGroupSelectChanged(itemIndex);
    }

}
