using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PLE_Init : PLE_IFunction
{
    GameObject UIObject;
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

    bool Initialized;

    public PLE_Init(PLE_FunctionController Controller) : base(Controller)
	{
        
    }


    public override void Init()
    {
        base.Init();

        Initialized = false;

        _prefab = Resources.Load("LevelEditorObjects/CommonArea/test") as GameObject;
        _prefab.transform.localPosition = new Vector3(0, 0, 0);

        #region UI Objcet

        UIObject = PLE_ToolKit.UITool.FindUIGameObject("UI_Init");

        if (UIObject == null)
            Debug.Log("UI_Init is not in Canvas");

        UIObject.SetActive(true);

        #endregion

        #region Action

        ACT_Init = () => EnableInit(Btn_Init);
        ACT_Save = () => EnableSave(Btn_Save);
        ACT_Load = () => EnableLoad(Btn_Load);

        #endregion

        #region Button

        Btn_Init = PLE_ToolKit.UITool.GetUIComponent<Button>("Button_Init");

        if (Btn_Init == null)
            Debug.Log("Btn_Init is null");

        Btn_Init.onClick.AddListener(ACT_Init);


        Btn_Save = PLE_ToolKit.UITool.GetUIComponent<Button>("Button_Save");

        if (Btn_Save == null)
            Debug.Log("Btn_Init is null");

        Btn_Save.onClick.AddListener(ACT_Save);


        Btn_Load = PLE_ToolKit.UITool.GetUIComponent<Button>("Button_Load");

        if (Btn_Load == null)
            Debug.Log("Btn_Load is null");

        Btn_Load.onClick.AddListener(ACT_Load);

        #endregion

        #region Slider

        Sld_X = PLE_ToolKit.UITool.GetUIComponent<Slider>("Slider_X");
        if (Sld_X == null) Debug.Log("Sld_X is null");

        Sld_Z = PLE_ToolKit.UITool.GetUIComponent<Slider>("Slider_Z");
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
                Vector3 new_position = new Vector3(i * PlayerLevelEditor.unitsize_x, 0, j * PlayerLevelEditor.unitsize_z);
                PLE_ToolKit.ToolLib.draft(_prefab, new_position, Color.yellow);
            }
        }
    }


    public override void Release()
    {
        base.Release();
        UIObject.SetActive(false);

        Btn_Init.onClick.RemoveListener(ACT_Init);
        Btn_Save.onClick.RemoveListener(ACT_Save);
        Btn_Load.onClick.RemoveListener(ACT_Load);
    }


    public void EnableInit(Button thisBtn)
    {

        if (Initialized == true) return;

        Initialized = true;

        GameObject _platform = new GameObject("LOADED LEVEL");

        for (int i = -Num_x; i <= Num_x; i++)
        {
            for (int j = -Num_z; j <= Num_z; j++)
            {
                GameObject _instance = GameObject.Instantiate(_prefab, new Vector3(i * PlayerLevelEditor.unitsize_x, 0, j * PlayerLevelEditor.unitsize_z), Quaternion.identity);
                _instance.name = "testBlock";

                if (_instance.GetComponent<LevelUnit>() == null)
                {
                    LevelUnit LU = _instance.AddComponent<LevelUnit>() as LevelUnit;
                    LU.defaultState = LevelUnitStates.floor;
                }

                _instance.transform.SetParent(_platform.transform);
            }
        }
    }

    public void EnableSave(Button thisBtn)
    {
        Debug.Log("Put Save Code in here, my baby");
    }

    public void EnableLoad(Button thisBtn)
    {
        Debug.Log("Put Load Code in here, my baby");
    }


}
