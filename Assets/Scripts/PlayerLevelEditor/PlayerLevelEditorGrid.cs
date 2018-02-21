﻿using System.Collections.Generic;
using UnityEngine;
using PlayerLevelEditor;
using ModMan;
using System.Linq;

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
    [SerializeField] GameObject wallPrefab;
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
    void Update()
    {
        if (!displaying)
            return;

        if (MouseHelper.hitItem) {
            RaycastHit hit = MouseHelper.raycastHit;
            if (Input.GetMouseButtonDown(MouseButtons.LEFT) || Input.GetMouseButtonDown(MouseButtons.RIGHT)) {                
                OnClickObject = hit.transform.gameObject;
            }

            if (currentEditorMenu == EditorMenu.PROPS_MENU) {
                //DrawPreviewBlock(hit);
            }
            else if (currentEditorMenu == EditorMenu.FLOOR_MENU) {
                DrawPreviewBlock(hit);
                HandleBlockInteractions(hit);
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

    private void HandleBlockInteractions(RaycastHit hit) {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonUp(MouseButtons.LEFT)) {
            SelectBlocks(hit);
        }
        else if (Input.GetMouseButtonUp(MouseButtons.LEFT) && !Input.GetKey(KeyCode.LeftAlt)) {
            DeselectBlocks();
            DuplicateBlocks(hit);
        }

        if (Input.GetMouseButtonUp(MouseButtons.RIGHT) && !Input.GetKey(KeyCode.LeftAlt)) {
            DeselectBlocks();
            DeleteBlocks(hit);
        }
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

        GameObject wall_N = GameObject.Instantiate(wallPrefab, position + Vector3.up * 5f + Vector3.forward * 2.5f, Quaternion.Euler(0f, 0f, 0f));
        GameObject wall_E = GameObject.Instantiate(wallPrefab, position + Vector3.up * 5f + Vector3.right * 2.5f, Quaternion.Euler(0f, 90f, 0f));
        GameObject wall_W = GameObject.Instantiate(wallPrefab, position + Vector3.up * 5f + Vector3.left * 2.5f, Quaternion.Euler(0f, 270f, 0f));
        GameObject wall_S = GameObject.Instantiate(wallPrefab, position + Vector3.up * 5f + Vector3.back * 2.5f, Quaternion.Euler(0f, 180f, 0f));

        GridTile tile = new GridTile(realBlock, ghostBlock, position, objectGrid.transform, tileParent.transform, wall_N, wall_E, wall_W, wall_S);
        gridTiles.Add(tile);
    }

    public void ShowWalls() {
        gridTiles.ForEach(tile => tile.EnableWalls());
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

    void DeleteBlocks(RaycastHit hit) {
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
        gridTiles.ForEach(tile => { tile.ChangeColor(spawnddBlockDefaultColor); });
    }

    void SelectBlocks(RaycastHit hit) {
        List<GameObject> Objects = SelectObjectsAlgorithm(hit);
        gridTiles.ForEach(tile => {
            if (Objects.Contains(tile.realTile)) {
                tile.ChangeColor(Color.red);
                selectedObjects.Add(tile.realTile);
            }
        });
    }

    List<GameObject> SelectObjectsAlgorithm(RaycastHit hit) {
        List<GameObject> Objects = new List<GameObject>();

        if (OnClickObject == null) {
            return Objects;
        }
        //if (Vector3.Distance(OnClickObject.transform.position, hit.transform.position)<2.5f) {
        //    if (!Objects.Contains(OnClickObject)) {
        //        Objects.Add(OnClickObject);
        //    }
        //    return Objects;
        //}


        float xMax = Mathf.Max(OnClickObject.transform.position.x, hit.transform.position.x);
        float xMin = Mathf.Min(OnClickObject.transform.position.x, hit.transform.position.x);

        float zMax = Mathf.Max(OnClickObject.transform.position.z, hit.transform.position.z);
        float zMin = Mathf.Min(OnClickObject.transform.position.z, hit.transform.position.z);

        float y = OnClickObject.transform.position.y;        


        var HitsInX = Physics.BoxCastAll(new Vector3(xMin, y, zMin), new Vector3(1, 40, 1), Vector3.right, Quaternion.identity, xMax - xMin);
        
        foreach (var itemX in HitsInX) {
            if(!Objects.Contains(itemX.transform.gameObject))
                Objects.Add(itemX.transform.gameObject);

            var HitsInZ = Physics.BoxCastAll(itemX.transform.position , new Vector3(1, 40, 1), Vector3.forward, Quaternion.identity, zMax - zMin);

            foreach (var itemZ in HitsInZ) {
                if (!Objects.Contains(itemZ.transform.gameObject)) {
                    Objects.Add(itemZ.transform.gameObject);
                }
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

    public void ClearSelectedBlocks() {
        DeselectBlocks();
    }

    public List<GameObject> GetSelectedBlocks() { return selectedObjects; }    

    public void SetGridVisiblity(bool i_set) {
        displaying = i_set;
        gridTiles.ForEach(tile => { tile.TryShowGhost(); });
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
        rend = realTile.GetComponent<MeshRenderer>();
        rend.GetPropertyBlock(propBlock);
        realTile.transform.SetParent(ghostParent);
        ghostTile.transform.SetParent(ghostParent);

        wall_N.transform.SetParent(ghostParent);
        wall_E.transform.SetParent(ghostParent);
        wall_W.transform.SetParent(ghostParent);
        wall_S.transform.SetParent(ghostParent);
    }
    Transform ghostParent, tileParent;
    MeshRenderer rend;
    MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
    public GameObject realTile;
    public GameObject ghostTile;
    public GameObject wall_N, wall_E, wall_W, wall_S;
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

    public void EnableWalls() {
        bool blockNorth = false;
        bool blockEast = false;
        bool blockWest = false;
        bool blockSouth = false;

        if (IsActive) {
            blockNorth = Physics.OverlapSphere(realTile.transform.position + Vector3.forward * 5f, .2f).ToList().Exists(item => item.GetComponent<PLEBlockUnit>());
            blockEast = Physics.OverlapSphere(realTile.transform.position + Vector3.right * 5f, .2f).ToList().Exists(item => item.GetComponent<PLEBlockUnit>());
            blockWest = Physics.OverlapSphere(realTile.transform.position + Vector3.left * 5f, .2f).ToList().Exists(item => item.GetComponent<PLEBlockUnit>());
            blockSouth = Physics.OverlapSphere(realTile.transform.position + Vector3.back * 5f, .2f).ToList().Exists(item => item.GetComponent<PLEBlockUnit>());
        }

        wall_N.SetActive(IsActive && !blockNorth);
        wall_E.SetActive(IsActive && !blockEast);
        wall_W.SetActive(IsActive && !blockWest);
        wall_S.SetActive(IsActive && !blockSouth);
    }
}