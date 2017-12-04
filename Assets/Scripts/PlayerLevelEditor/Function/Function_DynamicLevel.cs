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

        List<KeyValuePair<GameObject, Color>> ObjectColors;
        List<GameObject> m_GameObjects;

        public DynamicLevel(FunctionController Controller) : base(Controller)
        {

        }


        public override void Init()
        {
            base.Init();

            DynamicType = SetUIObject("DynamicType").GetComponent<Dropdown>();

            Stage = SetUIObject("StageList").GetComponent<Dropdown>();
            Stage.onValueChanged.AddListener(delegate { UpdateObject(Stage); });

            LevelObject = UnityTool.FindGameObject("LEVEL");

            if(LevelObject == null)
            {
                Debug.LogWarning("LevelObject NULL");
            }

            m_GameObjects = new List<GameObject>();
            ObjectColors = new List<KeyValuePair<GameObject, Color>>();

            foreach (Transform child in LevelObject.transform)
            {
                m_GameObjects.Add(child.gameObject);
                ObjectColors.Add(new KeyValuePair<GameObject, Color>(child.gameObject, child.gameObject.GetComponent<Renderer>().material.color));
            }

            LevelEditor.m_DynamicLevelSystem.UpdateObjectState(m_GameObjects);
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
                    LevelEditor.m_DynamicLevelSystem.SaveObjectState(hit.transform.gameObject, (LevelUnitStates)DynamicType.value);
                    return;
                }
            }
        }

        public override void Release()
        {
            base.Release();

            Stage.onValueChanged.RemoveAllListeners();

            ReleaseObjects();
        }

        public void UpdateObject(Dropdown stage)
        {
            LevelEditor.m_DynamicLevelSystem.UpdateObjectState(m_GameObjects);
        }

        public void ReleaseObjects()
        {
            foreach (var _obj in ObjectColors)
            {
                _obj.Key.GetComponent<Renderer>().material.color = _obj.Value;
            }
        }
    }

}

