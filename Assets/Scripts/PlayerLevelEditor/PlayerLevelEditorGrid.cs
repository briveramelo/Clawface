using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelEditorGrid : MonoBehaviour
{
    [SerializeField]
    GameObject MockLevel;

    [SerializeField]
    GameObject RealLevel;

    Dictionary<Vector3, GameObject> MockLevelDict = new Dictionary<Vector3, GameObject>();
    Dictionary<Vector3, GameObject> RealLevelDict = new Dictionary<Vector3, GameObject>();

    GameObject Prefab = null;
    GameObject GreenBlock = null;

    GameObject OnClickObject = null;

    List<GameObject> SelectedObjects = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        Prefab = Resources.Load(Strings.Editor.RESOURCE_PATH + Strings.Editor.BASIC_LE_BLOCK) as GameObject;
        GreenBlock = Resources.Load(Strings.Editor.RESOURCE_PATH + Strings.Editor.BASIC_LVL_BLOCK) as GameObject;

        if (MockLevel == null)
        {
            Debug.LogError("MockLevel is null, create a new one");
            GameObject newObject = new GameObject();
            MockLevel = newObject;
        }

        if(RealLevel == null)
        {
            Debug.LogError("MockLevel is null, create a new one");
            GameObject newObject = new GameObject();
            RealLevel = newObject;
        }

        InitBlocks();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000.0f))
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
    }

    void InitBlocks()
    {
        for (int i = -20; i <= 20; i++)
        {
            for (int j = -20; j <= 20; j++)
            {
                GameObject instance = GameObject.Instantiate(Prefab, new Vector3(i * 5, 0, j * 5), Quaternion.identity);
                instance.transform.SetParent(MockLevel.transform);
                MockLevelDict.Add(instance.transform.position, instance);
            }
        }
    }

    void DuplicateBlocks(RaycastHit hit)
    {
        List<GameObject> Objects = SelectObjectsAlgorithm(hit);

        foreach (GameObject Object in Objects)
        {
            if (!RealLevelDict.ContainsKey(Object.transform.position))
            {
                GameObject RealObject = GameObject.Instantiate(GreenBlock, Object.transform.position, Quaternion.identity);
                RealObject.transform.SetParent(RealLevel.transform);
                RealObject.transform.gameObject.GetComponent<Renderer>().material.color = Color.green;
                RealLevelDict.Add(RealObject.transform.position, RealObject);
            }

            if (MockLevelDict.ContainsKey(Object.transform.position))
            {
                MockLevelDict.Remove(Object.transform.position);
                GameObject.DestroyImmediate(Object);
            }
        }
    }

    void DeleteBlocks(RaycastHit hit)
    {
        List<GameObject> Objects = SelectObjectsAlgorithm(hit);

        foreach (GameObject Object in Objects)
        {
            if (!MockLevelDict.ContainsKey(Object.transform.position))
            {
                GameObject MockObject = GameObject.Instantiate(Prefab, Object.transform.position, Quaternion.identity);
                MockObject.transform.SetParent(MockLevel.transform);
                MockLevelDict.Add(Object.transform.position, MockObject);
            }

            if (RealLevelDict.ContainsKey(Object.transform.position))
            {
                RealLevelDict.Remove(Object.transform.position);
                GameObject.DestroyImmediate(Object);
            }

        }
    }


    void SelectBlocks(RaycastHit hit)
    {
        List<GameObject> Objects = SelectObjectsAlgorithm(hit);

        foreach(GameObject Object in Objects)
        {
            if (RealLevelDict.ContainsKey(Object.transform.position) && !SelectedObjects.Contains(Object.transform.gameObject))
            {
                RealLevelDict[Object.transform.position].GetComponent<Renderer>().material.color = Color.red;
                SelectedObjects.Add(RealLevelDict[Object.transform.position]);
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
}