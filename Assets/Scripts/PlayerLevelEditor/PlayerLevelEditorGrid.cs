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

    private GameObject OnClickObject = null;
    private List<GameObject> selectedGameObjects = new List<GameObject>();

    private bool inputGuard = false;

    private List<GameObject> lastHoveredObjects = new List<GameObject>();
    private List<List<GameObject>> lastSelectedGameObjects = new List<List<GameObject>>();
    #endregion

    #region Unity Serialized Fields

    [SerializeField] private GameObject objectGrid;
    [SerializeField] private GameObject realLevel;
    [SerializeField] private GameObject tileParent;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private int levelSize = 5;
    [SerializeField] private Color hoverColor = Color.blue;
    [SerializeField] private Color selectedColor = Color.blue;
    [SerializeField] private Color deletePreviewColor = Color.red;
    [SerializeField] private LevelEditor editorInstance;
    [SerializeField] private NavMeshSurface levelNav;

    #endregion

    #region Public Fields

    [HideInInspector] public bool displaying = false;
    private PLEMenu CurrentEditorMenu { get { return editorInstance.currentDisplayedMenu; } }

    #endregion

    const string GhostBlock = "GhostBlock";
    const string RealBlock = "RealBlock";


    #region Unity Lifecycle

    private void OnEnable()
    {
        if(EventSystem.Instance)
        {
            EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_STARTED, BakeMesh);
        }
    }

    void Start() {
        Initilaize();

        //EventSystem.Instance.RegisterEvent(Strings.Events.PLE_UPDATE_LEVELSTATE, OnTileHeightsChanged);
    }    

    private void OnDestroy() {
        if (EventSystem.Instance) {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.LEVEL_STARTED, BakeMesh);
        }
    }



    void Update()
    {
        if (!displaying)
            return;

        if (MouseHelper.HitItem) {
            RaycastHit hit = MouseHelper.raycastHit.Value;

            if (Input.GetMouseButtonDown(MouseButtons.LEFT) || Input.GetMouseButtonDown(MouseButtons.RIGHT)) {
                OnClickObject = hit.transform.gameObject;
            }

            if (!Input.GetKey(KeyCode.LeftAlt)) {
                HandleWireSelection(hit);
            }
            HandleBlockInteractions(hit);
        }
    }

    #endregion

    #region Public Interface

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

    public List<GameObject> GetSelectedBlocks() { return selectedGameObjects; }

    public void SetGridVisiblity(bool show) {
        displaying = show;
        gridTiles.ForEach(tile => { tile.ToggleGhostGlobal(show); });
    }

    #endregion

    #region Private Interface

    private void BakeMesh(params object[] i_params)
    {
        levelNav.BuildNavMesh();
    }

    private void Initilaize(params object[] par) {
        previewBlock = Resources.Load(Strings.Editor.RESOURCE_PATH + Strings.Editor.BASIC_LE_BLOCK) as GameObject;
        spawnedBlock = Resources.Load(Strings.Editor.RESOURCE_PATH + Strings.Editor.CHERLIN_LVL_BLOCK) as GameObject;

        InitGridTiles();
    }

    private void HandleWireSelection(RaycastHit hit)
    {
        if (hit.transform != null)
        {
            CleanUpLastHoveredObjects(hit);
            UpdateCurrentHoveredObject(hit);
            UpdateLastHoveredObjects(hit);
        }
    }

    private void CleanUpLastHoveredObjects(RaycastHit hit) {
        foreach (GameObject GO in lastHoveredObjects) {
            if (GO == null) {
                continue;
            }
            PreviewCubeController lastPCC = GO.GetComponent<PreviewCubeController>();
            if (lastPCC) {
                lastPCC.ResetColor();
            }
        }

        lastHoveredObjects.Clear();
    }

    private void UpdateCurrentHoveredObject(RaycastHit hit) {
        GameObject currentHoveredObject = hit.transform.gameObject;
        PreviewCubeController currentPCC = currentHoveredObject.GetComponent<PreviewCubeController>();
        if (currentPCC) {
            currentPCC.SetColor(hoverColor);
        }
        lastHoveredObjects.Add(currentHoveredObject);
    }

    private void UpdateLastHoveredObjects(RaycastHit hit) {
        if (Input.GetMouseButton(MouseButtons.LEFT)) {
            List<GameObject> selectedObjects = SelectObjectsAlgorithm(hit);
            foreach (GameObject selectedObject in selectedObjects) {
                PreviewCubeController ObjectPCC = selectedObject.GetComponent<PreviewCubeController>();
                if (ObjectPCC) {
                    lastHoveredObjects.Add(selectedObject);
                    ObjectPCC.SetColor(hoverColor);
                }
            }
        }
    }

    private void HandleBlockInteractions(RaycastHit hit) {
        HandleSelectingBlocks(hit);
        HandleDeleteBlockSelection(hit);
        HandleHoveringBlocks(hit);
    }

    private GridTile lastSelectedTile;
    private void HandleHoveringBlocks(RaycastHit hit) {
        if (!(Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(MouseButtons.LEFT))) {
            if (MouseHelper.HitTile) {
                Vector3 mousePos = MouseHelper.currentBlockUnit.transform.position;
                GridTile hoveredTile = GetTileAtPoint(mousePos);
                if (lastSelectedTile != null && hoveredTile != lastSelectedTile) {
                    if (!hoveredTile.isSelected) {
                        hoveredTile.ChangeColor(hoverColor);
                    }
                    if (!lastSelectedTile.isSelected) {
                        lastSelectedTile.ChangeColor(lastSelectedTile.CurrentTileStateColor);
                    }
                }
                lastSelectedTile = hoveredTile;
            }
            else {
                if (lastSelectedTile != null && !lastSelectedTile.isSelected) {
                    lastSelectedTile.ChangeColor(lastSelectedTile.CurrentTileStateColor);
                }
            }
        }
    }

    private void HandleSelectingBlocks(RaycastHit hit) {
        if (Input.GetMouseButtonDown(MouseButtons.LEFT) && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftAlt)) {
            DeselectBlocks();
        }
        if (Input.GetMouseButton(MouseButtons.LEFT) && Input.GetKey(KeyCode.LeftShift)) {
            ReselectPreviouslySelected();
            SelectBlocks(hit, selectedColor);
        }
        if (Input.GetMouseButtonUp(MouseButtons.LEFT) && Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftAlt)) {
            ToggleLastSelectedObjects(hit);
        }
        if (Input.GetMouseButtonUp(MouseButtons.LEFT) && !Input.GetKey(KeyCode.LeftAlt)) {
            DuplicateBlocks(hit);
            ShowWalls();
            editorInstance.CheckToSetMenuInteractability();
        }
    }

    private void HandleDeleteBlockSelection(RaycastHit hit) {
        if (Input.GetMouseButtonDown(MouseButtons.RIGHT)) {
            DeselectBlocks();
        }
        if (Input.GetMouseButton(MouseButtons.RIGHT) && !Input.GetKey(KeyCode.LeftAlt)) {
            DeselectBlocks();
            SelectBlocks(hit, deletePreviewColor);
        }
        if (Input.GetMouseButtonUp(MouseButtons.RIGHT)) {
            DeselectBlocks();
            if (!Input.GetKey(KeyCode.LeftAlt)) {
                DeleteBlocks(hit);
                ShowWalls();
                editorInstance.CheckToSetMenuInteractability();
            }
        }
    }

    private void InitGridTiles() {
        for (int i = -levelSize; i <= levelSize; i++) {
            for (int j = -levelSize; j <= levelSize; j++) {
                Vector3 position = new Vector3(i * 5, 0, j * 5);
                AddGridTile(position);
            }
        }
    }

    private void AddGridTile(Vector3 position) {
        GameObject ghostBlock = GameObject.Instantiate(previewBlock, position, Quaternion.identity);
        ghostBlock.name = GhostBlock;
        GameObject realBlock = GameObject.Instantiate(spawnedBlock, position, Quaternion.identity);
        realBlock.name = RealBlock;

        GameObject wall_N = GameObject.Instantiate(wallPrefab, position + Vector3.up * 5f + Vector3.forward * 2.5f, Quaternion.Euler(0f, 0f, 0f));
        GameObject wall_E = GameObject.Instantiate(wallPrefab, position + Vector3.up * 5f + Vector3.right * 2.5f, Quaternion.Euler(0f, 90f, 0f));
        GameObject wall_W = GameObject.Instantiate(wallPrefab, position + Vector3.up * 5f + Vector3.left * 2.5f, Quaternion.Euler(0f, 270f, 0f));
        GameObject wall_S = GameObject.Instantiate(wallPrefab, position + Vector3.up * 5f + Vector3.back * 2.5f, Quaternion.Euler(0f, 180f, 0f));

        GridTile tile = new GridTile(realBlock, ghostBlock, position, objectGrid.transform, tileParent.transform, wall_N, wall_E, wall_W, wall_S);
        gridTiles.Add(tile);
    }

    

    private void DuplicateBlocks(RaycastHit hit) {
        List<GameObject> Objects = SelectObjectsAlgorithm(hit);
        DuplicateBlocks(Objects);
    }

    private void DuplicateBlocks(List<GameObject> selectedObjects) {
        for (int i = 0; i < selectedObjects.Count; i++) {
            for (int j = 0; j < gridTiles.Count; j++) {
                GridTile tile = gridTiles[j];
                if (tile.IsEither(selectedObjects[i])) {
                    tile.IsActive = true;
                    PLEBlockUnit blockUnit = tile.realTile.GetComponent<PLEBlockUnit>();
                    blockUnit.SetOccupation(false);
                    break;
                }
            };
        }
    }

    private void DeleteBlocks(RaycastHit hit) {
        List<GameObject> Objects = SelectObjectsAlgorithm(hit);
        DeleteBlocks(Objects);
    }

    private void DeleteBlocks(List<GameObject> selectedObjects) {
        for (int i = 0; i < selectedObjects.Count; i++) {
            for (int j = 0; j < gridTiles.Count; j++) {
                GridTile tile = gridTiles[j];
                if (tile.IsEither(selectedObjects[i])) {
                    tile.IsActive = false;
                    PLEBlockUnit blockUnit = tile.realTile.GetComponent<PLEBlockUnit>();
                    blockUnit.ClearItems();
                    break;
                }
            }
        }
    }

    private void DeselectBlocks() {
        selectedGameObjects.Clear();
        lastSelectedGameObjects.Clear();
        gridTiles.ForEach(tile => {
            tile.ChangeColor(tile.CurrentTileStateColor);
            tile.SetSelected(false);
        });
    }

    private void ReselectPreviouslySelected() {
        selectedGameObjects.Clear();
        gridTiles.ForEach(tile => {
            tile.ChangeColor(tile.CurrentTileStateColor);
            tile.SetSelected(false);
        });

        gridTiles.ForEach(tile => {
            lastSelectedGameObjects.ForEach(lastList => {
                if (lastList.Contains(tile.realTile)) {
                    tile.ChangeColor(selectedColor);
                    tile.SetSelected(true);
                    selectedGameObjects.Add(tile.realTile);
                }
            });
        });
    }

    private void SelectBlocks(RaycastHit hit, Color selectionColor) {
        List<GameObject> selectedObjects = SelectObjectsAlgorithm(hit);

        gridTiles.ForEach(tile => {
            if (selectedObjects.Contains(tile.realTile)) {
                if (lastSelectedGameObjects.Exists(lastList => lastList.Contains(tile.realTile))) {
                    tile.ChangeColor(tile.CurrentTileStateColor);
                    tile.SetSelected(false);
                    if (selectedGameObjects.Contains(tile.realTile)) {
                        selectedGameObjects.Remove(tile.realTile);
                    }
                }
                else {
                    tile.ChangeColor(selectionColor);
                    tile.SetSelected(true);
                    if (!selectedGameObjects.Contains(tile.realTile)) {
                        selectedGameObjects.Add(tile.realTile);
                    }
                }
            }
        });
    }

    private void ToggleLastSelectedObjects(RaycastHit hit) {
        List<GameObject> selectedObjects = SelectObjectsAlgorithm(hit);
        for (int i = lastSelectedGameObjects.Count - 1; i >= 0; i--) {
            for (int j = selectedObjects.Count - 1; j >= 0; j--) {
                if (lastSelectedGameObjects[i].Contains(selectedObjects[j])) {
                    GridTile selectedTile = gridTiles.Find(tile => tile.realTile == selectedObjects[j]);
                    if (selectedTile!=null) {
                        selectedTile.ChangeColor(selectedTile.CurrentTileStateColor);
                        selectedTile.SetSelected(false);
                    }
                    lastSelectedGameObjects[i].Remove(selectedObjects[j]);
                    selectedObjects.Remove(selectedObjects[j]);
                }
            }
        }
        lastSelectedGameObjects.Add(selectedObjects);
    }

    private List<GameObject> SelectObjectsAlgorithm(RaycastHit hit) {
        List<GameObject> selectedObjects = new List<GameObject>();

        if (OnClickObject == null) {
            return selectedObjects;
        }


        float xMax = Mathf.Max(OnClickObject.transform.position.x, hit.transform.position.x);
        float xMin = Mathf.Min(OnClickObject.transform.position.x, hit.transform.position.x);

        float zMax = Mathf.Max(OnClickObject.transform.position.z, hit.transform.position.z);
        float zMin = Mathf.Min(OnClickObject.transform.position.z, hit.transform.position.z);

        float y = OnClickObject.transform.position.y;        


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
        IsActive = false;

        this.ghostParent = ghostParent;
        this.tileParent = tileParent;
        meshRenderer = realTile.GetComponent<MeshRenderer>();
        levelUnit = realTile.GetComponent<LevelUnit>();
        blockUnit = realTile.GetComponent<PLEBlockUnit>();
        meshRenderer.GetPropertyBlock(propBlock);
        realTile.transform.SetParent(ghostParent);
        ghostTile.transform.SetParent(ghostParent);

        wall_N.transform.SetParent(ghostParent);
        wall_E.transform.SetParent(ghostParent);
        wall_W.transform.SetParent(ghostParent);
        wall_S.transform.SetParent(ghostParent);
    }
    public const string BlockColorName = "_AlbedoTint";
    //private const string BlockColor = "_Color";

    Transform ghostParent, tileParent;
    MeshRenderer meshRenderer;
    MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
    LevelUnit levelUnit;
    PLEBlockUnit blockUnit;
    public GameObject realTile;
    public GameObject ghostTile;
    public GameObject wall_N, wall_E, wall_W, wall_S;
    public bool isSelected;
    public Vector3 Position { get { return realTile.transform.position; } set { realTile.transform.position = value; ghostTile.transform.position = value; } }
    public bool IsEither(GameObject other) { return realTile == other || ghostTile == other; }
    public bool IsActive {
        get { return realTile.activeSelf; }
        set {
            bool isActive = value;
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
    public void ChangeColor(Color color) {
        propBlock.SetColor(BlockColorName, color);
        meshRenderer.SetPropertyBlock(propBlock);
    }
    public Color CurrentTileStateColor { get { return levelUnit.CurrentStateColor; } }
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
}