using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PLEUIItem : ClickableBase, IUIGroupable {

    public PLEItem pleItem;
    public GameObject registeredItem;
    private ScrollGroup scrollGroup;
    private ColorChangingUI colorChangingUI;
    public bool isInteractable=true;
    public int ItemIndex { get; private set; }    

    public override void OnPointerDown(PointerEventData eventData) {
        if (isInteractable) {
            scrollGroup.SelectItem(ItemIndex);
        }
    }

    public override void OnPointerEnter(PointerEventData eventData) {
        
    }

    public override void OnPointerExit(PointerEventData eventData) {
        
    }

    public void InitializeItem(int itemIndex, ScrollGroup scrollGroup, GameObject registeredItem) {
        isInteractable = true;
        this.ItemIndex = itemIndex;
        this.scrollGroup = scrollGroup;
        colorChangingUI = GetComponent<ColorChangingUI>();
        colorChangingUI.SetUIIndex(itemIndex);
        this.registeredItem = registeredItem;
        this.pleItem = registeredItem.GetComponent<PLEItem>();
    }

    public void ToggleInteractable(bool isInteractable) {
        this.isInteractable = isInteractable;
        colorChangingUI.ToggleInteractable(isInteractable);
        //other visual indication..?
    }

    public void OnGroupSelectChanged(int itemIndex) {
        if (ItemIndex!=itemIndex || isInteractable) {
            colorChangingUI.OnGroupSelectChanged(itemIndex);
        }
    }

}
