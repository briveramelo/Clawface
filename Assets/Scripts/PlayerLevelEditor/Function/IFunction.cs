using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PlayerLevelEditor
{
    public class IFunction
    {
        protected FunctionController m_Controller = null;
        protected GameObject UIObject;

        public IFunction(FunctionController i_Controller)
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
            UIObject = PlayerLevelEditor.UITool.FindUIGameObject(UIName);

            if (UIObject == null)
            {
                Debug.Log(UIName + "is not in Canvas");
                return;
            }

            UIObject.SetActive(true);
        }

    }

}
