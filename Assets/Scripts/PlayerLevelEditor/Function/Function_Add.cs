using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using PlayerLevelEditor;
using Turing.LevelEditor;


namespace PlayerLevelEditor
{
    public class Add : IFunction
    {
        public static GameObject _prefab;

        GameObject _LevelObject;

        Button Btn_Add;
        UnityAction ACT_Add;

        public Vector3 mousePositionInScene;

        private bool clickToAddEnabled = false;

        public Add(FunctionController Controller) : base(Controller)
        {

        }

        public override void Init()
        {
            base.Init();

            _LevelObject = UnityTool.FindGameObject("LEVEL");

            SetUIObject("UI_Add");

            ACT_Add = () => EnableAdd(Btn_Add);
            Btn_Add = PlayerLevelEditor.UITool.GetUIComponent<Button>("Button_Add");
            if (Btn_Add == null) Debug.Log("Btn_Add is null");

            Btn_Add.onClick.AddListener(ACT_Add);
            Btn_Add.image.color = Color.white;

            _prefab = Resources.Load(Strings.Editor.RESOURCE_PATH + "Env/test") as GameObject;

            Database.Enable();
        }


        public override void Update()
        {
            base.Update();

            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000.0f))
            {
                mousePositionInScene = hit.point;

                if (Input.GetMouseButtonDown(0) && clickToAddEnabled)
                {
                    LevelUnit LU = hit.transform.gameObject.GetComponent<LevelUnit>();

                    if (LU != null)
                    {
                        GameObject.DestroyImmediate(LU);
                    }
          
                    Vector3 _pos = PlayerLevelEditor.ToolLib.ConvertToGrid(mousePositionInScene);

                    if (_pos.x == 0.0f && _pos.z == 0.0f)
                    {
                        return;
                    }

                    GameObject _instance = GameObject.Instantiate(_prefab, _pos, Quaternion.identity);

                    _instance.transform.SetParent(_LevelObject.transform);
                }

            }

            PlayerLevelEditor.ToolLib.draft(_prefab, PlayerLevelEditor.ToolLib.ConvertToGrid(mousePositionInScene - _prefab.transform.position), Color.green);
        }

        public override void Release()
        {
            base.Release();
            Btn_Add.onClick.RemoveListener(ACT_Add);

            Database.Disable();
        }


       
        void EnableAdd(Button thisBtn)
        {
            clickToAddEnabled = !clickToAddEnabled;
            thisBtn.image.color = clickToAddEnabled ? Color.red : Color.white;
        }

    }
}

