using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PLE_ToolKit;

public class PLE_Add : PLE_IFunction
{
    GameObject UIObject;
    GameObject _prefab;

    Button Btn_Add;

    public Vector3 mousePositionInScene;

    private bool clickToAddEnabled = false;


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

        Btn_Add = PLE_ToolKit.UITool.GetUIComponent<Button>("Button_Add");

        if (Btn_Add == null)
            Debug.Log("Btn_Add is null");

        Btn_Add.onClick.AddListener(() => EnableAdd(Btn_Add));



        _prefab = Resources.Load("LevelEditorObjects/CommonArea/test") as GameObject;
    }


    public override void Update()
    {
        base.Update();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000.0f))
        {
            mousePositionInScene = hit.point;

            if (Input.GetMouseButtonDown(0) && clickToAddEnabled)
            {
                Vector3 _pos = PLE_ToolKit.ToolLib.ConvertToGrid(mousePositionInScene);

                GameObject.Instantiate(_prefab, _pos, Quaternion.identity);
            }
        }

        PLE_ToolKit.ToolLib.draft(_prefab, PLE_ToolKit.ToolLib.ConvertToGrid(mousePositionInScene - _prefab.transform.position), Color.green);
    }

    public override void Release()
    {
        base.Release();
        UIObject.SetActive(false);
    }


    public void EnableAdd(Button thisBtn)
    {
        clickToAddEnabled = !clickToAddEnabled;
        thisBtn.image.color = clickToAddEnabled ? Color.red : Color.white;
    }

}
