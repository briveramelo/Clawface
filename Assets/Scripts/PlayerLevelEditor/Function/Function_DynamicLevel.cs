using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace PlayerLevelEditor
{
    public class DynamicLevel : IFunction
    {
        GameObject LevelObject;
        Dropdown Stage;
        Dropdown DynamicType;

        static List<Dictionary<int, LevelUnitStates>> map;


        public DynamicLevel(FunctionController Controller) : base(Controller)
        {

        }


        public override void Init()
        {
            base.Init();

            DynamicType = SetUIObject("DynamicType").GetComponent<Dropdown>();

            Stage = SetUIObject("StageList").GetComponent<Dropdown>();
            Stage.onValueChanged.AddListener(delegate { UpdateMapObjects(Stage); });
            if(Stage == null)
            {
                Debug.LogWarning("Stage NULL");
            }


            LevelObject = UnityTool.FindGameObject("LEVEL");

            if(LevelObject == null)
            {
                Debug.LogWarning("LevelObject NULL");
            }


            InitMapObjects();
        }

        public override void Update()
        {
            base.Update();

            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;


            if(Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log(map[Stage.value].Count);

                foreach (var v in map[Stage.value])
                {
                    Debug.Log(v.Value);
                }
            }


            if (Physics.Raycast(ray, out hit, 1000.0f) && Input.GetMouseButtonUp(0))
            {
                ClickableObject CampClickScript = hit.transform.gameObject.GetComponent<ClickableObject>();

                if (CampClickScript != null)
                {
                    CampClickScript.OnClick();
                    map[Stage.value][hit.transform.gameObject.GetInstanceID()] = (LevelUnitStates)DynamicType.value;
                    return;
                }
                else
                {
                    Debug.Log("NULL");
                }
            }
        }

        public override void Release()
        {
            base.Release();

            ReleaseMapObjects();
        }

        public void InitMapObjects()
        {
            if(map == null)
            {
                map = new List<Dictionary<int, LevelUnitStates>>();

                map.Add(new Dictionary<int, LevelUnitStates>());
                map.Add(new Dictionary<int, LevelUnitStates>());
                map.Add(new Dictionary<int, LevelUnitStates>());
            }

            UpdateMapObjects(Stage);
        }

        public void UpdateMapObjects(Dropdown dd)
        {
            foreach (Transform child in LevelObject.transform)
            {
                if (map[dd.value].ContainsKey(child.gameObject.GetInstanceID()) == false)
                {

                    if (child.gameObject.GetComponent<LevelUnit>() == null)
                        continue;

                    int key = child.gameObject.GetInstanceID();
                    map[dd.value].Add(key, LevelUnitStates.floor);

                    child.gameObject.GetComponent<ClickableObject>().ObjectType = new DynamicObject(child.gameObject);
                }
                else
                {
                    if (child.gameObject.GetComponent<LevelUnit>())
                    {
                        child.gameObject.GetComponent<LevelUnit>().defaultState = map[dd.value][child.gameObject.GetInstanceID()];
                    }
                }


                child.gameObject.GetComponent<ClickableObject>().UpdateThisObject();
            }

        }
        public void ReleaseMapObjects()
        {
            foreach (Transform child in LevelObject.transform)
            {
                child.gameObject.GetComponent<ClickableObject>().Release();
            }
        }
    }

}

