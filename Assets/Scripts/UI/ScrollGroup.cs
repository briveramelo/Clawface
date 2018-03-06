using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ScrollGroup : MonoBehaviour {

    [SerializeField] protected PlacementMenu placementMenu;
    [SerializeField] protected GameObject iconTemplate;
    [SerializeField] protected GameObject groupContent;

    protected abstract string ResourcesPath { get; }
    protected List<PLEUIItem> pleUIItems = new List<PLEUIItem>();
    protected bool hasInitialized = false;
    public int LastSelectedIndex { get; private set; }

    #region Unity Lifecycle
    private void Start() {
        TryInitialize();
    }
    #endregion


    #region Public Interface
    public PLEUIItem GetLastUIItem() {
        TryInitialize();
        return pleUIItems[LastSelectedIndex];
    }
    public virtual PLEUIItem GetUIItem(int index) {
        TryInitialize();
        return pleUIItems[0];
    }
    public void SelectItem(int itemIndex) {
        LastSelectedIndex = itemIndex;
        pleUIItems.ForEach(item => { item.OnGroupSelectChanged(itemIndex); });
        placementMenu.SelectUIItem(pleUIItems[itemIndex]);
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

        SelectItem(0);
    }
    #endregion

    protected void TryInitialize() {
        if (!hasInitialized) {
            InitializeUI();
            hasInitialized = true;
        }
    }
}
