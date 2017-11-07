using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PLE_ToolKit
{

    public static class UnityTool
    {
        public static void Attach(GameObject ParentObj, GameObject ChildObj, Vector3 Pos)
        {
            ChildObj.transform.parent = ParentObj.transform;
            ChildObj.transform.localPosition = Pos;
        }

        public static void AttachToRefPos(GameObject ParentObj, GameObject ChildObj, string RefPointName, Vector3 Pos)
        {
            // Search 
            bool bFinded = false;
            Transform[] allChildren = ParentObj.transform.GetComponentsInChildren<Transform>();
            Transform RefTransform = null;

            foreach (Transform child in allChildren)
            {
                if (child.name == RefPointName)
                {
                    if (bFinded == true)
                    {
                        Debug.LogWarning("Object[" + ParentObj.transform.name + "]has multiple ref with name[" + RefPointName + "]");
                        continue;
                    }
                    bFinded = true;
                    RefTransform = child;
                }
            }


            if (bFinded == false)
            {
                Debug.LogWarning("Object[" + ParentObj.transform.name + "]can't find[" + RefPointName + "]");
                Attach(ParentObj, ChildObj, Pos);
                return;
            }

            ChildObj.transform.parent = RefTransform;
            ChildObj.transform.localPosition = Pos;
            ChildObj.transform.localScale = Vector3.one;
            ChildObj.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }

        //Find GameObject in Scene
        public static GameObject FindGameObject(string GameObjectName)
        {
            GameObject pTmpGameObj = GameObject.Find(GameObjectName);

            if (pTmpGameObj == null)
            {
                Debug.LogWarning("Can't find the GameObject:[" + GameObjectName + "]");
                return null;
            }
            return pTmpGameObj;
        }

        //Find Child GameObject
        public static GameObject FindChildGameObject(GameObject Container, string gameobjectName)
        {
            if (Container == null)
            {
                Debug.LogError("NGUICustomTools.GetChild : Container = null");
                return null;
            }

            Transform pGameObjectTF = null; //= Container.transform.FindChild(gameobjectName);											


            if (Container.name == gameobjectName)
                pGameObjectTF = Container.transform;
            else
            {
                Transform[] allChildren = Container.transform.GetComponentsInChildren<Transform>(true);

                foreach (Transform child in allChildren)
                {
                    if (child.name == gameobjectName)
                    {
                        if (pGameObjectTF == null)
                            pGameObjectTF = child;
                        else
                            Debug.LogWarning("Container[" + Container.name + "]found multiple objects with name:[" + gameobjectName + "]");
                    }
                }
            }


            if (pGameObjectTF == null)
            {
                Debug.LogError("Container[" + Container.name + "]can't find[" + gameobjectName + "]");
                return null;
            }

            return pGameObjectTF.gameObject;
        }
    }


    public static class UITool
    {
        private static GameObject m_CanvasObj = null;

        public static void ReleaseCanvas()
        {
            m_CanvasObj = null;
        }


        public static GameObject FindUIGameObject(string UIName)
        {
            if (m_CanvasObj == null)
                m_CanvasObj = UnityTool.FindGameObject("Canvas");

            if (m_CanvasObj == null)
                return null;

            return UnityTool.FindChildGameObject(m_CanvasObj, UIName);
        }


        public static T GetUIComponent<T>(GameObject Container, string UIName) where T : UnityEngine.Component
        {

            GameObject ChildGameObject = UnityTool.FindChildGameObject(Container, UIName);

            if (ChildGameObject == null)
                return null;

            T tempObj = ChildGameObject.GetComponent<T>();

            if (tempObj == null)
            {
                Debug.LogWarning("Component[" + UIName + "] is not [" + typeof(T) + "]");
                return null;
            }

            return tempObj;
        }

        public static Button GetButton(string BtnName)
        {

            GameObject UIRoot = GameObject.Find("Canvas");
            if (UIRoot == null)
            {
                Debug.LogWarning("No UI Canvas");
                return null;
            }


            Transform[] allChildren = UIRoot.GetComponentsInChildren<Transform>();
            foreach (Transform child in allChildren)
            {
                if (child.name == BtnName)
                {
                    Button tmpBtn = child.gameObject.GetComponent<Button>();
                    if (tmpBtn == null)
                        Debug.LogWarning("UIComponent[" + BtnName + "] is not a Button");
                    return tmpBtn;
                }
            }

            Debug.LogWarning("UI Canvas: No Button[" + BtnName + "]");
            return null;
        }


        public static T GetUIComponent<T>(string UIName) where T : UnityEngine.Component
        {
            GameObject UIRoot = GameObject.Find("Canvas");

            if (UIRoot == null)
            {
                Debug.LogWarning("No UI Canvas");
                return null;
            }

            return GetUIComponent<T>(UIRoot, UIName);
        }
    }


    public class ToolLib : ScriptableObject
    {
        public static void draft(GameObject i_Object, Vector3 decal3, Color color)
        {
            MeshFilter objectMeshFilter = i_Object.GetComponent<MeshFilter>();
            if (objectMeshFilter != null)
            {
                Mesh mesh = objectMeshFilter.sharedMesh;
                drawDraft(i_Object, mesh, decal3, color);
            }

            foreach (Transform child in i_Object.transform)
            {
                MeshFilter SobjectMeshFilter = child.GetComponent<MeshFilter>();
                if (SobjectMeshFilter != null)
                {
                    Mesh Smesh = SobjectMeshFilter.sharedMesh;
                    drawDraft(child.gameObject, Smesh, decal3, color);
                }
                if (child.transform.childCount > 0)
                {

                    draft(child.gameObject, decal3, color);
                }
            }
        }


        public static void drawDraft(GameObject g_o, Mesh mesh, Vector3 decal3, Color color)
        {

            Vector3 v3_FrontTopLeft;
            Vector3 v3_FrontTopRight;
            Vector3 v3_FrontBottomLeft;
            Vector3 v3_FrontBottomRight;
            Vector3 v3_BackTopLeft;
            Vector3 v3_BackTopRight;
            Vector3 v3_BackBottomLeft;
            Vector3 v3_BackBottomRight;
            Vector3 v3_Center = mesh.bounds.center;
            Vector3 v3_Extents = mesh.bounds.extents;

            v3_FrontTopLeft = new Vector3(v3_Center.x - v3_Extents.x, v3_Center.y + v3_Extents.y, v3_Center.z - v3_Extents.z);
            v3_FrontTopRight = new Vector3(v3_Center.x + v3_Extents.x, v3_Center.y + v3_Extents.y, v3_Center.z - v3_Extents.z);
            v3_FrontBottomLeft = new Vector3(v3_Center.x - v3_Extents.x, v3_Center.y - v3_Extents.y, v3_Center.z - v3_Extents.z);
            v3_FrontBottomRight = new Vector3(v3_Center.x + v3_Extents.x, v3_Center.y - v3_Extents.y, v3_Center.z - v3_Extents.z);

            v3_BackTopLeft = new Vector3(v3_Center.x - v3_Extents.x, v3_Center.y + v3_Extents.y, v3_Center.z + v3_Extents.z);
            v3_BackTopRight = new Vector3(v3_Center.x + v3_Extents.x, v3_Center.y + v3_Extents.y, v3_Center.z + v3_Extents.z);
            v3_BackBottomLeft = new Vector3(v3_Center.x - v3_Extents.x, v3_Center.y - v3_Extents.y, v3_Center.z + v3_Extents.z);
            v3_BackBottomRight = new Vector3(v3_Center.x + v3_Extents.x, v3_Center.y - v3_Extents.y, v3_Center.z + v3_Extents.z);

            v3_FrontTopLeft = g_o.transform.TransformPoint(v3_FrontTopLeft);
            v3_FrontTopRight = g_o.transform.TransformPoint(v3_FrontTopRight);
            v3_FrontBottomLeft = g_o.transform.TransformPoint(v3_FrontBottomLeft);
            v3_FrontBottomRight = g_o.transform.TransformPoint(v3_FrontBottomRight);

            v3_BackTopLeft = g_o.transform.TransformPoint(v3_BackTopLeft);
            v3_BackTopRight = g_o.transform.TransformPoint(v3_BackTopRight);
            v3_BackBottomLeft = g_o.transform.TransformPoint(v3_BackBottomLeft);
            v3_BackBottomRight = g_o.transform.TransformPoint(v3_BackBottomRight);

            v3_FrontTopLeft = new Vector3(v3_FrontTopLeft.x + decal3.x, v3_FrontTopLeft.y + decal3.y, v3_FrontTopLeft.z + decal3.z);
            v3_FrontTopRight = new Vector3(v3_FrontTopRight.x + decal3.x, v3_FrontTopRight.y + decal3.y, v3_FrontTopRight.z + decal3.z);
            v3_FrontBottomLeft = new Vector3(v3_FrontBottomLeft.x + decal3.x, v3_FrontBottomLeft.y + decal3.y, v3_FrontBottomLeft.z + decal3.z);
            v3_FrontBottomRight = new Vector3(v3_FrontBottomRight.x + decal3.x, v3_FrontBottomRight.y + decal3.y, v3_FrontBottomRight.z + decal3.z);

            v3_BackTopLeft = new Vector3(v3_BackTopLeft.x + decal3.x, v3_BackTopLeft.y + decal3.y, v3_BackTopLeft.z + decal3.z);
            v3_BackTopRight = new Vector3(v3_BackTopRight.x + decal3.x, v3_BackTopRight.y + decal3.y, v3_BackTopRight.z + decal3.z);
            v3_BackBottomLeft = new Vector3(v3_BackBottomLeft.x + decal3.x, v3_BackBottomLeft.y + decal3.y, v3_BackBottomLeft.z + decal3.z);
            v3_BackBottomRight = new Vector3(v3_BackBottomRight.x + decal3.x, v3_BackBottomRight.y + decal3.y, v3_BackBottomRight.z + decal3.z);


            GL.Begin(GL.LINES);
            GL.Color(new Color(1, 0, 0));
            GL.Vertex3(v3_FrontTopLeft.x, v3_FrontTopLeft.y, v3_FrontTopLeft.z);
            GL.Vertex3(v3_FrontTopRight.x, v3_FrontTopRight.y, v3_FrontTopRight.z);
            GL.End();

            /*
            Handles.DrawDottedLine(v3_FrontTopLeft, v3_FrontTopRight, 5);
            Handles.DrawDottedLine(v3_FrontTopRight, v3_FrontBottomRight, 5);
            Handles.DrawDottedLine(v3_FrontBottomRight, v3_FrontBottomLeft, 5);
            Handles.DrawDottedLine(v3_FrontBottomLeft, v3_FrontTopLeft, 5);
            Handles.DrawDottedLine(v3_BackTopLeft, v3_BackTopRight, 5);
            Handles.DrawDottedLine(v3_BackTopRight, v3_BackBottomRight, 5);
            Handles.DrawDottedLine(v3_BackBottomRight, v3_BackBottomLeft, 5);
            Handles.DrawDottedLine(v3_BackBottomLeft, v3_BackTopLeft, 5);
            Handles.DrawDottedLine(v3_FrontTopLeft, v3_BackTopLeft, 5);
            Handles.DrawDottedLine(v3_FrontTopRight, v3_BackTopRight, 5);
            Handles.DrawDottedLine(v3_FrontBottomRight, v3_BackBottomRight, 5);
            Handles.DrawDottedLine(v3_FrontBottomLeft, v3_BackBottomLeft, 5);
            */
        }
    }
}