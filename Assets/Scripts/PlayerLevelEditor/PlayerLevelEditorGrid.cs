using System.Collections.Generic;
using UnityEngine;
using PlayerLevelEditor;

public class PlayerLevelEditorGrid : MonoBehaviour
{
    #region Private Fields

    private Dictionary<Vector3, GameObject> mockLevelDict = new Dictionary<Vector3, GameObject>();
    private Dictionary<Vector3, GameObject> realLevelDict = new Dictionary<Vector3, GameObject>();

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
    [SerializeField] private int levelSize = 20;
    [SerializeField] private Color hoverColor = Color.blue;
    [SerializeField] private Color selectedColor = Color.red;
    [SerializeField] private LevelEditor editorInstance;

    #endregion

    #region Public Fields

    [HideInInspector] public bool displaying = false;
    [HideInInspector] public EditorMenu currentEditorMenu = EditorMenu.MAIN_EDITOR_MENU;

    #endregion



    #region Unity Lifecycle

    // Use this for initialization
    void Start()
    {
        if (EventSystem.Instance)
        {
            EventSystem.Instance.RegisterEvent(Strings.Events.INIT_EDITOR, Initilaize);      
        }     
    }

    private void OnDestroy()
    {
        if (EventSystem.Instance)
        {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.INIT_EDITOR, Initilaize);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (!displaying)
            return;

        Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000.0f))
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                OnClickObject = hit.transform.gameObject;
            }

            if(currentEditorMenu == EditorMenu.PROPS_MENU)
            {
//                DrawPreviewBlock(hit);
            }
            else if(currentEditorMenu == EditorMenu.FLOOR_MENU)
            {
                DrawPreviewBlock(hit);
                CreateBlock(hit);
            }

          
        }
    }

    #endregion

    #region Private Interface

    private void Initilaize(params object[] par)
    {
        previewBlock = Resources.Load(Strings.Editor.RESOURCE_PATH + Strings.Editor.BASIC_LE_BLOCK) as GameObject;
        spawnedBlock = Resources.Load(Strings.Editor.RESOURCE_PATH + Strings.Editor.BASIC_LVL_BLOCK) as GameObject;

        spawnddBlockDefaultColor = spawnedBlock.GetComponent<Renderer>().sharedMaterial.color;

        InitBlocks();
    }

    private void CreateBlock(RaycastHit hit)
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonUp(0))
        {
            SelectBlocks(hit);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            DuplicateBlocks(hit);
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonUp(1))
        {
            DeleteBlocks(hit);
        }
    }

    private void DrawPreviewBlock(RaycastHit hit)
    {
        #region clean up lastHoveredObjects

        foreach (GameObject GO in lastHoveredObjects)
        {
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

        if (Input.GetMouseButton(0))
        {
            List<GameObject> Objects = SelectObjectsAlgorithm(hit);

            foreach (GameObject Object in Objects)
            {
                PreviewCubeController ObjectPCC = Object.GetComponent<PreviewCubeController>();

                if (ObjectPCC)
                {
                    lastHoveredObjects.Add(Object);
                    ObjectPCC.SetColor(hoverColor);
                }
            }
        }

        #endregion

    }

    void InitBlocks()
    {
        for (int i = -levelSize; i <= levelSize; i++)
        {
            for (int j = -levelSize; j <= levelSize; j++)
            {
                GameObject instance = GameObject.Instantiate(previewBlock, new Vector3(i * 5, 0, j * 5), Quaternion.identity);
                instance.transform.SetParent(objectGrid.transform);
                mockLevelDict.Add(instance.transform.position, instance);
                instance.SetActive(false);
            }
        }
    }

    void DuplicateBlocks(RaycastHit hit)
    {
        List<GameObject> Objects = SelectObjectsAlgorithm(hit);

        foreach (GameObject Object in Objects)
        {
            if (!realLevelDict.ContainsKey(Object.transform.position))
            {
                GameObject RealObject = GameObject.Instantiate(spawnedBlock, Object.transform.position, Quaternion.identity);
                RealObject.transform.SetParent(realLevel.transform);
                realLevelDict.Add(RealObject.transform.position, RealObject);
            }

            if (mockLevelDict.ContainsKey(Object.transform.position))
            {
                mockLevelDict.Remove(Object.transform.position);
                GameObject.DestroyImmediate(Object);
            }
        }
    }

    void DeleteBlocks(RaycastHit hit)
    {
        List<GameObject> Objects = SelectObjectsAlgorithm(hit);

        foreach (GameObject Object in Objects)
        {
            if (!mockLevelDict.ContainsKey(Object.transform.position))
            {
                GameObject MockObject = GameObject.Instantiate(previewBlock, Object.transform.position, Quaternion.identity);
                MockObject.transform.SetParent(objectGrid.transform);
                mockLevelDict.Add(Object.transform.position, MockObject);
            }

            if (realLevelDict.ContainsKey(Object.transform.position))
            {
                realLevelDict.Remove(Object.transform.position);
                selectedObjects.Remove(Object);
                GameObject.DestroyImmediate(Object);
            }
        }
    }


    void SelectBlocks(RaycastHit hit)
    {
        List<GameObject> Objects = SelectObjectsAlgorithm(hit);

        if (Objects.Count > 1)
            ClearSelectedBlocks();

        foreach(GameObject Object in Objects)
        {
            if (realLevelDict.ContainsKey(Object.transform.position))
            {
                if(selectedObjects.Contains(Object))
                {
                    Object.GetComponent<Renderer>().material.color = spawnddBlockDefaultColor;
                    selectedObjects.Remove(Object);
                }
                else
                {
                    Object.GetComponent<Renderer>().material.color = Color.red;
                    selectedObjects.Add(Object);
                }
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

    public void ClearSelectedBlocks()
    {
        foreach(GameObject Go in selectedObjects)
        {
            if(Go != null)
               Go.GetComponent<Renderer>().material.color = spawnddBlockDefaultColor;
        }

        selectedObjects.Clear();
    }

    public List<GameObject> GetSelectedBlocks()
    {
        return selectedObjects;
    }

    public void DoSomeShitForSelectedObjects()
    {
        foreach(GameObject obj in selectedObjects)
        {
            if (obj == null)
                continue;

            Debug.Log("Doing Action for Object: " + obj + " at " + obj.transform.position);
            obj.GetComponent<Renderer>().material.color = Color.green;
        }

        selectedObjects.Clear();
    }

    public void SetGridVisiblity(bool i_set)
    {
        displaying = i_set;
        foreach(KeyValuePair<Vector3,GameObject> go in mockLevelDict)
        {
            if(go.Value != null)
                go.Value.SetActive(i_set);
        }
    }

    #endregion
}