using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using PlayerLevelEditor;
using ModMan;
using System.Linq;
using UnityEngine.AI;

public class PlayerLevelEditorGrid : MonoBehaviour {
    #region Private Fields
    private List<GridTile> gridTiles = new List<GridTile>();

    private GameObject previewBlock = null;
    private GameObject spawnedBlock = null;

    private GameObject onClickObject = null;
    private List<GridTile> selectedGridTiles = new List<GridTile>();

    private bool inputGuard = false;
    private GridTile currentHoveredTile;
    private GridTile HoveredTile {
        get {
            return currentHoveredTile;
        }
        set {
            GridTile newHoveredTile = value;
            HoverTile(currentHoveredTile, false);
            HoverTile(newHoveredTile, true);
            currentHoveredTile = newHoveredTile;
        }
    }
    private void HoverTile(GridTile tile, bool isHovered) {
        if (tile != null) {
            tile.isHovered = isHovered;
            Color blockColor = isHovered ? hoverColor : (tile.isSelected ? selectedColor : tile.CurrentTileStateColor);
            tile.ChangeRealBlockColor(blockColor);

            Color? ghostColor = isHovered ? (hoverColor as Color?) : null;
            tile.ChangeHoverGhostColor(ghostColor);
        }
    }

    private List<GridTile> lastHighlightedGhostTiles = new List<GridTile>();
    private List<List<GameObject>> lastSelectedGameObjects = new List<List<GameObject>>();
    #endregion

    #region Unity Serialized Fields

    [SerializeField] private Transform objectGrid;
    [SerializeField] private Transform tileParent;
    [SerializeField] private Transform spawnsParent;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private int levelSize = 5;
    [SerializeField] private Color hoverColor = Color.blue;
    [SerializeField] private Color selectedColor = Color.blue;
    [SerializeField] private Color deletePreviewColor = Color.red;
    [SerializeField] private LevelEditor editorInstance;
    [SerializeField] private MainPLEMenu mainPLEMenu;
    [SerializeField] private NavMeshSurface levelNav;

    #endregion

    #region Public Fields
    public int LevelSize { get { return levelSize; } }
    [HideInInspector] public bool displaying = false;

    #endregion




    #region Unity Lifecycle
    void Awake() {
        Initialize();
        EventSystem.Instance.RegisterEvent(Strings.Events.PLE_ON_LEVEL_DATA_LOADED, BakeNavMesh);
        //EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_RESTARTED, BakeNavMesh);
    }

    private void OnDestroy() {
        if (EventSystem.Instance) {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLE_ON_LEVEL_DATA_LOADED, BakeNavMesh);
            //EventSystem.Instance.UnRegisterEvent(Strings.Events.LEVEL_RESTARTED, BakeNavMesh);
        }
    }

    void Update() {
        if (!displaying)
            return;
                

        if (MouseHelper.HitItem) {
            RaycastHit hit = MouseHelper.raycastHit.Value;

            if (Input.GetMouseButtonDown(MouseButtons.LEFT) || Input.GetMouseButtonDown(MouseButtons.RIGHT)) {
                onClickObject = hit.transform.gameObject;
            }

            TryHoverTile();
            HandleBlockSelectionInteractions(hit);
        }
        HandleGroupGhostSelectionPreview();
    }

    #endregion

    #region Public Interface
    public List<GridTile> GetSelectedGridTiles() { return selectedGridTiles; }
    public bool AnyTilesSelected() { return selectedGridTiles.Count > 0; }
    public bool AnyTilesEnabled() {
        return gridTiles.Any(tile => tile.IsActive);
    }

    public void ShowWalls() {
        gridTiles.ForEach(tile => tile.EnableWalls());
    }
    public GridTile GetTileAtPoint(Vector3 point) {
        return gridTiles.Find(tile => tile.Position.NoY().IsAboutEqual(point.NoY()));
    }
    public void ResetGrid() {
        gridTiles.ForEach(tile => { tile.IsActive = false; });
    }

    public void ClearSelectedBlocks() {
        if (gameObject.activeSelf) {
            DeselectBlocks();
        }
    }

    public void SetGridVisiblity(bool show) {
        displaying = show;
        gridTiles.ForEach(tile => { tile.ToggleGhostGlobal(show); });
    }
    public void ResetTileHeightsAndStates() {
        gridTiles.ForEach(tile => {
            if (tile.IsActive) {
                tile.ResetTileHeightAndStates();
            }
        });
    }
    #endregion

    #region Private Interface

    public void BakeNavMesh(params object[] i_params) {
        spawnsParent.gameObject.SetActive(false);
        ResetTileHeightsAndStates();
        levelNav.BuildNavMesh();
        spawnsParent.gameObject.SetActive(true);
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_ON_LEVEL_READY);
    }

    #region Initialization
    private void Initialize() {
        previewBlock = Resources.Load(Strings.Editor.RESOURCE_PATH + Strings.Editor.BASIC_LE_BLOCK) as GameObject;
        spawnedBlock = Resources.Load(Strings.Editor.RESOURCE_PATH + Strings.Editor.CHERLIN_LVL_BLOCK) as GameObject;

        InitializeGridTiles();
    }

    private void InitializeGridTiles() {
        for (int i = -levelSize; i <= levelSize; i++) {
            for (int j = -levelSize; j <= levelSize; j++) {
                Vector3 position = new Vector3(i * 5, 0, j * 5);
                AddGridTile(position);
            }
        }
    }

    private void AddGridTile(Vector3 position) {
        GameObject ghostBlock = GameObject.Instantiate(previewBlock, position, Quaternion.identity);
        ghostBlock.name = Strings.GHOST_BLOCK;
        GameObject realBlock = GameObject.Instantiate(spawnedBlock, position, Quaternion.identity);
        realBlock.name = Strings.REAL_BLOCK;

        GameObject wall_N = GameObject.Instantiate(wallPrefab, position + Vector3.up * 5.001f + Vector3.forward * 2.5f, Quaternion.Euler(0f, 0f, 0f));
        GameObject wall_E = GameObject.Instantiate(wallPrefab, position + Vector3.up * 5.001f + Vector3.right * 2.5f, Quaternion.Euler(0f, 90f, 0f));
        GameObject wall_W = GameObject.Instantiate(wallPrefab, position + Vector3.up * 5.001f + Vector3.left * 2.5f, Quaternion.Euler(0f, 270f, 0f));
        GameObject wall_S = GameObject.Instantiate(wallPrefab, position + Vector3.up * 5.001f + Vector3.back * 2.5f, Quaternion.Euler(0f, 180f, 0f));

        GridTile tile = new GridTile(realBlock, ghostBlock, position, objectGrid, tileParent, wall_N, wall_E, wall_W, wall_S);
        gridTiles.Add(tile);
    }
    #endregion

    #region Hovering Ghosts
    private void HandleGroupGhostSelectionPreview() {
        if (MouseHelper.HitItem) {
            RaycastHit hit = MouseHelper.raycastHit.Value;
            if (!Input.GetKey(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.Z)) {
                if (Input.GetMouseButton(MouseButtons.LEFT)) {
                    UnhighlightGhostTiles();
                    HighlightGhostTiles(hit);
                }
            }
        }
        if (Input.GetMouseButtonUp(MouseButtons.LEFT)) {
            UnhighlightGhostTiles();
        }
    }    

    private void UnhighlightGhostTiles() {
        lastHighlightedGhostTiles.ForEach(tile => { tile.ChangeHoverGhostColor(); });
        lastHighlightedGhostTiles.Clear();
    }

    private void HighlightGhostTiles(RaycastHit hit) {
        if (Input.GetMouseButton(MouseButtons.LEFT)) {
            List<GameObject> selectedObjects = SelectObjectsAlgorithm(hit);

            for (int i=0; i<selectedObjects.Count; i++) {
                GridTile currentTile = gridTiles.Find(tile => tile.ghostTile == selectedObjects[i]);
                if (currentTile!=null) {
                    currentTile.TryShowGhost();
                    currentTile.ChangeHoverGhostColor(hoverColor);
                    if (!lastHighlightedGhostTiles.Contains(currentTile)) {
                        lastHighlightedGhostTiles.Add(currentTile);
                    }
                }
            }
        }

    }
    #endregion

    private void TryHoverTile() {
        if (!Input.GetKey(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.Z)) {
            Vector3 blockPosition = MouseHelper.currentHoveredObject != null ? MouseHelper.currentHoveredObject.transform.position : Vector3.one * 10000;
            GridTile newHoveredRealTile = GetTileAtPoint(blockPosition);
            HoveredTile = newHoveredRealTile;
        }
    }
    #region Real Tiles Interactions


    private void HandleBlockSelectionInteractions(RaycastHit hit) {
        HandleSelectingBlocks(hit);
        HandleDeleteBlockSelection(hit);
    }        

    private void HandleSelectingBlocks(RaycastHit hit) {
        if (!Input.GetKey(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.Z)) {
            if (Input.GetMouseButtonDown(MouseButtons.LEFT) && !Input.GetKey(KeyCode.LeftShift)) {
                DeselectBlocks();
                mainPLEMenu.SetMenuButtonInteractabilityByState();
            }
            if (Input.GetMouseButton(MouseButtons.LEFT)) {
                ReselectPreviouslySelected();
                SelectBlocks(hit, selectedColor);
            }
            if (Input.GetMouseButtonUp(MouseButtons.LEFT)) {
                ToggleLastSelectedObjects(hit);
            }
            if (Input.GetMouseButtonUp(MouseButtons.LEFT)) {
                ShowBlocks(hit);
                ShowWalls();
                mainPLEMenu.SetMenuButtonInteractabilityByState();
            }
        }
    }

    private void HandleDeleteBlockSelection(RaycastHit hit) {
        if (Input.GetMouseButtonDown(MouseButtons.RIGHT)) {
            DeselectBlocks();
            mainPLEMenu.SetMenuButtonInteractabilityByState();
        }
        if (Input.GetMouseButton(MouseButtons.RIGHT) && !Input.GetKey(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.Z)) {
            DeselectBlocks();
            SelectBlocks(hit, deletePreviewColor);
        }
        if (Input.GetMouseButtonUp(MouseButtons.RIGHT)) {
            DeselectBlocks();
            if (!Input.GetKey(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.Z)) {
                DeleteBlocks(hit);
                ShowWalls();
                mainPLEMenu.SetMenuButtonInteractabilityByState();
            }
        }
    }

    private void ShowBlocks(RaycastHit hit) {
        List<GameObject> Objects = SelectObjectsAlgorithm(hit);
        ShowBlocks(Objects);
    }

    private void ShowBlocks(List<GameObject> selectedObjects) {
        for (int i = 0; i < selectedObjects.Count; i++) {
            GridTile selectedTile = gridTiles.Find(tile => tile.ghostTile == selectedObjects[i]);
            if (selectedTile != null) {
                selectedTile.IsActive = true;
                selectedTile.ResetTileHeightAndStates();
                selectedTile.blockUnit.SetOccupation(false);
            }
        }
    }

    private void DeleteBlocks(RaycastHit hit) {
        List<GameObject> Objects = SelectObjectsAlgorithm(hit);
        DeleteBlocks(Objects);
    }

    private void DeleteBlocks(List<GameObject> selectedObjects) {
        for (int i = 0; i < selectedObjects.Count; i++) {
            GridTile selectedTile = gridTiles.Find(tile => tile.realTile == selectedObjects[i]);
            if (selectedTile != null) {
                selectedTile.IsActive = false;
                selectedTile.blockUnit.ClearItems();
            }
        }
    }

    private void DeselectBlocks() {
        selectedGridTiles.Clear();
        lastSelectedGameObjects.Clear();
        gridTiles.ForEach(tile => {
            SelectTile(tile, tile.CurrentTileStateColor, false);
        });
    }

    private void ReselectPreviouslySelected() {
        selectedGridTiles.Clear();
        gridTiles.ForEach(tile => {
            SelectTile(tile, tile.CurrentTileStateColor, false);
        });

        gridTiles.ForEach(tile => {
            lastSelectedGameObjects.ForEach(lastList => {
                if (lastList.Contains(tile.realTile)) {
                    SelectTile(tile, selectedColor, true);
                }
            });
        });
    }

    private void SelectBlocks(RaycastHit hit, Color selectionColor) {
        List<GameObject> selectedObjects = SelectObjectsAlgorithm(hit);

        gridTiles.ForEach(tile => {
            if (selectedObjects.Contains(tile.realTile)) {
                if (lastSelectedGameObjects.Exists(lastList => lastList.Contains(tile.realTile))) {
                    SelectTile(tile, tile.CurrentTileStateColor, false);
                }
                else {
                    SelectTile(tile, selectionColor, true);
                }
            }
        });
    }

    private void SelectTile(GridTile tile, Color selectionColor, bool isSelected) {
        tile.ChangeRealBlockColor(selectionColor);
        tile.SetSelected(isSelected);

        if (isSelected && !selectedGridTiles.Contains(tile)) {
            selectedGridTiles.Add(tile);
        }
        else if (selectedGridTiles.Contains(tile)) {
            selectedGridTiles.Remove(tile);
        }
    }

    private void ToggleLastSelectedObjects(RaycastHit hit) {
        List<GameObject> selectedObjects = SelectObjectsAlgorithm(hit);


        for (int i = lastSelectedGameObjects.Count - 1; i >= 0; i--) {
            for (int j = selectedObjects.Count - 1; j >= 0; j--) {
                if (lastSelectedGameObjects[i].Contains(selectedObjects[j])) {
                    GridTile selectedTile = gridTiles.Find(tile => tile.realTile == selectedObjects[j]);
                    lastSelectedGameObjects[i].Remove(selectedObjects[j]);

                    SelectTile(selectedTile, selectedTile.CurrentTileStateColor, false);
                }
            }
        }
        lastSelectedGameObjects.Add(selectedObjects);
    }
    #endregion


    private List<GameObject> SelectObjectsAlgorithm(RaycastHit hit) {
        List<GameObject> selectedObjects = new List<GameObject>();

        if (onClickObject == null) {
            return selectedObjects;
        }


        float xMax = Mathf.Max(onClickObject.transform.position.x, hit.transform.position.x);
        float xMin = Mathf.Min(onClickObject.transform.position.x, hit.transform.position.x);

        float zMax = Mathf.Max(onClickObject.transform.position.z, hit.transform.position.z);
        float zMin = Mathf.Min(onClickObject.transform.position.z, hit.transform.position.z);

        float y = onClickObject.transform.position.y;        


        var HitsInX = Physics.BoxCastAll(new Vector3(xMin, y, zMin), new Vector3(1, 40, 1), Vector3.right, Quaternion.identity, xMax - xMin);
        
        foreach (var itemX in HitsInX) {
            if(!selectedObjects.Contains(itemX.transform.gameObject))
                selectedObjects.Add(itemX.transform.gameObject);

            var HitsInZ = Physics.BoxCastAll(itemX.transform.position , new Vector3(1, 40, 1), Vector3.forward, Quaternion.identity, zMax - zMin);

            foreach (var itemZ in HitsInZ) {
                if (!selectedObjects.Contains(itemZ.transform.gameObject)) {
                    selectedObjects.Add(itemZ.transform.gameObject);
                }
            }
        }

        return selectedObjects;
    }    
    #endregion

    
}

[System.Serializable]
public class GridTile {
    public GridTile(GameObject real, GameObject ghost, Vector3 position, Transform ghostParent, Transform tileParent, GameObject wall_N, GameObject wall_E, GameObject wall_W, GameObject wall_S) {
        this.wall_N = wall_N;
        this.wall_E = wall_E;
        this.wall_S = wall_S;
        this.wall_W = wall_W;

        wall_N.SetActive(false);
        wall_E.SetActive(false);
        wall_W.SetActive(false);
        wall_S.SetActive(false);

        realTile = real;
        ghostTile = ghost;
        Position = position;

        this.ghostParent = ghostParent;
        this.tileParent = tileParent;
        meshRenderer = realTile.GetComponent<MeshRenderer>();
        levelUnit = realTile.GetComponent<LevelUnit>();
        blockUnit = realTile.GetComponent<PLEBlockUnit>();
        ghostPreview = ghostTile.GetComponent<PreviewCubeController>();
        meshRenderer.GetPropertyBlock(propBlock);
        realTile.transform.SetParent(ghostParent);
        ghostTile.transform.SetParent(ghostParent);

        wall_N.transform.SetParent(ghostParent);
        wall_E.transform.SetParent(ghostParent);
        wall_W.transform.SetParent(ghostParent);
        wall_S.transform.SetParent(ghostParent);

        IsActive = false;
    }
    public const string BlockColorName = "_AlbedoTint";

    Transform ghostParent, tileParent;
    MeshRenderer meshRenderer;
    PreviewCubeController ghostPreview;
    MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
    public LevelUnit levelUnit;
    public PLEBlockUnit blockUnit;
    public GameObject realTile;
    public GameObject ghostTile;
    public GameObject wall_N, wall_E, wall_W, wall_S;
    public bool isSelected;
    public bool isHovered;

    public Vector3 Position { get { return realTile.transform.position; } set { realTile.transform.position = value; ghostTile.transform.position = value; } }
    public bool IsEither(GameObject other) { return realTile == other || ghostTile == other; }
    public bool IsActive {
        get { return realTile.activeInHierarchy; }
        set {
            bool isActive = value;
            if (!isActive) {
                levelUnit.HideBlockingObject();
            }
            realTile.SetActive(isActive);
            ghostTile.SetActive(!isActive);
            Transform newParent = isActive ? tileParent : ghostParent;
            realTile.transform.SetParent(newParent);
        }
    }
    public void ToggleGhostGlobal(bool showGlobal) {
        if (showGlobal) {
            TryShowGhost();
        }
        else {
            HideGhost();
        }
    }
    public void HideGhost() {
        ghostTile.SetActive(false);
    }
    public void TryShowGhost() {
        ghostTile.SetActive(!IsActive);
    }
    public void ChangeHoverGhostColor(Color? newColor=null) {
        if (newColor == null) {
            ghostPreview.ResetColor();
        }
        else {
            ghostPreview.SetColor(newColor.Value);
        }
    }


    public void ChangeRealBlockColor(Color color) {
        propBlock.SetColor(BlockColorName, color);
        meshRenderer.SetPropertyBlock(propBlock);
    }
    public Color CurrentTileStateColor { get { return levelUnit.CurrentStateColor; } }
    public Color FloorTileStateColor { get { return levelUnit.FlatColor; } }
    public void SetSelected(bool isSelected) {
        this.isSelected = isSelected;
    }

    public void EnableWalls() {
        bool blockNorth = false;
        bool blockEast = false;
        bool blockWest = false;
        bool blockSouth = false;

        if (IsActive) {
            blockNorth = Physics.OverlapBox(realTile.transform.position + Vector3.forward * 5f, new Vector3(1,10,1) ).ToList().Exists(item => item.GetComponent<PLEBlockUnit>());
            blockEast = Physics.OverlapBox(realTile.transform.position + Vector3.right * 5f, new Vector3(1, 10, 1)).ToList().Exists(item => item.GetComponent<PLEBlockUnit>());
            blockWest = Physics.OverlapBox(realTile.transform.position + Vector3.left * 5f, new Vector3(1, 10, 1)).ToList().Exists(item => item.GetComponent<PLEBlockUnit>());
            blockSouth = Physics.OverlapBox(realTile.transform.position + Vector3.back * 5f, new Vector3(1, 10, 1)).ToList().Exists(item => item.GetComponent<PLEBlockUnit>());
        }

        wall_N.SetActive(IsActive && !blockNorth);
        wall_E.SetActive(IsActive && !blockEast);
        wall_W.SetActive(IsActive && !blockWest);
        wall_S.SetActive(IsActive && !blockSouth);
    }

    public void ResetTileHeightAndStates()
    {
        blockUnit.SyncTileHeightStates();
        levelUnit.SnapToFloorState();
        ChangeRealBlockColor(FloorTileStateColor);
    }
}