using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerLevelEditor
{
    public class DynamicLevelSystem : ISystem
    {
        Dropdown Stage;

        List<Dictionary<int, LevelUnitStates>> map;

        List<GameObject> m_GameObjects;


        public DynamicLevelSystem()
        {
            Init();
        }


        public override void Init()
        {
            base.Init();

            Stage = UITool.FindUIGameObject("StageList").GetComponent<Dropdown>();

            m_GameObjects = new List<GameObject>();

            map = new List<Dictionary<int, LevelUnitStates>>(Stage.options.Count);

            map.Add(new Dictionary<int, LevelUnitStates>());
            map.Add(new Dictionary<int, LevelUnitStates>());
            map.Add(new Dictionary<int, LevelUnitStates>());
        }


        public void IniteObjectState(List<GameObject> i_GameObjects)
        {
            foreach (GameObject _Object in i_GameObjects)
            {
                if (_Object.GetComponent<LevelUnit>() == null)
                    continue;

                _Object.GetComponent<ClickableObject>().ObjectType = new DynamicObject(_Object);

                for (int i = 0; i < Stage.options.Count; i++)
                {
                    if (map[i].ContainsKey(_Object.GetInstanceID()) == false)
                    {
                        map[i].Add(_Object.GetInstanceID(), LevelUnitStates.floor);
                    }
                }
            }
        }


        public void SaveObjectState(GameObject i_GameObject, LevelUnitStates i_State)
        {
            map[Stage.value][i_GameObject.GetInstanceID()] = i_State;
        }

        public void UpdateObjectState(List<GameObject> i_GameObjects)
        {
            m_GameObjects.Clear();

            foreach (GameObject _Object in i_GameObjects)
            {
                if (_Object.GetComponent<LevelUnit>() == null)
                    continue;

                if (map[Stage.value].ContainsKey(_Object.GetInstanceID()) == false)
                {
                    int key = _Object.GetInstanceID();
                    map[Stage.value].Add(key, LevelUnitStates.floor);

                    _Object.GetComponent<ClickableObject>().ObjectType = new DynamicObject(_Object);
                }
                else
                {
                    _Object.GetComponent<LevelUnit>().defaultState = map[Stage.value][_Object.GetInstanceID()];
                }

                m_GameObjects.Add(_Object);

                _Object.GetComponent<ClickableObject>().UpdateThisObject();
            }
        }


        public void RegisterEvent()
        {
            for (int j = 0; j < m_GameObjects.Count; j++)
            {
                LevelUnit LU = m_GameObjects[j].GetComponent<LevelUnit>();

                if(LU)
                    LU.DeRegisterFromEvents();
            }

            for (int j = 0; j < m_GameObjects.Count; j++)
            {
                LevelUnit LU = m_GameObjects[j].GetComponent<LevelUnit>();

                if (LU == null) continue;

                for (int i = 0; i < Stage.options.Count; i++)
                {
                    string event_name = "PLE_TEST_WAVE_" + i.ToString();

                    LevelUnitStates state = map[i][m_GameObjects[j].GetInstanceID()];

                    switch (state)
                    {
                        case LevelUnitStates.cover:
                            LU.AddCoverStateEvent(event_name);
                            break;

                        case LevelUnitStates.floor:
                            LU.AddFloorStateEvent(event_name);
                            break;

                        case LevelUnitStates.pit:
                            LU.AddPitStateEvent(event_name);
                            break;
                    }
                }

                LU.RegisterToEvents();
            }
        }


        public void DeRegisterEvent()
        {
            for (int j = 0; j < m_GameObjects.Count; j++)
            {
                LevelUnit LU = m_GameObjects[j].GetComponent<LevelUnit>();

                if(LU)
                   LU.DeRegisterFromEvents();
            }

            Release();
        }


        public override void Release()
        {
            for (int j = 0; j < m_GameObjects.Count; j++)
            {
                LevelUnit LU = m_GameObjects[j].GetComponent<LevelUnit>();
                LU.HideBlockingObject();
            }
        }

    }
}


