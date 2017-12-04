using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerLevelEditor
{
    public class DynamicLevelSystem : ISystem
    {
        int NumStage = 3;
        Dropdown Stage;
        List<Dictionary<int, LevelUnitStates>> map;

        public DynamicLevelSystem()
        {
            Init();
        }


        public override void Init()
        {
            base.Init();

            Stage = UITool.FindUIGameObject("StageList").GetComponent<Dropdown>();

            map = new List<Dictionary<int, LevelUnitStates>>(NumStage);

            map.Add(new Dictionary<int, LevelUnitStates>());
            map.Add(new Dictionary<int, LevelUnitStates>());
            map.Add(new Dictionary<int, LevelUnitStates>());
        }


        public void UpdateObject(GameObject i_GameObject, LevelUnitStates i_State)
        {
            map[Stage.value][i_GameObject.GetInstanceID()] = i_State;
        }

        public void UpdateObject(List<GameObject> i_GameObjects)
        {
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

            
                _Object.GetComponent<ClickableObject>().UpdateThisObject();
            }
        }


    }
}


