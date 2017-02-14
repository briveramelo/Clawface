//// ObjectDatabaseManagerEditor.cs

//using UnityEngine;
//using UnityEditor;

///// <summary>
///// Custom editor for OBJDB manager.
///// </summary>
//[CustomEditor(typeof(ObjectDatabaseManager))]
//public class ObjectDatabaseManagerEditor : Editor {

//    #region Vars

//    /// <summary>
//    /// Target OBJDB manager.
//    /// </summary>
//    ObjectDatabaseManager _target;

//    #endregion
//    #region Unity Callbacks

//    void OnEnable() {
//        _target = target as ObjectDatabaseManager;
//    }

//    #endregion
//    #region Unity Overrides

//    public override void OnInspectorGUI() {

//        // Rebuild categories button
//        if (GUILayout.Button("Rebuild categories")) {
//            _target.RebuildCategories();
//        }

//        // JSON save button
//        if (GUILayout.Button("Save database to JSON")) {
//            _target.SaveToJSON();
//        }

//        // JSON load button
//        if (GUILayout.Button("Load database from JSON")) {
//            _target.LoadFromJSON();
//        }

//        // Draw default GUI (for database elements)
//        base.OnInspectorGUI();
//    }

//    #endregion
//}
