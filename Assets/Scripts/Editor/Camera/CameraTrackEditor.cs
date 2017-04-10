// CameraTrackEditor.cs

using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom editor for camera track class.
/// </summary>
[CustomEditor(typeof(CameraTrack))]
public class CameraTrackEditor : Editor {

    #region Constants

    const float _MINI_BUTTON_WIDTH = 20f;

    #endregion
    #region Vars

    /// <summary>
    /// Target camera track (selected in editor).
    /// </summary>
    CameraTrack _target;

    static GUIContent _AddPositionButtonContent = new GUIContent("+", "Add a new camera position after this position");
    static GUIContent _DeletePositionButtonContent = new GUIContent("-", "Delete this camera position");
    static GUIContent _MovePositionUpButtonContent = new GUIContent("^", "Move this camera position backward in the list");
    static GUIContent _MovePositionDownButtonContent = new GUIContent("v", "Move this camera position forward in the list");


    /// <summary>
    /// Time of timeline preview.
    /// </summary>
    float _previewTime = 0f;

    bool _positionsExpanded = false;
    bool _eventsExpanded = false;

    #endregion
    #region Serialized Properties

    SerializedObject _serializedTarget;

    SerializedProperty _cameraProp;
    SerializedProperty _playOnAwakeProp;
    SerializedProperty _speedCurveProp;
    SerializedProperty _positionsProp;
    SerializedProperty _onCompleteTrackProp;
    SerializedProperty _eventsProp;

    #endregion
    #region Unity Callbacks

    private void OnEnable() {
        _target = target as CameraTrack;
        _serializedTarget = new SerializedObject(target);

        _cameraProp = _serializedTarget.FindProperty("_cameraToMove");
        _playOnAwakeProp = _serializedTarget.FindProperty("_playOnAwake");
        _speedCurveProp = _serializedTarget.FindProperty("_speed");
        _positionsProp = _serializedTarget.FindProperty("_positions");
        _onCompleteTrackProp = _serializedTarget.FindProperty("onCompleteTrack");
        _eventsProp = _serializedTarget.FindProperty("_events");

        _target.CheckPositionNames();
    }

    public override void OnInspectorGUI() {
        _target = target as CameraTrack;

        if (Application.isPlaying) DrawRuntimeInspector();
        else DrawEditorInspector();
    }

    #endregion
    #region Methods

    void DrawRuntimeInspector() {
        // Show playback information
        GUILayout.Label(_target.IsPlaying ? "Playing" : "Stopped");
        GUILayout.Label("Playback Time: " + _target.PlaybackTime.ToString());
        GUILayout.Label("Playback Progress: " + _target.Progress.ToString());

        // Show playback control buttons
        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        if (_target.IsPlaying) {
            if (GUILayout.Button("Pause")) _target.Pause();
        } else {
            if (GUILayout.Button("Play")) _target.Play();
        }
        if (GUILayout.Button("Stop")) _target.Stop();
        GUILayout.EndHorizontal();

        EditorUtility.SetDirty(_target);
    }

    void DrawEditorInspector() {
        // Update serialized data
        _serializedTarget.Update();
        EditorGUILayout.Space();

        // Draw serialized properties
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Camera Track Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_cameraProp);
        EditorGUILayout.PropertyField(_playOnAwakeProp);
        EditorGUILayout.PropertyField(_speedCurveProp);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_onCompleteTrackProp);
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();

        // Draw timeline preview
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Timeline Preview", EditorStyles.boldLabel);
        float newP = EditorGUILayout.Slider(_previewTime, 0f, _target.EndTime);
        if (newP != _previewTime) {
            _previewTime = newP;
            _target.PreviewTime(_previewTime);
        }
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();

        // Draw positions
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUI.indentLevel++;
        _positionsExpanded = EditorGUILayout.Foldout(_positionsExpanded, "Positions", true, EditorStyles.toolbarDropDown);
        if (_positionsExpanded) {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            if (_positionsProp.arraySize > 0) {

                // Get newest serialized properties
                float height = EditorGUI.GetPropertyHeight(_positionsProp.GetArrayElementAtIndex(0));
                _positionsProp = _serializedTarget.FindProperty("_positions");

                // Draw all properties
                GUILayout.BeginVertical();
                for (int i = 0; i < _positionsProp.arraySize; i++) {
                    int index = i;
                    var positionInfo = _positionsProp.GetArrayElementAtIndex(i);

                    // Was the position data deleted this cycle?
                    // Prevents an ArgumentOutOfRangeException.
                    bool deleted = false;

                    // Draw control buttons
                    GUILayout.BeginHorizontal(GUILayout.MinHeight(height), GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(false));
                    GUILayout.Label(i.ToString(), GUILayout.MaxWidth(32f));

                    if (GUILayout.Button(_AddPositionButtonContent, GUILayout.Width(_MINI_BUTTON_WIDTH))) _target.AddPositionAfterIndex(index);
                    if (GUILayout.Button(_DeletePositionButtonContent, GUILayout.Width(_MINI_BUTTON_WIDTH))) {
                        _target.DeletePositionAtIndex(index);
                        _positionsProp.DeleteArrayElementAtIndex(index);
                        deleted = true;
                    }
                    if (GUILayout.Button(_MovePositionUpButtonContent, GUILayout.Width(_MINI_BUTTON_WIDTH))) _target.MovePositionBackward(index);
                    if (GUILayout.Button(_MovePositionDownButtonContent, GUILayout.Width(_MINI_BUTTON_WIDTH))) _target.MovePositionForward(index);

                    // Draw position properties
                    if (!deleted) {
                        SerializedProperty prop = _positionsProp.GetArrayElementAtIndex(i);
                        EditorGUILayout.PropertyField(prop);
                    }
                    GUILayout.EndHorizontal();

                }
                EditorGUILayout.EndVertical();
            }
            // Draw add new position button
            EditorGUILayout.Space();
            if (GUILayout.Button("Add new position")) _target.AddPosition();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            

        }
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();


        // Draw events
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUI.indentLevel++;
        _eventsExpanded = EditorGUILayout.Foldout(_eventsExpanded, "Events", true, EditorStyles.toolbarDropDown);
        if (_eventsExpanded) {
            for (int i = 0; i < _eventsProp.arraySize; i++) {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(_eventsProp.GetArrayElementAtIndex(i));
                if (GUILayout.Button("X", EditorStyles.helpBox, GUILayout.MaxWidth(CameraEventDrawer.REMOVE_EVENT_BUTTON_WIDTH)))
                    _target.RemoveCameraEvent(i);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Add new event")) _target.AddCameraEvent();
        }
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();

        // Apply modified serialized changes
        _serializedTarget.ApplyModifiedProperties();
    }

    [MenuItem("Camera/Create camera track")]
    static void CreateCameraTrack () {
        new GameObject("New camera track", typeof(CameraTrack));
    }

    #endregion
}
