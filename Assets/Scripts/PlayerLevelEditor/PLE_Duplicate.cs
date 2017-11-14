using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PLE_Duplicate : PLE_IFunction
{

    GameObject Handle;
    GameObject sceneActiveSelection;
    Vector3[] clones;

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
        Handle.transform.position   = new Vector3(0, 10, 0);
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
            DrawBox();
        }

    }


    public override void Release()
    {
        base.Release();

        GameObject.DestroyImmediate(Handle);
    }


    void DrawBox()
    {
        Vector3 distance = Handle.transform.position - sceneActiveSelection.transform.position;

        List<Vector3> positions = new List<Vector3>();

        float dx = distance.x;
        float dy = distance.y;
        float dz = distance.z;

        int count_x = Mathf.Abs((int)(dx / PlayerLevelEditor.unitsize_x ));
        int count_y = Mathf.Abs((int)(dy / PlayerLevelEditor.unitsize_y));
        int count_z = Mathf.Abs((int)(dz / PlayerLevelEditor.unitsize_z));

        float factor_x = dx > 0 ? PlayerLevelEditor.unitsize_x : -PlayerLevelEditor.unitsize_x;
        float factor_y = dy > 0 ? PlayerLevelEditor.unitsize_y : -PlayerLevelEditor.unitsize_y;
        float factor_z = dz > 0 ? PlayerLevelEditor.unitsize_z : -PlayerLevelEditor.unitsize_z;


        for (int i = 0; i <= count_x; i++)
        {
            for (int j = 0; j <= count_y; j++)
            {
                for (int k = 0; k <= count_z; k++)
                {
                    if (i == 0 && j == 0 && k == 0) continue;

                    Vector3 new_position;

                    new_position = new Vector3(factor_x * i, factor_y * j, factor_z * k);

                    positions.Add(new_position);

                    PLE_ToolKit.ToolLib.draft(sceneActiveSelection, new_position, Color.yellow);
                }
            }
        }

        clones = positions.ToArray();
    }

}
