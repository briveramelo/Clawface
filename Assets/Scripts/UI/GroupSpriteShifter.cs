using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupSpriteShifter : MonoBehaviour {

    [SerializeField] private List<ImageToggle> imageToggles;
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite unselectedSprite;

    private void OnEnable() {
        OnToggleChanged();
    }

    public void OnToggleChanged() {
        imageToggles.ForEach(imageToggle => { imageToggle.image.sprite = unselectedSprite; });
        imageToggles.Find(imageToggle => imageToggle.toggle.isOn).image.sprite = selectedSprite;
    }
}

[System.Serializable]
class ImageToggle {
    public Toggle toggle;
    public Image image;
}
