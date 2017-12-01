using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


namespace PlayerLevelEditor
{

    public class Initialize : IFunction
    {
        GameObject _prefab;

        Button Btn_Init;
        Button Btn_Save;
        Button Btn_Load;

        UnityAction ACT_Init;
        UnityAction ACT_Save;
        UnityAction ACT_Load;


        Slider Sld_X;
        Slider Sld_Z;

        int Num_x = 0;
        int Num_z = 0;

        static bool Initialized = false;

        public Initialize(FunctionController Controller) : base(Controller)
        {

        }


        public override void Init()
        {
            base.Init();

            _prefab = Resources.Load("PlayerLevelEditorObjects/Env/test") as GameObject;
            _prefab.transform.localPosition = new Vector3(0, 0, 0);

            #region UI Objcet

            SetUIObject("UI_Init");

            #endregion

            #region Action

            ACT_Init = () => EnableInit(Btn_Init);
            ACT_Save = () => EnableSave(Btn_Save);
            ACT_Load = () => EnableLoad(Btn_Load);

            #endregion

            #region Button

            Btn_Init = PlayerLevelEditor.UITool.GetUIComponent<Button>("Button_Init");

            if (Btn_Init == null)
                Debug.Log("Btn_Init is null");

            Btn_Init.onClick.AddListener(ACT_Init);


            Btn_Save = PlayerLevelEditor.UITool.GetUIComponent<Button>("Button_Save");

            if (Btn_Save == null)
                Debug.Log("Btn_Init is null");

            Btn_Save.onClick.AddListener(ACT_Save);


            Btn_Load = PlayerLevelEditor.UITool.GetUIComponent<Button>("Button_Load");

            if (Btn_Load == null)
                Debug.Log("Btn_Load is null");

            Btn_Load.onClick.AddListener(ACT_Load);

            #endregion

            #region Slider

            Sld_X = PlayerLevelEditor.UITool.GetUIComponent<Slider>("Slider_X");
            if (Sld_X == null) Debug.Log("Sld_X is null");

            Sld_Z = PlayerLevelEditor.UITool.GetUIComponent<Slider>("Slider_Z");
            if (Sld_Z == null) Debug.Log("Sld_Z is null");

            #endregion
        }

        public override void Update()
        {
            base.Update();

            Num_x = (int)Sld_X.value;
            Num_z = (int)Sld_Z.value;

            for (int i = -Num_x; i <= Num_x; i++)
            {
                for (int j = -Num_z; j <= Num_z; j++)
                {
                    Vector3 new_position = new Vector3(i * PlayerLevelEditor.System.unitsize_x, 0, j * PlayerLevelEditor.System.unitsize_z);
                    PlayerLevelEditor.ToolLib.draft(_prefab, new_position, Color.yellow);
                }
            }
        }


        public override void Release()
        {
            base.Release();

            Btn_Init.onClick.RemoveListener(ACT_Init);
            Btn_Save.onClick.RemoveListener(ACT_Save);
            Btn_Load.onClick.RemoveListener(ACT_Load);
        }


        void EnableInit(Button thisBtn)
        {

            if (Initialized == true) return;

            Initialized = true;

            GameObject _platform = new GameObject("LOADED LEVEL");

            for (int i = -Num_x; i <= Num_x; i++)
            {
                for (int j = -Num_z; j <= Num_z; j++)
                {
                    GameObject _instance = GameObject.Instantiate(_prefab, new Vector3(i * PlayerLevelEditor.System.unitsize_x, 0, j * PlayerLevelEditor.System.unitsize_z), Quaternion.identity);
                    _instance.name = "testBlock";


                    //Edge + Wall
                    if (i == -Num_x || i == Num_x || j == -Num_z || j == Num_z)
                    {
                        _AddNavMeshModifier(_instance, PlayerLevelEditor.NavMeshAreas.NotWalkable);

                        GameObject _wall = GameObject.Instantiate(_prefab, new Vector3(i * PlayerLevelEditor.System.unitsize_x, 
                                                                                           PlayerLevelEditor.System.unitsize_y, 
                                                                                       j * PlayerLevelEditor.System.unitsize_z), Quaternion.identity);

                        _wall.transform.SetParent(_platform.transform);
                        _AddNavMeshModifier(_wall, PlayerLevelEditor.NavMeshAreas.NotWalkable);

                    }
                    else if (_instance.GetComponent<LevelUnit>() == null)
                    {
                        LevelUnit LU = _instance.AddComponent<LevelUnit>() as LevelUnit;
                        LU.defaultState = LevelUnitStates.floor;
                        _AddNavMeshModifier(_instance, PlayerLevelEditor.NavMeshAreas.Walkable);
                    }

                    _instance.transform.SetParent(_platform.transform);
                }
            }
        }


        void _AddNavMeshModifier(GameObject _object, int _state)
        {
            UnityEngine.AI.NavMeshModifier _mod = _object.AddComponent<UnityEngine.AI.NavMeshModifier>() as UnityEngine.AI.NavMeshModifier;
            _mod.overrideArea = true;
            _mod.area = _state;
        }


        void EnableSave(Button thisBtn)
        {
            Debug.Log("Put Save Code in here, my baby");
        }

        void EnableLoad(Button thisBtn)
        {
            Debug.Log("Put Load Code in here, my baby");
        }

    }
}

