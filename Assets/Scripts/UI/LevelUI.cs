using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class LevelUI : ClickableBase, IUIGroupable {

    public LevelData levelData;
    public Image image;
    [SerializeField] private ColorChangingUI colorChangingUI;
    [SerializeField] private Image favoriteIcon;

    private PLELevelSelectMenu menu;
    private int levelIndex;
    private const int imageWidth = 225;
    private const int imageHeight = 225;
    private static readonly Vector2 imageDimensions = new Vector2(imageWidth, imageHeight);

    public void Initialize(PLELevelSelectMenu menu, LevelData levelData, int levelIndex, bool isHathosLevel=false) {
        this.menu = menu;
        this.levelIndex = levelIndex;
        colorChangingUI.SetUIIndex(levelIndex);
        if (!isHathosLevel) {
            favoriteIcon.enabled = levelData.isFavorite;
            this.levelData = levelData;
            image.sprite = levelData.MySprite;
        }
    }

    public void ToggleIsFavorite() {
        levelData.isFavorite = !levelData.isFavorite;
        favoriteIcon.enabled = levelData.isFavorite;
    }

    public override void OnPointerDown(PointerEventData eventData) {
        menu.SelectLevel(levelIndex);
    }

    public void OnGroupSelectChanged(int selectedIndex) {
        colorChangingUI.OnGroupSelectChanged(selectedIndex);
    }
}
