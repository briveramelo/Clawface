using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PLE_ToolKit;

public class PLE_Add : PLE_IFunction
{
    GameObject UIObject;
    GameObject _ex;
    GameObject _prefab;

    public Vector3 mousePositionInScene;

    public PLE_Add(PLE_FunctionController Controller) : base(Controller)
	{

    }

    public override void Init()
    {
        base.Init();

        UIObject = PLE_ToolKit.UITool.FindUIGameObject("UI_Add");

        if (UIObject == null)
            Debug.Log("UI_Add is not in Canvas");

        UIObject.SetActive(true);

        _prefab = Resources.Load("LevelEditorObjects/CommonArea/test") as GameObject;

    }


    public override void Update()
    {
        base.Update();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, 1000.0f))
            {
                mousePositionInScene = hit.point;

                Vector3 _pos = ConvertToGrid(mousePositionInScene);

                GameObject.Instantiate(_prefab, _pos, Quaternion.identity);

                Debug.Log(mousePositionInScene);
            }

        }



        //        PLE_ToolKit.ToolLib.draft(_ex, new Vector3(0, 0, 0), Color.green);
    }

    public Vector3 ConvertToGrid(Vector3 mousePositionInScene)
    {
        float width = 5.0f;
        float height = 5.0f;

        float Grid_x = Mathf.Floor((mousePositionInScene.x + width / 2) / width) * width;
        float Grid_z = Mathf.Floor((mousePositionInScene.z + height / 2) / height) * height;

        RaycastHit hit;

        if (Physics.Raycast(new Vector3(Grid_x, 1000.0f, Grid_z), Vector3.down, out hit))
        {
            return new Vector3(Grid_x, hit.point.y, Grid_z);
        }

        return new Vector3(Grid_x, mousePositionInScene.y, Grid_z);
    }

    public override void Release()
    {
        base.Release();
        UIObject.SetActive(false);
    }

}
