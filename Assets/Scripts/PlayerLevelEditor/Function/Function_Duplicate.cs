using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace PlayerLevelEditor
{
    public class Duplicate : IFunction
    {
        GameObject _prefab;
        GameObject Handle;
        GameObject sceneActiveSelection;

        Vector3[] clones;
        UnityEngine.Camera mainCamera;

        private Vector3 mousePosition;
        private float distance = 10.0f;
        private bool movable = false;

        Button Btn_Duplicate;
        UnityAction ACT_Duplicate;


        public Duplicate(FunctionController Controller) : base(Controller)
        {

        }


        public override void Init()
        {
            base.Init();

            SetUIObject("UI_Duplicate");

            mainCamera = UnityEngine.Camera.main;

            Handle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Handle.transform.position = new Vector3(0, 10, 0);
            Handle.transform.localScale = new Vector3(3, 3, 3);
            Handle.GetComponent<Renderer>().material.color = Color.red;


            ACT_Duplicate = () => EnableDuplicate(Btn_Duplicate);
            Btn_Duplicate = PlayerLevelEditor.UITool.GetUIComponent<Button>("Button_Duplicate");
            if (Btn_Duplicate == null) Debug.Log("Button_Duplicate is null");

            Btn_Duplicate.onClick.AddListener(ACT_Duplicate);

            _prefab = Resources.Load("LevelEditorObjects/CommonArea/test") as GameObject;
        }

        public override void Update()
        {
            base.Update();

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000.0f))
            {
                if (Input.GetMouseButton(0))
                {

                    if (hit.collider.gameObject == Handle)
                    {
                        Vector3 screenPoint = mainCamera.WorldToScreenPoint(Handle.transform.position);
                        mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
                        Handle.transform.position = mainCamera.ScreenToWorldPoint(mousePosition);
                    }
                    else
                    {
                        sceneActiveSelection = hit.collider.gameObject;
                        Handle.transform.position = sceneActiveSelection.transform.position + new Vector3(0, 2 * PlayerLevelEditor.System.unitsize_y, 0);
                    }
                }
            }

            if (sceneActiveSelection)
            {
                PlayerLevelEditor.Camera _camera = mainCamera.GetComponent<PlayerLevelEditor.Camera>();
                _camera.DrawLine(Handle.transform.position, sceneActiveSelection.transform.position);
                DrawBox();
            }

        }


        public override void Release()
        {
            base.Release();

            GameObject.DestroyImmediate(Handle);

            Btn_Duplicate.onClick.RemoveListener(ACT_Duplicate);
        }


        void DrawBox()
        {
            Vector3 distance = Handle.transform.position - sceneActiveSelection.transform.position;

            List<Vector3> positions = new List<Vector3>();

            float dx = distance.x;
            float dy = distance.y;
            float dz = distance.z;

            int count_x = Mathf.Abs((int)(dx / PlayerLevelEditor.System.unitsize_x));
            int count_y = Mathf.Abs((int)(dy / PlayerLevelEditor.System.unitsize_y));
            int count_z = Mathf.Abs((int)(dz / PlayerLevelEditor.System.unitsize_z));

            float factor_x = dx > 0 ? PlayerLevelEditor.System.unitsize_x : -PlayerLevelEditor.System.unitsize_x;
            float factor_y = dy > 0 ? PlayerLevelEditor.System.unitsize_y : -PlayerLevelEditor.System.unitsize_y;
            float factor_z = dz > 0 ? PlayerLevelEditor.System.unitsize_z : -PlayerLevelEditor.System.unitsize_z;


            for (int i = 0; i <= count_x; i++)
            {
                for (int j = 0; j <= count_y; j++)
                {
                    for (int k = 0; k <= count_z; k++)
                    {
                        if (i == 0 && j == 0 && k == 0) continue;

                        Vector3 new_position;

                        new_position = new Vector3(factor_x * i, factor_y * j, factor_z * k);

                        positions.Add(new_position);

                        PlayerLevelEditor.ToolLib.draft(sceneActiveSelection, new_position, Color.yellow);
                    }
                }
            }

            clones = positions.ToArray();
        }


        void EnableDuplicate(Button thisBtn)
        {
            GameObject newClone = null;

            foreach (Vector3 thisClone in clones)
            {
                newClone = GameObject.Instantiate(sceneActiveSelection.gameObject, thisClone + sceneActiveSelection.transform.position, sceneActiveSelection.transform.rotation) as GameObject;
                newClone.transform.localScale = sceneActiveSelection.transform.localScale;

                newClone.transform.parent = sceneActiveSelection.transform.parent;
                newClone.transform.localScale = sceneActiveSelection.transform.localScale;
            }
        }
    }

}
