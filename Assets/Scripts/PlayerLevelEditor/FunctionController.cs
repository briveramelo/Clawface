using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PlayerLevelEditor
{
    public class FunctionController
    {
        private IFunction currentFunction;

        private bool runBegin = false;

        public void SetFunction(IFunction i_Function)
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

}