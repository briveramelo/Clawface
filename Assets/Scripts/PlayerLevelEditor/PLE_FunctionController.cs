using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLE_FunctionController
{

    private PLE_IFunction currentFunction;

    private bool runBegin = false;

    public void SetFunction(PLE_IFunction i_Function)
    {
        runBegin = false;

        if (currentFunction != null)
            currentFunction.Release();

        currentFunction = i_Function;
    }


    public void Update()
    {
        if (currentFunction != null && runBegin == false)
        {
            currentFunction.Init();
            runBegin = true;
        }


        if (currentFunction != null)
            currentFunction.Update();
    }

}
