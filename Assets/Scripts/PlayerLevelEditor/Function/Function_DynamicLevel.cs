using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerLevelEditor
{
    public class DynamicLevel : IFunction
    {
        GameObject StageList;

        public DynamicLevel(FunctionController Controller) : base(Controller)
        {

        }


        public override void Init()
        {
            base.Init();

            SetUIObject("StageList");

            StageList = UIObject;
        }

        public override void Update()
        {
            base.Update();

            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000.0f) && Input.GetMouseButtonUp(0))
            {

                OnClickObject CampClickScript = hit.transform.gameObject.GetComponent<OnClickObject>();

                if (CampClickScript != null)
                {
                    CampClickScript.Click(StageList);
                    return;
                }
            }
        }

        public override void Release()
        {
            base.Release();
        }
    }

}

