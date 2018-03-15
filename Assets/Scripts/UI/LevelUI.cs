using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class LevelUI : ClickableBase, IUIGroupable {

    public LevelData levelData;
    public Image image;
    public Selectable selectable;

    [SerializeField] private ColorChangingUI colorChangingUI;
    [SerializeField] private Image favoriteIcon;
    [SerializeField] private Image outline;

    private PLELevelSelectMenu menu;
    public int LevelIndex { get; private set; }
    private const int imageWidth = 225;
    private const int imageHeight = 225;
    private static readonly Vector2 imageDimensions = new Vector2(imageWidth, imageHeight);    

    public void Initialize(PLELevelSelectMenu menu, LevelData levelData, int levelIndex, bool isHathosLevel=false) {
        this.menu = menu;
        this.LevelIndex = levelIndex;
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
        menu.SelectLevel(LevelIndex);
    }

    public void OnGroupSelectChanged(int selectedIndex) {
        colorChangingUI.OnGroupSelectChanged(selectedIndex);
    }
    public void ScaleLevelUISize(float scaleMutliplier) {
        transform.localScale = Vector3.one * scaleMutliplier;
    }
    public float CurrentScale { get { return outline.transform.localScale.x; } }
}
