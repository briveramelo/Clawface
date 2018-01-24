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
    GameObject OnClickObject = null;

    List<GameObject> SelectedObjects = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        Prefab = Resources.Load(Strings.Editor.RESOURCE_PATH + Strings.Editor.BASIC_LVL_BLOCK) as GameObject;

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
            if (Input.GetMouseButtonDown(0))
            {
                OnClickObject = hit.transform.gameObject;
            }

            if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
            {
                SelectBlocks(hit);
            }     

            if (Input.GetMouseButtonUp(0))
            {
                DuplicateBlocks(hit);
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
        float xMax = Mathf.Max(OnClickObject.transform.position.x, hit.transform.position.x);
        float xMin = Mathf.Min(OnClickObject.transform.position.x, hit.transform.position.x);

        float zMax = Mathf.Max(OnClickObject.transform.position.z, hit.transform.position.z);
        float zMin = Mathf.Min(OnClickObject.transform.position.z, hit.transform.position.z);

        var HitsInX = Physics.BoxCastAll(new Vector3(xMin, 0, zMin), new Vector3(1, 1, 1), Vector3.right, Quaternion.identity, xMax - xMin);

        foreach (var itemX in HitsInX)
        {
            if (!RealLevelDict.ContainsKey(itemX.transform.position))
            {
                GameObject RealObject = GameObject.Instantiate(Prefab, itemX.transform.position, Quaternion.identity);
                RealObject.transform.SetParent(RealLevel.transform);
                RealObject.transform.gameObject.GetComponent<Renderer>().material.color = Color.green;
                RealLevelDict.Add(itemX.transform.position, RealObject);
            }

            var HitsInZ = Physics.BoxCastAll(itemX.transform.position, new Vector3(1, 1, 1), Vector3.forward, Quaternion.identity, zMax - zMin);

            foreach (var itemZ in HitsInZ)
            {
                if (!RealLevelDict.ContainsKey(itemZ.transform.position))
                {
                    GameObject RealObject = GameObject.Instantiate(Prefab, itemZ.transform.position, Quaternion.identity);
                    RealObject.transform.SetParent(RealLevel.transform);
                    RealObject.transform.gameObject.GetComponent<Renderer>().material.color = Color.green;
                    RealLevelDict.Add(itemZ.transform.position, RealObject);

                    if (MockLevelDict.ContainsKey(itemZ.transform.position))
                    {
                        MockLevelDict.Remove(itemZ.transform.position);
                        GameObject.DestroyImmediate(itemZ.transform.gameObject);
                    }

                }
            }

            if (MockLevelDict.ContainsKey(itemX.transform.position))
            {
                MockLevelDict.Remove(itemX.transform.position);
                GameObject.DestroyImmediate(itemX.transform.gameObject);
            }

        }
    }

    void SelectBlocks(RaycastHit hit)
    {
        float xMax = Mathf.Max(OnClickObject.transform.position.x, hit.transform.position.x);
        float xMin = Mathf.Min(OnClickObject.transform.position.x, hit.transform.position.x);

        float zMax = Mathf.Max(OnClickObject.transform.position.z, hit.transform.position.z);
        float zMin = Mathf.Min(OnClickObject.transform.position.z, hit.transform.position.z);

        var HitsInX = Physics.BoxCastAll(new Vector3(xMin, 0, zMin), new Vector3(1, 1, 1), Vector3.right, Quaternion.identity, xMax - xMin);

        foreach (var itemX in HitsInX)
        {
            if (RealLevelDict.ContainsKey(itemX.transform.position) && !SelectedObjects.Contains(itemX.transform.gameObject))
            {
                RealLevelDict[itemX.transform.position].GetComponent<Renderer>().material.color = Color.red;
                SelectedObjects.Add(RealLevelDict[itemX.transform.position]);
            }

            var HitsInZ = Physics.BoxCastAll(itemX.transform.position, new Vector3(1, 1, 1), Vector3.forward, Quaternion.identity, zMax - zMin);

            foreach (var itemZ in HitsInZ)
            {
                if (RealLevelDict.ContainsKey(itemZ.transform.position) && !SelectedObjects.Contains(itemZ.transform.gameObject))
                {
                    RealLevelDict[itemZ.transform.position].GetComponent<Renderer>().material.color = Color.red;
                    SelectedObjects.Add(RealLevelDict[itemZ.transform.position]);
                }
            }
        }
    }

    public void DoSomeShitForSelectedObjects()
    {
        foreach(GameObject obj in SelectedObjects)
        {
            Debug.Log("Doing Action for Object: " + obj + " at " + obj.transform.position);
            obj.GetComponent<Renderer>().material.color = Color.green;
        }

        SelectedObjects.Clear();
    }

}