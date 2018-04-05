using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace PLE
{
    public class DynamicObject : IMapObject
    {
        Dropdown    DynamicTypeObject;
        Material    m_Material;
        LevelUnit   m_LevelUnit;


        public DynamicObject(GameObject i_GameObject)
        {
            gameObject  = i_GameObject;
            m_Material  = gameObject.GetComponent<Renderer>().material;
            m_LevelUnit = gameObject.GetComponent<LevelUnit>();

            if(m_LevelUnit != null)
            {
                m_Material.color = Color.red;
                m_LevelUnit.defaultState = LevelUnitStates.Floor;
            }
        }

        public override void OnClick()
        {
            if(DynamicTypeObject == null)
            {
                DynamicTypeObject = UITool.FindUIGameObject("DynamicType").GetComponent<Dropdown>();
            }

            if (m_LevelUnit == null) return;

            switch (DynamicTypeObject.value)
            {
                case 0:
                    m_LevelUnit.defaultState = LevelUnitStates.Cover;
                    m_Material.color = Color.green;
                    return;

                case 1:
                    m_LevelUnit.defaultState = LevelUnitStates.Floor;
                    m_Material.color = Color.red;
                    return;

                case 2:
                    m_LevelUnit.defaultState = LevelUnitStates.Pit;
                    m_Material.color = Color.blue;
                    return;

                default:
                    return;
            }
        }


        public override void Update()
        {
            if (DynamicTypeObject == null)
            {
                DynamicTypeObject = UITool.FindUIGameObject("DynamicType").GetComponent<Dropdown>();
            }

            if (m_LevelUnit == null) return;

            switch(m_LevelUnit.defaultState)
            {
                case LevelUnitStates.Floor:
                    m_Material.color = Color.red;
                    return;

                case LevelUnitStates.Pit:
                    m_Material.color = Color.blue;
                    return;

                case LevelUnitStates.Cover:
                    m_Material.color = Color.green;
                    return;
            }
        }
    }

}

