using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ScrollGroup : MonoBehaviour {

    [SerializeField] protected GameObject iconTemplate;
    [SerializeField] protected GameObject groupContent;

    protected abstract string ResourcesPath { get; }
    protected List<PLEUIItem> pleUIItems = new List<PLEUIItem>();

    #region Unity Lifecycle
    private void Start() {
        InitializeUI();
    }
    #endregion


    #region Public Interface
    public void SelectItem(int itemIndex) {
        pleUIItems.ForEach(item => { item.OnGroupSelectChanged(itemIndex); });
    }
    #endregion

    #region Protected Interface
    protected virtual void InitializeUI() {
        GameObject[] gameObjects = Resources.LoadAll<GameObject>(ResourcesPath) as GameObject[];
    #if !UNITY_EDITOR
        Texture2D[] objectTextures;
        objectTextures = Resources.LoadAll<Texture2D>(Strings.Editor.IMAGE_PREVIEW_PATH) as Texture2D[];
    #endif

        for (int i = 0; i < gameObjects.Length; i++) {
            GameObject go = gameObjects[i];

            GameObject toAdd = GameObject.Instantiate(iconTemplate);
            PLEUIItem spawnToSet = toAdd.GetComponent<PLEUIItem>();
            pleUIItems.Add(spawnToSet);
            spawnToSet.InitializeItem(i, this);
            if (i == 0) {
                SelectItem(i);
            }
            spawnToSet.registeredItem = go;

            toAdd.SetActive(true);
            toAdd.name = go.name;

        #if UNITY_EDITOR
            Texture2D icon = UnityEditor.AssetPreview.GetAssetPreview(go);
        #else
            Texture2D icon = objectTextures[i];
        #endif
            if (!icon) {
                Debug.LogWarning("No icon image for prop: " + go.name);
                icon = Texture2D.whiteTexture;
            }
            else {
                Sprite newSprite = Sprite.Create(icon, new Rect(0, 0, icon.width, icon.height), new Vector2(0.5f, 0.5f));
                toAdd.GetComponent<Image>().sprite = newSprite;
            }

            toAdd.transform.SetParent(groupContent.transform);
            RectTransform myRec = toAdd.GetComponent<RectTransform>();
            myRec.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        }
    }
    #endregion
}
