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
    [SerializeField] protected List<GameObject> itemPrefabs;
    [SerializeField] DiffAnim scrollAnim;
    [SerializeField] protected float scrollMultiplier;

    protected abstract string ResourcesPath { get; }
    protected List<PLEUIItem> pleUIItems = new List<PLEUIItem>();
    protected bool hasInitialized = false;
    public int LastSelectedIndex { get; private set; }
    private string ScrollCoroutineName{get{ return coroutineName + "Scroll"; } }

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
    public virtual void SelectUIItem(int itemIndex) {
        LastSelectedIndex = itemIndex;
        pleUIItems.ForEach(item => { item.OnGroupSelectChanged(itemIndex); });
        placementMenu.PostSelectUIItem(pleUIItems[itemIndex]);
    }
    public virtual PLEUIItem SelectLastSelectedUIItem() {
        pleUIItems.ForEach(item => { item.OnGroupSelectChanged(LastSelectedIndex); });
        PLEUIItem lastSelectedItem = pleUIItems[LastSelectedIndex];
        placementMenu.PostSelectUIItem(lastSelectedItem);
        return lastSelectedItem;
    }
    public virtual void DeselectAllUIItems() {
        pleUIItems.ForEach(item => { item.OnGroupSelectChanged(-1); });
    }
    public virtual void MoveScrollbar(bool isRight) {
        float unitTileSize = 1f / (pleUIItems.Count-1);        
        float amountToMove = unitTileSize * (isRight.ToInt()) * scrollMultiplier;
        float targetAmount = scrollbar.value + amountToMove;

        MEC.Timing.KillCoroutines(ScrollCoroutineName);
        scrollAnim.startValue = scrollbar.value;
        scrollAnim.diff = targetAmount - scrollAnim.startValue;
        scrollAnim.Animate(ScrollCoroutineName);
    }
    #endregion

    #region Protected Interface
    protected virtual void InitializeUI() {
        scrollAnim.OnUpdate = (val) => { scrollbar.value = val; };        
        itemPrefabs.OrderBy(go => go.name).ToList();

        for (int i = 0; i < itemPrefabs.Count; i++) {
            GameObject itemPrefab = itemPrefabs[i];
            GameObject toAdd = Instantiate(iconTemplate);
            PLEItem pleItem = itemPrefab.GetComponent<PLEItem>();
            PLEUIItem pleUIItem = toAdd.GetComponent<PLEUIItem>();
            pleUIItems.Add(pleUIItem);
            pleUIItem.InitializeItem(i, this, itemPrefab);

            toAdd.SetActive(true);
            toAdd.name = itemPrefab.name;

            toAdd.transform.SetParent(groupContent.transform);
            RectTransform myRec = toAdd.GetComponent<RectTransform>();
            myRec.localScale = Vector3.one;
        }

        SelectUIItem(0);
    }
    #endregion

    protected void TryInitialize() {
        if (!hasInitialized) {
            InitializeUI();
            hasInitialized = true;
        }
    }
}
