using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelEditor : MonoBehaviour
{

    PLE_FunctionController controller = new PLE_FunctionController();

    Button Btn_Init;
    Button Btn_Add;

    // Use this for initialization
    void Start ()
    {
        controller.SetFunction(new PLE_Init(controller));

        Btn_Init = PLE_ToolKit.UITool.GetUIComponent<Button>("Button_Init");
        if(Btn_Init != null) Btn_Init.onClick.AddListener(() => UseInitFunc(Btn_Init));
        Btn_Init.onClick.RemoveListener(() => UseInitFunc(Btn_Init));

        Btn_Add  = PLE_ToolKit.UITool.GetUIComponent<Button>("Button_Add");
        if (Btn_Add != null) Btn_Add.onClick.AddListener(() => UsingAddFunc(Btn_Add));
        Btn_Add.onClick.RemoveListener(() => UsingAddFunc(Btn_Add));
    }
	
	// Update is called once per frame
	void Update ()
    {
        controller.Update();
	}


    public void UseInitFunc(Button thisBtn)
    {
        controller.SetFunction(new PLE_Init(controller));
    }

    public void UsingAddFunc(Button thisBtn)
    {
        controller.SetFunction(new PLE_Add(controller));
    }

}
