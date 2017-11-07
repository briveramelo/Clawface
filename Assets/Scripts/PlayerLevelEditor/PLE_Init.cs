using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLE_Init : PLE_IFunction
{
    GameObject UIObject;

    public PLE_Init(PLE_FunctionController Controller) : base(Controller)
	{

    }


    public override void Init()
    {
        base.Init();

        UIObject = PLE_ToolKit.UITool.FindUIGameObject("UI_Init");

        if (UIObject == null)
            Debug.Log("UI_Init is not in Canvas");

        UIObject.SetActive(true);
    }


    public override void Release()
    {
        base.Release();
        UIObject.SetActive(false);
    }

}
