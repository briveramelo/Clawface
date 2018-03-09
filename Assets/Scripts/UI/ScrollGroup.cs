using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using ModMan;
public abstract class ScrollGroup : RoutineRunner {

    [SerializeField] protected PlacementMenu placementMenu;
    [SerializeField] protected GameObject iconTemplate;
    [SerializeField] protected GameObject groupContent;
    [SerializeField] protected Scrollbar scrollbar;
    [SerializeField] DiffAnim scrollAnim;
    [SerializeField] protected float scrollMultiplier;

    protected abstract string ResourcesPath { get; }
    protected abstract string IconImagePath { get; }
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
        return pleUIItems[index];
    }
    public virtual PLEUIItem TryGetFirstAvailableUIItem() {
        TryInitialize();
        return pleUIItems.Find(item => item.isInteractable);
    }
    public virtual void SelectItem(int itemIndex) {
        LastSelectedIndex = itemIndex;
        pleUIItems.ForEach(item => { item.OnGroupSelectChanged(itemIndex); });
        placementMenu.TrySelectUIItem(pleUIItems[itemIndex]);
    }
    public virtual void DeselectAll() {
        pleUIItems.ForEach(item => { item.OnGroupSelectChanged(-1); });
    }
    public virtual void MoveScrollbar(bool isRight) {
        float unitTileSize = 1f / (pleUIItems.Count-1);        
        float amountToMove = unitTileSize * (isRight.ToInt()) * scrollMultiplier;
        float targetAmount = scrollbar.value + amountToMove;

        MEC.Timing.KillCoroutines(coroutineName);
        scrollAnim.startValue = scrollbar.value;
        scrollAnim.diff = targetAmount - scrollAnim.startValue;
        scrollAnim.Animate(coroutineName);
    }
    #endregion

    #region Protected Interface
    protected virtual void InitializeUI() {
        scrollAnim.OnUpdate = (val) => { scrollbar.value = val; };

        List<GameObject> itemPrefabs = (Resources.LoadAll<GameObject>(ResourcesPath) as GameObject[]).ToList();
        itemPrefabs.OrderBy(go => go.name).ToList();

        Texture2D[] objectTextures;
        objectTextures = Resources.LoadAll<Texture2D>(IconImagePath) as Texture2D[];

        for (int i = 0; i < itemPrefabs.Count; i++) {
            GameObject itemPrefab = itemPrefabs[i];
            GameObject toAdd = Instantiate(iconTemplate);
            PLEItem pleItem = itemPrefab.GetComponent<PLEItem>();
            PLEUIItem pleUIItem = toAdd.GetComponent<PLEUIItem>();
            pleUIItems.Add(pleUIItem);
            pleUIItem.InitializeItem(i, this, itemPrefab);

            toAdd.SetActive(true);
            toAdd.name = itemPrefab.name;
            if (pleItem.iconPreview!=null) {
                pleUIItem.imagePreview.sprite = pleItem.iconPreview;
            }
            else {
                Texture2D itemTex = null;
                if (i >= objectTextures.Length) {
                    itemTex = Texture2D.whiteTexture;
                }
                else {
                    itemTex = objectTextures[i];
                }
                Sprite newSprite = Sprite.Create(itemTex, new Rect(0, 0, itemTex.width, itemTex.height), new Vector2(0.5f, 0.5f));
                pleUIItem.imagePreview.sprite = newSprite;
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
