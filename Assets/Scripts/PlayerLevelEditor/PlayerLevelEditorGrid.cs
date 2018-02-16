using System.Collections.Generic;
using UnityEngine;
using PlayerLevelEditor;
using ModMan;
public class PlayerLevelEditorGrid : MonoBehaviour {
    #region Private Fields
    private List<GridTile> gridTiles = new List<GridTile>();

    private GameObject previewBlock = null;
    private GameObject spawnedBlock = null;
    private Color spawnddBlockDefaultColor;

    private GameObject OnClickObject = null;
    private List<GameObject> selectedObjects = new List<GameObject>();

    private bool inputGuard = false;

    private List<GameObject> lastHoveredObjects = new List<GameObject>();

    #endregion

    #region Unity Serialized Fields

    [SerializeField] GameObject objectGrid;
    [SerializeField] GameObject realLevel;
    [SerializeField] GameObject tileParent;
    private int levelSize = 5;
    [SerializeField] private Color hoverColor = Color.blue;
    [SerializeField] private Color selectedColor = Color.red;
    [SerializeField] private LevelEditor editorInstance;

    #endregion

    #region Public Fields

    [HideInInspector] public bool displaying = false;
    [HideInInspector] public EditorMenu currentEditorMenu = EditorMenu.MAIN_EDITOR_MENU;

    #endregion

    const string GhostBlock = "GhostBlock";
    const string RealBlock = "RealBlock";


    #region Unity Lifecycle

    // Use this for initialization
    void Start() {
        Initilaize();
    }


    // Update is called once per frame
    void Update() {
        if (!displaying)
            return;

        if (MouseHelper.hitItem) {
            RaycastHit hit = MouseHelper.raycastHit;
            if (Input.GetMouseButtonDown(MouseButtons.LEFT) || Input.GetMouseButtonDown(MouseButtons.RIGHT)) {
                OnClickObject = hit.transform.gameObject;
            }

            if (currentEditorMenu == EditorMenu.PROPS_MENU) {
                //                DrawPreviewBlock(hit);
            }
            else if (currentEditorMenu == EditorMenu.FLOOR_MENU) {
                DrawPreviewBlock(hit);
                CreateBlock(hit);
            }


        }
    }

    #endregion

    #region Private Interface

    private void Initilaize(params object[] par) {
        previewBlock = Resources.Load(Strings.Editor.RESOURCE_PATH + Strings.Editor.BASIC_LE_BLOCK) as GameObject;
        spawnedBlock = Resources.Load(Strings.Editor.RESOURCE_PATH + Strings.Editor.BASIC_LVL_BLOCK) as GameObject;

        spawnddBlockDefaultColor = spawnedBlock.GetComponent<Renderer>().sharedMaterial.color;

        InitGridTiles();
    }

    private void CreateBlock(RaycastHit hit) {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonUp(MouseButtons.LEFT)) {
            SelectBlocks(hit);
        }
        else if (Input.GetMouseButtonUp(MouseButtons.LEFT)) {
            DeselectBlocks();
            DuplicateBlocks(hit);
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonUp(MouseButtons.RIGHT)) {
            DeselectBlocks();
            DeleteBlocks(hit);
        }
    }

    private void DrawPreviewBlock(RaycastHit hit) {
        #region clean up lastHoveredObjects

        foreach (GameObject GO in lastHoveredObjects) {
            if (GO == null)
                continue;

            PreviewCubeController lastPCC = GO.GetComponent<PreviewCubeController>();

            if (lastPCC)
                lastPCC.ResetColor();
        }

        lastHoveredObjects.Clear();

        #endregion

        #region update currentHoveredObject

        GameObject currentHoveredObject = hit.transform.gameObject;
        PreviewCubeController currentPCC = currentHoveredObject.GetComponent<PreviewCubeController>();

        if (currentPCC)
            currentPCC.SetColor(hoverColor);

        lastHoveredObjects.Add(currentHoveredObject);

        #endregion

        #region update lastHoveredObjects

        if (Input.GetMouseButton(MouseButtons.LEFT)) {
            List<GameObject> Objects = SelectObjectsAlgorithm(hit);

            foreach (GameObject Object in Objects) {
                PreviewCubeController ObjectPCC = Object.GetComponent<PreviewCubeController>();

                if (ObjectPCC) {
                    lastHoveredObjects.Add(Object);
                    ObjectPCC.SetColor(hoverColor);
                }
            }
        }

        #endregion

    }

    void InitGridTiles() {
        for (int i = -levelSize; i <= levelSize; i++) {
            for (int j = -levelSize; j <= levelSize; j++) {
                Vector3 position = new Vector3(i * 5, 0, j * 5);
                AddGridTile(position);
            }
        }
    }

    void AddGridTile(Vector3 position) {
        GameObject ghostBlock = GameObject.Instantiate(previewBlock, position, Quaternion.identity);
        ghostBlock.name = GhostBlock;
        GameObject realBlock = GameObject.Instantiate(spawnedBlock, position, Quaternion.identity);
        realBlock.name = RealBlock;

        GridTile tile = new GridTile(realBlock, ghostBlock, position, objectGrid.transform, tileParent.transform);
        gridTiles.Add(tile);
    }

    void DuplicateBlocks(RaycastHit hit) {
        List<GameObject> Objects = SelectObjectsAlgorithm(hit);
        DuplicateBlocks(Objects);
    }

    void DuplicateBlocks(List<GameObject> Objects) {
        for (int i = 0; i < Objects.Count; i++) {
            for (int j = 0; j < gridTiles.Count; j++) {
                GridTile tile = gridTiles[j];
                if (tile.IsEither(Objects[i])) {
                    tile.IsActive = true;
                    break;
                }
            };
        }
    }

    void DeleteBlocks(RaycastHit hit)
    {
        List<GameObject> Objects = SelectObjectsAlgorithm(hit);
        DeleteBlocks(Objects);
    }

    void DeleteBlocks(List<GameObject> Objects) {
        for (int i = 0; i < Objects.Count; i++) {
            for (int j = 0; j < gridTiles.Count; j++) {
                GridTile tile = gridTiles[j];
                if (tile.IsEither(Objects[i])) {
                    tile.IsActive = false;
                    break;
                }
            }
        }
    }

    void DeselectBlocks() {
        selectedObjects.Clear();
        for (int i = 0; i < gridTiles.Count; i++) {
            GridTile tile = gridTiles[i];
            tile.ChangeColor(spawnddBlockDefaultColor);
        }
    }

    void SelectBlocks(RaycastHit hit)
    {
        List<GameObject> Objects = SelectObjectsAlgorithm(hit);

        for(int i = 0; i < gridTiles.Count; i++) {
            GridTile tile = gridTiles[i];
            if (Objects.Contains(tile.realTile)) {
                tile.ChangeColor(Color.red);
                selectedObjects.Add(tile.realTile);
            }
        }
    }

    List<GameObject> SelectObjectsAlgorithm(RaycastHit hit)
    {
        List<GameObject> Objects = new List<GameObject>();

        if (OnClickObject == null)
            return Objects;


        float xMax = Mathf.Max(OnClickObject.transform.position.x, hit.transform.position.x);
        float xMin = Mathf.Min(OnClickObject.transform.position.x, hit.transform.position.x);

        float zMax = Mathf.Max(OnClickObject.transform.position.z, hit.transform.position.z);
        float zMin = Mathf.Min(OnClickObject.transform.position.z, hit.transform.position.z);


        var HitsInX = Physics.BoxCastAll(new Vector3(xMin, 0, zMin), new Vector3(1, 1, 1), Vector3.right, Quaternion.identity, xMax - xMin);

        foreach (var itemX in HitsInX)
        {
            if(!Objects.Contains(itemX.transform.gameObject))
                Objects.Add(itemX.transform.gameObject);

            var HitsInZ = Physics.BoxCastAll(itemX.transform.position, new Vector3(1, 1, 1), Vector3.forward, Quaternion.identity, zMax - zMin);

            foreach (var itemZ in HitsInZ)
            {
                if (!Objects.Contains(itemZ.transform.gameObject))
                    Objects.Add(itemZ.transform.gameObject);
            }
        }

        return Objects;
    }

    #endregion

    #region Public Interface
    public GridTile GetTileAtPoint(Vector3 point) {
        return gridTiles.Find(tile => tile.Position.IsAboutEqual(point));
    }
    public void ResetGrid() {
        gridTiles.ForEach(tile => {
            tile.IsActive = false;
        });
    }    

    public void ClearSelectedBlocks()
    {
        DeselectBlocks();
    }

    public List<GameObject> GetSelectedBlocks()
    {
        return selectedObjects;
    }    

    public void SetGridVisiblity(bool i_set)
    {
        displaying = i_set;
        for (int i = 0; i < gridTiles.Count; i++) {
            gridTiles[i].TryShowGhost();
        }
    }

    #endregion
}

[System.Serializable]
public class GridTile {
    public GridTile(GameObject real, GameObject ghost, Vector3 position, Transform ghostParent, Transform tileParent) {
        realTile = real;
        ghostTile = ghost;
        Position = position;
        IsActive = false;

        this.ghostParent = ghostParent;
        this.tileParent = tileParent;
        rend = realTile.GetComponent<MeshRenderer>();
        rend.GetPropertyBlock(propBlock);
        realTile.transform.SetParent(ghostParent);
        ghostTile.transform.SetParent(ghostParent);
    }
    Transform ghostParent, tileParent;
    MeshRenderer rend;
    MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
    public GameObject realTile;
    public GameObject ghostTile;
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
        propBlock.SetColor("_Color", color);
        rend.SetPropertyBlock(propBlock);
    }
}