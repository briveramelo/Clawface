using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class LevelUI : ClickableBase, IUIGroupable {

    public LevelData levelData;

    [SerializeField] private Image image;
    [SerializeField] private ColorChangingUI colorChangingUI;
    private PLELevelSelectMenu menu;
    private int levelIndex;
    private const int imageWidth = 225;
    private const int imageHeight = 225;
    private static readonly Vector2 imageDimensions = new Vector2(imageWidth, imageHeight);

    public void Initialize(PLELevelSelectMenu menu, LevelData levelData, int levelIndex) {
        this.menu = menu;
        this.levelIndex = levelIndex;
        colorChangingUI.SetUIIndex(levelIndex);
        this.levelData = levelData;
        image.sprite = levelData.MySprite;
    }

    public override void OnPointerDown(PointerEventData eventData) {
        menu.SelectLevel(levelIndex);
    }

    public void OnGroupSelectChanged(int selectedIndex) {
        colorChangingUI.OnGroupSelectChanged(selectedIndex);
    }
}
