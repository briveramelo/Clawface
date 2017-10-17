using UnityEngine;
using System.Collections;
using UnityEditor;

namespace OET_lib
{
	public class ToolLib : ScriptableObject
    {
		public static int header(string stringText, int vpos, bool autoHide)
		{
			int width = Screen.width;
			int height = Screen.height;
			int thisH = 0;
			if (width > 150 && height > 200 || !autoHide)
            {
				GUIStyle richStyle = new GUIStyle (GUI.skin.box);
				richStyle.richText = true;
				richStyle.wordWrap = true;
				richStyle.fontSize = 10;
				richStyle.normal.textColor = GUI.skin.label.normal.textColor;
				richStyle.alignment = TextAnchor.MiddleLeft;
				thisH = (int)richStyle.CalcHeight (new GUIContent (stringText), width - 20) + 4;
				GUI.Box (new Rect (10, vpos, width - 20, thisH), stringText, richStyle);
				thisH += 15;
			}
			return(thisH);
		}

		public static void alertBox(string title, string stringText)
		{
			int vpos = 0;
			int width = Screen.width;
			int height = Screen.height;

			GUIStyle richStyle = new GUIStyle (GUI.skin.label);
			richStyle.richText = true;
			richStyle.wordWrap = true;
			richStyle.fontSize = 11;
			richStyle.normal.textColor = GUI.skin.label.normal.textColor;
			richStyle.alignment = TextAnchor.MiddleCenter;
			string headerTitle = "<b>" + title + "</b>\n\n";
            int lw = width > 600 ? 600 : width - 20;
            GUI.Box (new Rect (width / 2 - lw / 2, vpos, lw, height - vpos > 200 ? 200 : height - vpos), headerTitle + stringText, richStyle);
		}

		public static Bounds getAllBounds(GameObject i_Object)
        {
			MeshFilter objectMeshFilter = i_Object.GetComponent<MeshFilter> ();
			Bounds allBounds = new Bounds();
			if (objectMeshFilter != null)
            {
				Mesh mesh = objectMeshFilter.sharedMesh;
				allBounds = mesh.bounds;
			}
			foreach (Transform child in i_Object.transform)
            {
				MeshFilter SobjectMeshFilter = child.GetComponent<MeshFilter> ();
				if (SobjectMeshFilter != null)
                {
					Mesh Smesh = SobjectMeshFilter.sharedMesh;
					allBounds.Encapsulate(Smesh.bounds);
				}
				if (child.transform.childCount > 0)
                {
					allBounds.Encapsulate(getAllBounds (child.gameObject));
				}
			}
			return(allBounds);
		}

		public static void draft(GameObject i_Object, Vector3 decal3, Color color)
        {
			MeshFilter objectMeshFilter = i_Object.GetComponent<MeshFilter> ();
			if (objectMeshFilter != null)
            {
				Mesh mesh = objectMeshFilter.sharedMesh;
				drawDraft (i_Object, mesh, decal3, color);
			}

			foreach (Transform child in i_Object.transform)
            {
				MeshFilter SobjectMeshFilter = child.GetComponent<MeshFilter> ();
				if (SobjectMeshFilter != null)
                {
					Mesh Smesh = SobjectMeshFilter.sharedMesh;
					drawDraft (child.gameObject, Smesh, decal3, color);
				}
				if (child.transform.childCount > 0)
                {
					
					draft (child.gameObject, decal3, color);
				}
			}
		}


		public static void drawDraft(GameObject g_o, Mesh mesh, Vector3 decal3, Color color)
        {
			Handles.color = color;

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
			
			v3_FrontTopLeft      = new Vector3 (v3_Center.x - v3_Extents.x, v3_Center.y + v3_Extents.y, v3_Center.z- v3_Extents.z);
			v3_FrontTopRight     = new Vector3 (v3_Center.x + v3_Extents.x, v3_Center.y + v3_Extents.y, v3_Center.z - v3_Extents.z); 
			v3_FrontBottomLeft   = new Vector3 (v3_Center.x - v3_Extents.x, v3_Center.y - v3_Extents.y, v3_Center.z - v3_Extents.z);
			v3_FrontBottomRight  = new Vector3 (v3_Center.x + v3_Extents.x, v3_Center.y - v3_Extents.y, v3_Center.z - v3_Extents.z);
			
			v3_BackTopLeft       = new Vector3 (v3_Center.x - v3_Extents.x, v3_Center.y + v3_Extents.y, v3_Center.z + v3_Extents.z);
			v3_BackTopRight      = new Vector3 (v3_Center.x + v3_Extents.x, v3_Center.y + v3_Extents.y, v3_Center.z + v3_Extents.z);
			v3_BackBottomLeft    = new Vector3 (v3_Center.x - v3_Extents.x, v3_Center.y - v3_Extents.y, v3_Center.z + v3_Extents.z);
			v3_BackBottomRight   = new Vector3 (v3_Center.x + v3_Extents.x, v3_Center.y - v3_Extents.y, v3_Center.z + v3_Extents.z);
			
			v3_FrontTopLeft      = g_o.transform.TransformPoint (v3_FrontTopLeft);
			v3_FrontTopRight     = g_o.transform.TransformPoint (v3_FrontTopRight);
			v3_FrontBottomLeft   = g_o.transform.TransformPoint (v3_FrontBottomLeft);
			v3_FrontBottomRight  = g_o.transform.TransformPoint (v3_FrontBottomRight);
			
			v3_BackTopLeft       = g_o.transform.TransformPoint (v3_BackTopLeft);
			v3_BackTopRight      = g_o.transform.TransformPoint (v3_BackTopRight);
			v3_BackBottomLeft    = g_o.transform.TransformPoint (v3_BackBottomLeft);
			v3_BackBottomRight   = g_o.transform.TransformPoint (v3_BackBottomRight);

			v3_FrontTopLeft      = new Vector3(v3_FrontTopLeft.x + decal3.x, v3_FrontTopLeft.y + decal3.y, v3_FrontTopLeft.z + decal3.z); 
			v3_FrontTopRight     = new Vector3(v3_FrontTopRight.x + decal3.x, v3_FrontTopRight.y + decal3.y, v3_FrontTopRight.z + decal3.z); 
			v3_FrontBottomLeft   = new Vector3(v3_FrontBottomLeft.x + decal3.x, v3_FrontBottomLeft.y + decal3.y, v3_FrontBottomLeft.z + decal3.z); 
			v3_FrontBottomRight  = new Vector3(v3_FrontBottomRight.x + decal3.x, v3_FrontBottomRight.y + decal3.y, v3_FrontBottomRight.z + decal3.z); 
			
			v3_BackTopLeft       = new Vector3(v3_BackTopLeft.x + decal3.x, v3_BackTopLeft.y + decal3.y, v3_BackTopLeft.z + decal3.z); 
			v3_BackTopRight      = new Vector3(v3_BackTopRight.x + decal3.x, v3_BackTopRight.y + decal3.y, v3_BackTopRight.z + decal3.z); 
			v3_BackBottomLeft    = new Vector3(v3_BackBottomLeft.x + decal3.x, v3_BackBottomLeft.y + decal3.y, v3_BackBottomLeft.z + decal3.z); 
			v3_BackBottomRight   = new Vector3(v3_BackBottomRight.x + decal3.x, v3_BackBottomRight.y + decal3.y, v3_BackBottomRight.z + decal3.z); 

			Handles.DrawDottedLine (v3_FrontTopLeft, v3_FrontTopRight, 5);
			Handles.DrawDottedLine (v3_FrontTopRight, v3_FrontBottomRight, 5);
			Handles.DrawDottedLine (v3_FrontBottomRight, v3_FrontBottomLeft, 5);
			Handles.DrawDottedLine (v3_FrontBottomLeft, v3_FrontTopLeft, 5);
			Handles.DrawDottedLine (v3_BackTopLeft, v3_BackTopRight, 5);
			Handles.DrawDottedLine (v3_BackTopRight, v3_BackBottomRight, 5);
			Handles.DrawDottedLine (v3_BackBottomRight, v3_BackBottomLeft, 5);
			Handles.DrawDottedLine (v3_BackBottomLeft, v3_BackTopLeft, 5);
			Handles.DrawDottedLine (v3_FrontTopLeft, v3_BackTopLeft, 5);
			Handles.DrawDottedLine (v3_FrontTopRight, v3_BackTopRight, 5);
			Handles.DrawDottedLine (v3_FrontBottomRight, v3_BackBottomRight, 5);
			Handles.DrawDottedLine (v3_FrontBottomLeft, v3_BackBottomLeft, 5);
		
		}
	}
}