using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

public class SelectorToggleGroup : MonoBehaviour {

    [SerializeField] List<SelectorToggle> selectorToggles;
    private SelectorToggle selectedToggle;

    public void HandleGroupSelection(int toggleIndex) {
        if (selectorToggles.IsValidIndex(toggleIndex)) {
            SelectorToggle toggle = selectorToggles[toggleIndex];
            if (toggle != null) {
                HandleGroupSelection(toggle);
            }
        }
    }

    public void HandleGroupSelection(SelectorToggle selectedToggle) {
        this.selectedToggle = selectedToggle;
        selectorToggles.ForEach(selectorToggle => {
            selectorToggle.SetIsSelected(selectorToggle == selectedToggle);
        });
    }

    public bool ShouldStaySelected(SelectorToggle selectedToggle) {
        return this.selectedToggle != null && this.selectedToggle == selectedToggle;
    }
}
