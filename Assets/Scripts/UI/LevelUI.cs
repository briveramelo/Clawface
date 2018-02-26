using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class LevelUI : ClickableBase {

    public LevelData levelData;

    [SerializeField] private Image image;

    private PLELevelSelectMenu menu;
    private int levelIndex;
    private const int imageWidth = 225;
    private const int imageHeight = 225;
    private static readonly Vector2 imageDimensions = new Vector2(imageWidth, imageHeight);

    public void Initialize(PLELevelSelectMenu menu, LevelData levelData, int levelIndex) {
        this.menu = menu;
        this.levelIndex = levelIndex;
        this.levelData = levelData;
        image.sprite = Sprite.Create(levelData.Snapshot, new Rect(Vector2.zero, levelData.size.AsVector), Vector2.one * .5f);
    }

    public override void OnPointerDown(PointerEventData eventData) {
        menu.SelectLevel(levelIndex);
    }
}
