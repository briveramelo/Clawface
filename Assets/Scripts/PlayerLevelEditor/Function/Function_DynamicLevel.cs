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

        List<KeyValuePair<GameObject, Color>> Objects;


        public DynamicLevel(FunctionController Controller) : base(Controller)
        {

        }


        public override void Init()
        {
            base.Init();

            DynamicType = SetUIObject("DynamicType").GetComponent<Dropdown>();

            Stage = SetUIObject("StageList").GetComponent<Dropdown>();
            Stage.onValueChanged.AddListener(delegate { UpdateMapObjects(Stage); });

            LevelObject = UnityTool.FindGameObject("LEVEL");

            if(LevelObject == null)
            {
                Debug.LogWarning("LevelObject NULL");
            }

            Objects = new List<KeyValuePair<GameObject, Color>>();

            foreach (Transform child in LevelObject.transform)
            {          
                Objects.Add(new KeyValuePair<GameObject, Color>(child.gameObject, child.gameObject.GetComponent<Renderer>().material.color));
            }


            InitMapObjects();
        }

        public override void Update()
        {
            base.Update();

            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000.0f) && Input.GetMouseButtonUp(0))
            {
                ClickableObject CampClickScript = hit.transform.gameObject.GetComponent<ClickableObject>();

                if (CampClickScript != null)
                {
                    CampClickScript.OnClick();
                    map[Stage.value][hit.transform.gameObject.GetInstanceID()] = (LevelUnitStates)DynamicType.value;
                    return;
                }
            }
        }

        public override void Release()
        {
            base.Release();

            Stage.onValueChanged.RemoveAllListeners();

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

        public void UpdateMapObjects(Dropdown stage)
        {
            Debug.Log("Update");

            foreach (Transform child in LevelObject.transform)
            {
                if (child.gameObject.GetComponent<LevelUnit>() == null)
                    continue;

                if (map[stage.value].ContainsKey(child.gameObject.GetInstanceID()) == false)
                {
                    int key = child.gameObject.GetInstanceID();
                    map[stage.value].Add(key, LevelUnitStates.floor);

                    child.gameObject.GetComponent<ClickableObject>().ObjectType = new DynamicObject(child.gameObject);
                }
                else
                {
                    child.gameObject.GetComponent<LevelUnit>().defaultState = map[stage.value][child.gameObject.GetInstanceID()];
                }


                child.gameObject.GetComponent<ClickableObject>().UpdateThisObject();
            }

        }
        public void ReleaseMapObjects()
        {
            foreach (var _obj in Objects)
            {
                _obj.Key.GetComponent<Renderer>().material.color = _obj.Value;
            }
        }
    }

}

