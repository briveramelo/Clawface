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

    GameObject OnClickObject = null;

	// Use this for initialization
	void Start ()
    {
        if(MockLevel == null)
        {
            Debug.LogError("MockLevel is null");
        }

        foreach (Transform child in MockLevel.transform)
        {
            MockLevelDict.Add(child.transform.position, child.gameObject);      
        }

        if (MockLevel == null)
        {
            Debug.LogError("MockLevel is null");
        }


        Debug.Log(MockLevelDict[new Vector3(0, 0, 0)]);

        //var hits = Physics.BoxCastAll(Vector3.zero, new Vector3(1, 1, 1), Vector3.right, Quaternion.identity, 10000);
        //Debug.Log("Hits length: " + hits.Length);

        //foreach (var item in hits)
        //{
        //    Debug.Log(item.transform.name);
        //    Debug.Log(item.transform.position);
        //}


    }
	
	// Update is called once per frame
	void Update ()
    {
        Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000.0f))
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Click: ");
                Debug.Log(hit.transform.position);

                OnClickObject = hit.transform.gameObject;
            }

            if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("Release: ");
                Debug.Log(hit.transform.position);

                float xMax = Mathf.Max(OnClickObject.transform.position.x, hit.transform.position.x);
                float xMin = Mathf.Min(OnClickObject.transform.position.x, hit.transform.position.x);

                float zMax = Mathf.Max(OnClickObject.transform.position.z, hit.transform.position.z);
                float zMin = Mathf.Min(OnClickObject.transform.position.z, hit.transform.position.z);


                var HitsInX = Physics.BoxCastAll(new Vector3(xMin, 0, zMin), new Vector3(1, 1, 1), Vector3.right, Quaternion.identity, xMax - xMin);

                foreach (var itemX in HitsInX)
                {
                    Debug.Log(itemX.transform.position);
                    itemX.transform.gameObject.GetComponent<Renderer>().material.color = Color.green;

                    var HitsInZ = Physics.BoxCastAll(itemX.transform.position, new Vector3(1, 1, 1), Vector3.forward, Quaternion.identity, zMax - zMin);

                    foreach (var itemZ in HitsInZ)
                    {
                        itemZ.transform.gameObject.GetComponent<Renderer>().material.color = Color.green;
                    }
                }


            }
        }
    }
}
