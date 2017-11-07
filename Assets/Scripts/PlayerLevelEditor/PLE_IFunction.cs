using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLE_IFunction
{
    protected PLE_FunctionController m_Controller = null;

    public PLE_IFunction(PLE_FunctionController i_Controller)
    {
        m_Controller = i_Controller;
    }

    public virtual void Init()
    {

    }

    public virtual void Release()
    {

    }


    public virtual void Update()
    {

    }

}
