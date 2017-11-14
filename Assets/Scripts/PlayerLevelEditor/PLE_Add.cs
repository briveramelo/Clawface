using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using PLE_ToolKit;

public class PLE_Add : PLE_IFunction
{
    GameObject _prefab;

    Button  Btn_Add;
    Button  Btn_Delete;

    UnityAction ACT_Add;
    UnityAction ACT_Delete;


    public Vector3 mousePositionInScene;

    private bool clickToAddEnabled = false;
    private bool clickToDeleteEnabled = false;

    public PLE_Add(PLE_FunctionController Controller) : base(Controller)
	{

    }

    public override void Init()
    {
        base.Init();

        SetUIObject("UI_Add");

        ACT_Add = () => EnableAdd(Btn_Add);
        Btn_Add = PLE_ToolKit.UITool.GetUIComponent<Button>("Button_Add");
        if (Btn_Add == null) Debug.Log("Btn_Add is null");
  
        Btn_Add.onClick.AddListener(ACT_Add);
        Btn_Add.image.color = Color.white;


        ACT_Delete = () => EnableDelete(Btn_Delete);
        Btn_Delete = PLE_ToolKit.UITool.GetUIComponent<Button>("Button_Delete");
        if (Btn_Delete == null) Debug.Log("Btn_Delete is null");

        Btn_Delete.onClick.AddListener(ACT_Delete);
        Btn_Delete.image.color = Color.white;


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

            //Right Click
            if(Input.GetMouseButtonDown(1) && clickToDeleteEnabled)
            {
                GameObject.DestroyImmediate(hit.collider.gameObject);
            }

        }

        PLE_ToolKit.ToolLib.draft(_prefab, PLE_ToolKit.ToolLib.ConvertToGrid(mousePositionInScene - _prefab.transform.position), Color.green);
    }

    public override void Release()
    {
        base.Release();
        Btn_Add.onClick.RemoveListener(ACT_Add);
    }


    public void EnableAdd(Button thisBtn)
    {
        clickToAddEnabled = !clickToAddEnabled;
        thisBtn.image.color = clickToAddEnabled ? Color.red : Color.white;
    }


    public void EnableDelete(Button thisBtn)
    {
        clickToDeleteEnabled = !clickToDeleteEnabled;
        thisBtn.image.color = clickToDeleteEnabled ? Color.red : Color.white;
    }

}
