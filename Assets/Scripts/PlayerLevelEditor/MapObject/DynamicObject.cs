using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace PlayerLevelEditor
{
    public class DynamicObject : IMapObject
    {
        public Color OriginColor;
        public Color currentColor;


        Dropdown    DynamicTypeObject;
        Material    material;
        LevelUnit   LU;


        public DynamicObject(GameObject i_GameObject)
        {
            gameObject = i_GameObject;

            material = gameObject.GetComponent<Renderer>().material;

            OriginColor = material.color;

            LU = gameObject.GetComponent<LevelUnit>();

            if(LU != null)
            {
                material.color = Color.red;
                LU.defaultState = LevelUnitStates.floor;
            }
        }


        public override void OnClick()
        {
            if(DynamicTypeObject == null)
            {
                DynamicTypeObject = UITool.FindUIGameObject("DynamicType").GetComponent<Dropdown>();
            }

            switch (DynamicTypeObject.value)
            {
                case 1:

                    if(LU != null)
                    {
                        LU.defaultState = LevelUnitStates.floor;
                        gameObject.GetComponent<Renderer>().material.color = Color.red;
                        currentColor = Color.red;
                    }

                    return;
                case 2:

                    if (LU != null)
                    {
                        gameObject.GetComponent<Renderer>().material.color = Color.blue;
                        LU.defaultState = LevelUnitStates.pit;
                        currentColor = Color.blue;
                    }

                    return;
                case 0:


                    if (LU != null)
                    {
                        gameObject.GetComponent<Renderer>().material.color = Color.green;
                        LU.defaultState = LevelUnitStates.cover;
                        currentColor = Color.green;
                    }

                    return;

                default:
                    Debug.Log("No Option");
                    return;
            }
        }


        public override void Update()
        {
            if (DynamicTypeObject == null)
            {
                DynamicTypeObject = UITool.FindUIGameObject("DynamicType").GetComponent<Dropdown>();
            }

            if (LU == null) return;

            switch(LU.defaultState)
            {
                case LevelUnitStates.floor:
                    gameObject.GetComponent<Renderer>().material.color = Color.red;
                    return;

                case LevelUnitStates.pit:
                    gameObject.GetComponent<Renderer>().material.color = Color.blue;
                    return;
                case LevelUnitStates.cover:

                    gameObject.GetComponent<Renderer>().material.color = Color.green;
                    return;
            }
        }

        public override void Release()
        {
            Debug.Log("Release");
            material.color = OriginColor;
        }
    }

}

