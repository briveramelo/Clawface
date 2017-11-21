using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLE_IFunction
{
    protected PLE_FunctionController m_Controller = null;
    protected GameObject UIObject;


    public PLE_IFunction(PLE_FunctionController i_Controller)
    {
        m_Controller = i_Controller;
    }

    public virtual void Init()
    {

    }

    public virtual void Release()
    {
        if (UIObject == null)
        {
            Debug.Log("UIObject is not in Canvas");
            return;
        }

        UIObject.SetActive(false);
    }


    public virtual void Update()
    {

    }


    protected void SetUIObject(string UIName)
    {
        UIObject = PLE_ToolKit.UITool.FindUIGameObject(UIName);

        if (UIObject == null)
        {
            Debug.Log(UIName + "is not in Canvas");
            return;
        }

        UIObject.SetActive(true);
    }

}
