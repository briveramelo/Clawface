using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerLevelEditor
{
    public class LevelEditor : MonoBehaviour
    {
        static public DynamicLevelSystem m_DynamicLevelSystem;


        Database ObjectDB;
        FunctionController controller = new FunctionController();

        Button Btn_Init;
        Button Btn_Add;
        Button Btn_Duplicate;
        Button Btn_Dynamic;
        Button Btn_Test;
        Button Btn_EndTest;


        // Use this for initialization
        void Start()
        {
            m_DynamicLevelSystem = new DynamicLevelSystem();

            ObjectDB = new Database();

            controller.SetFunction(new Initialize(controller));

            Btn_Init = PlayerLevelEditor.UITool.GetUIComponent<Button>("Function_Init");
            if (Btn_Init != null) Btn_Init.onClick.AddListener(() => UseInitFunc(Btn_Init));


            Btn_Add = PlayerLevelEditor.UITool.GetUIComponent<Button>("Function_Add");
            if (Btn_Add != null) Btn_Add.onClick.AddListener(() => UsingAddFunc(Btn_Add));


            Btn_Duplicate = PlayerLevelEditor.UITool.GetUIComponent<Button>("Function_Duplicate");
            if (Btn_Duplicate != null) Btn_Duplicate.onClick.AddListener(() => UsingDuplicateFunc(Btn_Duplicate));


            Btn_Dynamic = PlayerLevelEditor.UITool.GetUIComponent<Button>("Function_Dynamic");
            if (Btn_Dynamic != null) Btn_Dynamic.onClick.AddListener(() => UsingDynamicFunc(Btn_Dynamic));


            Btn_Test = PlayerLevelEditor.UITool.GetUIComponent<Button>("Function_Test");
            if (Btn_Test != null) Btn_Test.onClick.AddListener(() => UsingTestFunc(Btn_Test));

            Btn_EndTest = PlayerLevelEditor.UITool.GetUIComponent<Button>("Function_EndTest");

            if (Btn_EndTest != null)
            {
                UITool.FindUIGameObject("Function_EndTest").SetActive(false);
                Btn_EndTest.onClick.AddListener(() => UseInitFunc(Btn_EndTest));
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }


            controller.Update();
        }


        public void UseInitFunc(Button thisBtn)
        {
            controller.SetFunction(new Initialize(controller));
        }

        public void UsingAddFunc(Button thisBtn)
        {
            controller.SetFunction(new Add(controller));
        }

        public void UsingDuplicateFunc(Button thisBtn)
        {
            controller.SetFunction(new Duplicate(controller));
        }

        public void UsingDynamicFunc(Button thisBtn)
        {
            controller.SetFunction(new DynamicLevel(controller));
        }

        public void UsingTestFunc(Button thisBtn)
        {
            if (Initialize.IsDone() == false)
                return;

            controller.SetFunction(new Test(controller));
        }
    }

    class NavMeshAreas
    {
        public const int Walkable = 0;
        public const int NotWalkable = 1;
        public const int Jump = 2;
    }
}
