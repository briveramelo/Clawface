using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PLE
{
    public abstract class IFunction
    {
        protected FunctionController m_Controller = null;

        protected List<GameObject> UIObjects;

        public IFunction(FunctionController i_Controller)
        {
            m_Controller = i_Controller;
        }

        public virtual void Init()
        {
            UIObjects = new List<GameObject>();
        }

        public virtual void Release()
        {
            if (UIObjects == null)
            {
                return;
            }

            foreach(GameObject _obj in UIObjects)
            {
                _obj.SetActive(false);
            }
        }


        public virtual void Update()
        {

        }


        protected GameObject SetUIObject(string UIName)
        {
            GameObject UIObject = PLE.UITool.FindUIGameObject(UIName);

            if (UIObject == null)
            {
                Debug.Log(UIName + "is not in Canvas");
                return null;
            }

            UIObject.SetActive(true);
            UIObjects.Add(UIObject);

            return UIObject;
        }

    }

}
