using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelEditorGrid : MonoBehaviour
{
    #region Private Fields

    private Dictionary<Vector3, GameObject> mockLevelDict = new Dictionary<Vector3, GameObject>();
    private Dictionary<Vector3, GameObject> realLevelDict = new Dictionary<Vector3, GameObject>();

    private GameObject previewBlock = null;
    private GameObject spawnedBlock = null;

    private GameObject OnClickObject = null;

    private List<GameObject> SelectedObjects = new List<GameObject>();

    private bool inputGuard = false;

    private GameObject currentHoveredObject = null;
    private GameObject lastHoveredObject = null;

    private GameObject currentlySelectedObject = null;

    #endregion

    #region Unity Serialized Fields

    [SerializeField] GameObject objectGrid;
    [SerializeField] GameObject realLevel;
    [SerializeField] private int levelSize = 20;
    [SerializeField] private Color hoverColor = Color.blue;
    [SerializeField] private Color selectedColor = Color.red;

    #endregion

    #region Public Fields

    [HideInInspector] public bool displaying = false;

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
        if (displaying)
        {
            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000.0f))
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
        //lastHoveredObject = new GameObject();
        InitBlocks();

    }

    private void CreateBlock(RaycastHit hit)
    {

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            OnClickObject = hit.transform.gameObject;
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonUp(0))
        {
            SelectBlocks(hit);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            DuplicateBlocks(hit);
        }


        if (Input.GetMouseButtonUp(1))
        {
            DeleteBlocks(hit);
        }
    }

    private void DrawPreviewBlock(RaycastHit hit)
    {

        currentHoveredObject = hit.transform.gameObject;

        PreviewCubeController currentPCC = currentHoveredObject.GetComponent<PreviewCubeController>();
        PreviewCubeController lastPcc = null;
        if (lastHoveredObject != null)
        {
            lastPcc = lastHoveredObject.GetComponent<PreviewCubeController>();
        }

        if (currentHoveredObject != lastHoveredObject)
        {

            if (lastPcc != null && lastPcc.selected == false)
            {
                lastPcc.ResetColor();
            }
        }

        if (currentPCC)
        {
            currentPCC.SetColor(hoverColor);
        }

        lastHoveredObject = currentHoveredObject;
              
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
                RealObject.transform.gameObject.GetComponent<Renderer>().material.color = Color.green;
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
                GameObject.DestroyImmediate(Object);
            }

        }
    }


    void SelectBlocks(RaycastHit hit)
    {
        List<GameObject> Objects = SelectObjectsAlgorithm(hit);

        foreach(GameObject Object in Objects)
        {
            if (realLevelDict.ContainsKey(Object.transform.position) && !SelectedObjects.Contains(Object.transform.gameObject))
            {
                realLevelDict[Object.transform.position].GetComponent<Renderer>().material.color = Color.red;
                SelectedObjects.Add(realLevelDict[Object.transform.position]);
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

    public void DoSomeShitForSelectedObjects()
    {
        foreach(GameObject obj in SelectedObjects)
        {
            if (obj == null)
                continue;

            Debug.Log("Doing Action for Object: " + obj + " at " + obj.transform.position);
            obj.GetComponent<Renderer>().material.color = Color.green;
        }

        SelectedObjects.Clear();
    }

    public void SetGridVisiblity(bool i_set)
    {
        foreach(KeyValuePair<Vector3,GameObject> go in mockLevelDict)
        {
            go.Value.SetActive(i_set);
        }
    }

    #endregion
}