using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScrollbarMover : MonoBehaviour {

    [SerializeField] private Scrollbar scrollBar;
    [SerializeField] private float scrollSpeedMultiplier=1f;
    private void Update() {
        HandleScrollbar();
    }
    void HandleScrollbar() {
        float mouseScrollDeltaY = Input.mouseScrollDelta.y;
        if (Mathf.Abs(mouseScrollDeltaY) > 0) {
            scrollBar.value += mouseScrollDeltaY * scrollSpeedMultiplier;
        }
    }
}
