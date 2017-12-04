using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PlayerLevelEditor
{
    public class ClickableObject : MonoBehaviour
    {
        public IMapObject ObjectType;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void UpdateThisObject()
        {
            if (ObjectType != null)
                ObjectType.Update();
        }

        public void OnClick()
        {
            if(ObjectType != null)
               ObjectType.OnClick();
        }
    }
}


