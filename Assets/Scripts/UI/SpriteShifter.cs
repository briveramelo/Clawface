using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteShifter : ClickableBase {

    [SerializeField] Image myImage;
    [SerializeField] Toggle myToggle;
    [SerializeField] Sprite selectedSprite;
    [SerializeField] Sprite unselectedSprite;

    public void OnToggleChanged() {
        myImage.sprite = myToggle.isOn ? selectedSprite : unselectedSprite;
    }
    
}
