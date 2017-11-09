using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLE_Duplicate : PLE_IFunction
{

    GameObject Handle;
    GameObject sceneActiveSelection;

    private Vector3 mousePosition;

    float distance = 10.0f;


    private bool movable = false;


    public PLE_Duplicate(PLE_FunctionController Controller) : base(Controller)
    {

    }


    public override void Init()
    {
        base.Init();

        Handle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Handle.transform.position   = new Vector3(0, 0, 0);
        Handle.transform.localScale = new Vector3(3, 3, 3);
        Handle.GetComponent<Renderer>().material.color = Color.red;

    }

    public override void Update()
    {
        base.Update();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000.0f))
        {
            if (Input.GetMouseButton(0))
            {
     
                if (hit.collider.gameObject == Handle)
                {
                    Vector3 screenPoint = Camera.main.WorldToScreenPoint(Handle.transform.position);
                    mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
                    Handle.transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
                }
                else
                {
                    sceneActiveSelection = hit.collider.gameObject;
                }
            }
        }


        if(sceneActiveSelection)
        {
            PLE_Camera _camera = Camera.main.GetComponent<PLE_Camera>();
            _camera.DrawLine(Handle.transform.position, sceneActiveSelection.transform.position);
        }

    }


    public override void Release()
    {
        base.Release();

        GameObject.DestroyImmediate(Handle);
    }

}
