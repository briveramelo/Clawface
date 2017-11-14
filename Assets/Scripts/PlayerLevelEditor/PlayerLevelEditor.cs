using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelEditor : MonoBehaviour
{
    public static float unitsize_x = 5.0f;
    public static float unitsize_y = 5.0f;
    public static float unitsize_z = 5.0f;

    PLE_FunctionController controller = new PLE_FunctionController();

    Button Btn_Init;
    Button Btn_Add;
    Button Btn_Duplicate;

    // Use this for initialization
    void Start ()
    {
        controller.SetFunction(new PLE_Init(controller));

        Btn_Init = PLE_ToolKit.UITool.GetUIComponent<Button>("Function_Init");
        if(Btn_Init != null) Btn_Init.onClick.AddListener(() => UseInitFunc(Btn_Init));


        Btn_Add  = PLE_ToolKit.UITool.GetUIComponent<Button>("Function_Add");
        if (Btn_Add != null) Btn_Add.onClick.AddListener(() => UsingAddFunc(Btn_Add));


        Btn_Duplicate = PLE_ToolKit.UITool.GetUIComponent<Button>("Function_Duplicate");

        if (Btn_Duplicate != null) Btn_Duplicate.onClick.AddListener(() => UsingDuplicateFunc(Btn_Duplicate));
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


    public void UsingDuplicateFunc(Button thisBtn)
    {
        controller.SetFunction(new PLE_Duplicate(controller));
    }


}
