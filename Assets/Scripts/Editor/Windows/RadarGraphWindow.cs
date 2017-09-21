using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class RadarGraphWindow : EditorWindow{

    #region Fields
    List<IRadarGraph> iRadarGraphs = new List<IRadarGraph>();
    bool drawWebLines, showAdvanced, setValuesManually, showGraphColors, hideUnSelectedHandles, showAreaGraph, useCustomMaxArea;
    float shapeRadius = 130, customMaxGraphArea=10;
    const float standardHeight = 16f;
    const float standardSpacing = standardHeight + 4f;
    float labelSpacing = 85f;
    const int minNodes = 3;
    int segments;
    float unitAngle { get { return 360f / iRadarGraphs[0].BaseNodes.Count; } }

    Rect firstRect = new Rect(20, standardHeight, 265, standardHeight);
    Rect lastRect { get { return new Rect(20, position.height-standardHeight*2, 265, standardHeight); } }
    Vector2 areaGraphSize = new Vector2(150, 150);

    GUIStyle centeredStyle;
    GUIStyle CenteredStyle {
        get {
            if (centeredStyle == null) {
                centeredStyle = new GUIStyle(GUI.skin.textField) {
                    alignment = TextAnchor.MiddleCenter
                };
            }
            return centeredStyle;
        }
    }
    Object ActiveObject { get { return Selection.activeObject; } }
    Object[] ActiveObjects { get { return Selection.objects; } }
    List<ScriptableObject> myScriptableObjects = new List<ScriptableObject>();
    List<ScriptableObject> MyScriptableObjects {
        get {
            myScriptableObjects.Clear();
            iRadarGraphs.ForEach(graph => { myScriptableObjects.Add(graph as ScriptableObject); });
            return myScriptableObjects;
        }
    }
    List<SerializedObject> mySerializedObjects=new List<SerializedObject>();
    List<SerializedObject> MySerializedObjects {
        get {
            if (lastSelectedCount != iRadarGraphs.Count || mySerializedObjects.Count==0) {
                mySerializedObjects.Clear();
                MyScriptableObjects.ForEach(graph => { mySerializedObjects.Add(new SerializedObject(graph)); });
                ResetSelectionList();
            }
            return mySerializedObjects;
        }
    }
    List<bool> selectionList = new List<bool>();
    List<bool> SelectionList {
        get {
            if (selectionList.Count!= iRadarGraphs.Count || selectionList.Count==0) {
                ResetSelectionList();
            }
            return selectionList;
        }
        set {
            selectionList = value;
        }
    }
    void ResetSelectionList() {
        selectionList.Clear();
        selectionList.Add(true);
        for (int i = 1; i < iRadarGraphs.Count; i++) {
            selectionList.Add(false);
        }
    }
    int lastSelectedCount;
    int SelectedRadarIndex { get { return SelectionList.FindIndex(item => item); } }
    #endregion


    [MenuItem("Window/Radar Graph", false, 1)]
    public static void ShowWindow() {
        RadarGraphWindow window = GetWindow<RadarGraphWindow>("Radar Graph");
        window.Show();
    }

    void OnGUI() {
        if (ActiveObject != null) {
            TrySetRadarGraph();
            int radarCount = iRadarGraphs.Count;
            if (radarCount > 0) {
                RadarType type = iRadarGraphs[0].RadarType;
                bool tooFewNodes = iRadarGraphs.Exists(graph => graph.BaseNodes.Count < minNodes);
                bool graphMismatch = iRadarGraphs.Exists(graph => graph.RadarType != type);
                bool showAll = !tooFewNodes && !graphMismatch;
                if (!showAll) {
                    if (tooFewNodes) {
                        EditorGUI.LabelField(firstRect, "Use at least 3 nodes");
                    }
                    else if (graphMismatch) {
                        EditorGUI.LabelField(firstRect, "Selected graphs are mismatched types");
                    }
                }
                else {
                    int nodeCount = iRadarGraphs[0].BaseNodes.Count;
                    DrawHelperControls(radarCount, unitAngle);
                    for (int i=0; i< radarCount; i++) {
                        if (iRadarGraphs[i].ShouldReset()) {
                            iRadarGraphs[i].ResetNodes();
                        }
                                    
                        MySerializedObjects[i].Update();

                        DrawRadarGraph(i, nodeCount, type);
                        DrawAreaGraph(i);

                        if (GUI.changed) {
                            Undo.RecordObject(ActiveObject, "Radar Graph Editor Modified");
                            EditorUtility.SetDirty(ActiveObject);
                        }
                        MySerializedObjects[i].ApplyModifiedProperties();
                    }
                }
            }
        }
        lastSelectedCount = ActiveObjects.Length;
    }

    #region Helper Methods
    void DrawAreaGraph(int radarIndex) {
        if (radarIndex == 0) {
            showAreaGraph = EditorGUI.ToggleLeft(lastRect, "Show Area Graph", showAreaGraph);
        }
        if (showAreaGraph) {
            Vector2 basePosition = new Vector2(standardSpacing, position.height-standardHeight*2);
            Rect areaGraphRect = new Rect(basePosition, areaGraphSize).AddPosition(-Vector2.up * (standardSpacing*4 + areaGraphSize.y));
            RectBounds bounds = areaGraphRect.Bounds();
            float maxArea = GetArea(radarIndex, AreaType.Max);

            float usedMaxGraphArea = useCustomMaxArea ? customMaxGraphArea : maxArea;
            if (radarIndex==0) {
                areaGraphSize = EditorGUI.Vector2Field(lastRect.AddPosition(-Vector2.up*(standardHeight*3+5)), "Graph Size", areaGraphSize);
                customMaxGraphArea = EditorGUI.FloatField(lastRect.AddPosition(new Vector2(150, -(standardHeight * 1 + 5))), "Max Area", customMaxGraphArea);
                useCustomMaxArea= EditorGUI.ToggleLeft(lastRect.AddPosition(-Vector2.up * (standardHeight * 1 + 5)), "Use Custom Max", useCustomMaxArea);
                DrawBoundaryLine(bounds.BottomLeft, bounds.BottomRight);
                DrawBoundaryLine(bounds.BottomLeft, bounds.TopLeft);
                EditorGUI.LabelField(new Rect(areaGraphRect).AddPosition(-Vector2.up * standardSpacing), string.Format("{0:0.#}", usedMaxGraphArea));
            }
            float xPercent = (float)(radarIndex + 1) / iRadarGraphs.Count;
            float adjustedXPercent = xPercent - 0.5f / iRadarGraphs.Count;
            Vector2 startPos = bounds.BottomLeft + adjustedXPercent * (bounds.BottomRight - bounds.BottomLeft);
            float area = GetArea(radarIndex);
            float yPercent = area / usedMaxGraphArea;
            Vector2 ballPos = startPos - Vector2.up * areaGraphRect.height * yPercent;
            DrawActualValueLine(radarIndex, startPos, ballPos);
            Handles.color = iRadarGraphs[radarIndex].Color;
            Vector3 ballPosition = new Vector3(ballPos.x, ballPos.y, -10);
            Handles.DrawSphere(43, ballPosition, Quaternion.identity, 5);
            EditorGUI.LabelField(new Rect(ballPosition, new Vector2(30, standardHeight)).AddPosition(-Vector2.up * standardSpacing ), string.Format("{0:0.#}", area));
        }
    }

    void DrawHelperControls(int radarCount, float unitAngle) {
        shapeRadius = EditorGUI.Slider(firstRect, "Shape Size", shapeRadius, 50, 390);
        showAdvanced = EditorGUI.ToggleLeft(new Rect(firstRect).AddPosition(new Vector2(0, standardSpacing * 1)), "Advanced", showAdvanced);
        if (showAdvanced) {
            EditorGUI.indentLevel++;
            labelSpacing = EditorGUI.Slider(new Rect(firstRect).AddPosition(new Vector2(0, standardSpacing * 2)), "Label Spacing", labelSpacing, 50, 150);
            segments = EditorGUI.IntSlider(new Rect(firstRect).AddPosition(new Vector2(0, standardSpacing * 3)), new GUIContent("Segments", "Lock to segments by holding 'Ctrl' while dragging. For best results, start at full value"), segments, 0, 5);


            drawWebLines = EditorGUI.ToggleLeft(new Rect(firstRect).AddPosition(new Vector2(0, standardSpacing * 4)), "Draw Web Lines", drawWebLines);
            setValuesManually = EditorGUI.ToggleLeft(new Rect(firstRect).AddPosition(new Vector2(0, standardSpacing * 5)), "Set Values Manually", setValuesManually);
            hideUnSelectedHandles = EditorGUI.ToggleLeft(new Rect(firstRect).AddPosition(new Vector2(0, standardSpacing * 6)), "Hide Unselected handles", hideUnSelectedHandles);
            
            showGraphColors = EditorGUI.Foldout(new Rect(firstRect).AddPosition(new Vector2(0, standardSpacing * 7)), showGraphColors, "Selected Graph(s)", true);
            if (showGraphColors) {
                EditorGUI.indentLevel++;
                for (int j = 0; j < radarCount; j++) {
                    EditorGUI.BeginDisabledGroup(j == SelectedRadarIndex);
                    SelectionList[j] = EditorGUI.ToggleLeft(new Rect(firstRect).AddPosition(new Vector2(0, standardSpacing * (8 + j))), "", SelectionList[j]);
                    EditorGUI.EndDisabledGroup();

                    iRadarGraphs[j].Color = EditorGUI.ColorField(new Rect(firstRect).AddPosition(new Vector2(20, standardSpacing * (8 + j))), MyScriptableObjects[j].name, iRadarGraphs[j].Color);
                    if (SelectionList[j]) {
                        DeselectOthers(j, radarCount);
                    }
                }
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
        }
    }

    
    void DeselectOthers(int j, int radarCount) {
        for (int radarIndex = 0; radarIndex < radarCount; radarIndex++) {
            if (j != radarIndex) {
                SelectionList[radarIndex] = false;
            }
        }        
    }

    static float TriangleArea(float sideA, float sideB, float intermediateAngle_Deg) {
        return 0.5f * sideA * sideB * Mathf.Sin(intermediateAngle_Deg * Mathf.Deg2Rad);
    }
    float GetArea(int radarIndex, AreaType type=AreaType.Actual) {
        float area = 0;
        int nodeCount = iRadarGraphs[radarIndex].BaseNodes.Count;
        for (int i = 1; i < nodeCount; i++) {
            RadarNode node = iRadarGraphs[radarIndex].BaseNodes[i];
            RadarNode lastNode = iRadarGraphs[radarIndex].BaseNodes[i-1];

            float lastValue = type == AreaType.Actual ? lastNode.Value : lastNode.valueRange.Max;
            float value = type == AreaType.Actual ? node.Value : node.valueRange.Max;
            area += TriangleArea(lastValue - lastNode.valueRange.Min, value - node.valueRange.Min, unitAngle);
        }
        RadarNode firstNode = iRadarGraphs[radarIndex].BaseNodes[0];
        RadarNode finalNode = iRadarGraphs[radarIndex].BaseNodes[nodeCount - 1];
        float firstValue = type == AreaType.Actual ? firstNode.Value : firstNode.valueRange.Max;
        float finalValue = type == AreaType.Actual ? finalNode.Value : finalNode.valueRange.Max;
        area += TriangleArea(firstValue - firstNode.valueRange.Min, finalValue - finalNode.valueRange.Min, unitAngle);
        return area;
    }
    enum AreaType {
        Actual, Max
    }

    void DrawRadarGraph(int radarIndex, int nodeCount, RadarType type) {
        Vector2 center = position.size / 2;
        Vector3 center3 = center;
        Handles.color = Color.black;
        Handles.DrawSphere(42, center3 - Vector3.forward * 10, Quaternion.identity, 10f);

        for (int j = 0; j < nodeCount; j++) {
            float nodeAngle = j * unitAngle - 90f;
            Vector2 unitNodeVector = nodeAngle.ConvertAngleToVector2();
            RadarNode node = iRadarGraphs[radarIndex].BaseNodes[j];

            node.center = center;
            node.direction = unitNodeVector * shapeRadius;
            Vector2 boundaryNodePosition = center + unitNodeVector * shapeRadius;
            Rect labelRect = new Rect(-115f / 2f, -standardHeight / 2, 150, standardHeight).AddPosition(boundaryNodePosition).AddPosition(unitNodeVector * (labelSpacing));
            Rect sliderRect = new Rect(labelRect).AddPosition(Vector3.up * standardSpacing);
            node.maxPosition = boundaryNodePosition;

            if (j > 0) {
                RadarNode lastNode = iRadarGraphs[radarIndex].BaseNodes[j - 1];
                if (radarIndex==0) {
                    DrawBoundaryLine(lastNode.maxPosition, node.maxPosition);
                    DrawSegmentLines(radarIndex, j - 1, j);
                }
                DrawActualValueLine(radarIndex, lastNode.Position, node.Position);
                Handles.DrawSphere(49, new Vector3(node.Position.x, node.Position.y, -10), Quaternion.identity, 5f);
            }
            float snapAmount = (1f / (segments + 1));

            if (radarIndex==SelectedRadarIndex || !hideUnSelectedHandles) {
                DrawHandle(radarIndex, j, nodeAngle, snapAmount);
            }
            if (drawWebLines) {
                DrawWebLine(radarIndex, j);
            }

            DrawSelectedGraphControls(radarIndex, j, sliderRect, labelRect, type);
        }
        if (radarIndex == 0) {
            DrawSegmentLines(radarIndex, nodeCount - 1, 0);
            DrawBoundaryLine(iRadarGraphs[radarIndex].BaseNodes[nodeCount - 1].maxPosition, iRadarGraphs[radarIndex].BaseNodes[0].maxPosition);
        }
        DrawActualValueLine(radarIndex, iRadarGraphs[radarIndex].BaseNodes[nodeCount - 1].Position, iRadarGraphs[radarIndex].BaseNodes[0].Position);
        Handles.DrawSphere(49, new Vector3(iRadarGraphs[radarIndex].BaseNodes[0].Position.x, iRadarGraphs[radarIndex].BaseNodes[0].Position.y, -10), Quaternion.identity, 5f);
    }



    void DrawSelectedGraphControls(int radarIndex, int nodeIndex, Rect sliderRect, Rect labelRect, RadarType type) {
        RadarNode node = iRadarGraphs[radarIndex].BaseNodes[nodeIndex];
        if (radarIndex == SelectedRadarIndex) {
            DrawLabel(radarIndex, nodeIndex, labelRect, type);
            if (!setValuesManually) {
                if(node.isRangeReversed) node.Value = EditorGUI.Slider(sliderRect, node.Value, node.valueRange.Max, node.valueRange.Min);
                else node.Value = EditorGUI.Slider(sliderRect, node.Value, node.valueRange.Min, node.valueRange.Max);
            }
            else {
                node.Value = EditorGUI.FloatField(sliderRect, node.Value);
            }
        }
    }

    void DrawLabel(int radarIndex, int nodeIndex, Rect labelRect, RadarType type) {
        switch (type) {
            case RadarType.CharacterStats:
                CharacterStatsNode nodeC = (CharacterStatsNode)iRadarGraphs[radarIndex].BaseNodes[nodeIndex];
                nodeC.statType = (CharacterStatType)EditorGUI.EnumPopup(labelRect, nodeC.statType);
                break;
            case RadarType.String_SAMPLE:
                StringNode_SAMPLE nodeR = (StringNode_SAMPLE)iRadarGraphs[radarIndex].BaseNodes[nodeIndex];
                nodeR.Name = EditorGUI.TextField(labelRect, nodeR.Name, CenteredStyle);
                break;
            case RadarType.CharacterStats_SAMPLE:
                CharacterStatsNode_SAMPLE nodeS = (CharacterStatsNode_SAMPLE)iRadarGraphs[radarIndex].BaseNodes[nodeIndex];
                nodeS.statType = (CharacterStatType_SAMPLE)EditorGUI.EnumPopup(labelRect, nodeS.statType);
                break;
            case RadarType.WeaponStats_SAMPLE:
                WeaponStatNode_SAMPLE nodeW = (WeaponStatNode_SAMPLE)iRadarGraphs[radarIndex].BaseNodes[nodeIndex];
                nodeW.statType = (WeaponStat_SAMPLE)EditorGUI.EnumPopup(labelRect, nodeW.statType);
                break;
        }
    }

    void DrawHandle(int radarIndex, int nodeIndex,float nodeAngle, float snapAmount) {
        Handles.color = iRadarGraphs[radarIndex].Color;
        RadarNode node = iRadarGraphs[radarIndex].BaseNodes[nodeIndex];
        float size = 300f;
        //TO DO
        //project position handles.onto node direction
        //node.value = Handles.SnapValue(node.value, snapAmount);
        //if (Event.current.type==EventType.Repaint) {
        //    Handles.ArrowHandleCap(radarIndex * 200 + nodeIndex, node.Position - Vector3.forward * 10, Quaternion.Euler(-nodeAngle, 90, 0), size, EventType.Repaint);
        //}
        //Vector3 newPos = Handles.handle(node.Position, Quaternion.Euler(-nodeAngle, 90, 0));//, size, node.direction.normalized * snapAmount, Handles.ArrowHandleCap);
        //Handles.DoPositionHandle
        //Vector3 fromCenter = newPos - node.center;
        //float newValue = fromCenter.magnitude / node.direction.magnitude;
        //node.value = newValue;
        node.value = Mathf.Clamp01(Handles.ScaleValueHandle(node.value, node.Position - Vector3.forward * 10, Quaternion.Euler(-nodeAngle,90,0), size, Handles.ArrowHandleCap, snapAmount));
    }

    void DrawWebLine(int radarIndex, int nodeIndex) {
        Handles.color = Color.grey;
        RadarNode node = iRadarGraphs[radarIndex].BaseNodes[nodeIndex];
        Handles.DrawLine(node.center, node.maxPosition);
    }

    void DrawBoundaryLine(Vector2 start, Vector2 end) {
        Handles.color = Color.black;
        Handles.DrawLine(start, end);
    }
    void DrawSegmentLines(int radarIndex, int lastIndex, int currentIndex) {
        for (int j = 1; j <= segments; j++) {
            float percentage = (float)j / (segments + 1);
            Vector3 lastPosition = iRadarGraphs[radarIndex].BaseNodes[lastIndex].GetPosition(percentage);
            Vector3 thisPosition = iRadarGraphs[radarIndex].BaseNodes[currentIndex].GetPosition(percentage);
            Handles.color = Color.grey;
            Handles.DrawLine(lastPosition, thisPosition);
        }
    }

    void DrawActualValueLine(int radarIndex, Vector2 start, Vector2 end) {
        Handles.color = iRadarGraphs[radarIndex].Color;
        Handles.DrawLine(start, end);        
    }

    void TrySetRadarGraph() {
        iRadarGraphs.Clear();
        for (int i = 0; i < ActiveObjects.Length; i++) {
            IRadarGraph graph = ActiveObjects[i] as IRadarGraph;
            if (graph!=null) {
                iRadarGraphs.Add(graph);
            }
        }
    }
    #endregion
}

public struct RectBounds {
    public Vector2 TopLeft { get { return new Vector2(rect.xMin, rect.yMin); } }
    public Vector2 BottomLeft { get { return new Vector2(rect.xMin, rect.yMax); } }
    public Vector2 TopRight { get { return new Vector2(rect.xMax, rect.yMin); } }
    public Vector2 BottomRight { get { return new Vector2(rect.xMax, rect.yMax); } }
    public Rect rect;
    public RectBounds(Rect rect) {
        this.rect = rect;
    }
}

public static class Ext {
    public static RectBounds Bounds(this Rect rect) {
        return new RectBounds(rect);
    }

    public static Rect AddPosition(this Rect rect, Vector2 position) {
        rect.x += position.x;
        rect.y += position.y;
        return rect;
    }
    public static Rect AddSize(this Rect rect, Vector2 size) {
        rect.width += size.x;
        rect.height += size.y;
        return rect;
    }
    public static Rect AddPosition(this Rect rect, Rect position) {
        rect.x += position.x;
        rect.y += position.y;
        return rect;
    }
    public static Rect AddSize(this Rect rect, Rect size) {
        rect.width += size.width;
        rect.height += size.height;
        return rect;
    }
    public static Vector2 ConvertAngleToVector2(this float inputAngle) {
        return new Vector2(Mathf.Cos(Mathf.Deg2Rad * inputAngle), Mathf.Sin(Mathf.Deg2Rad * inputAngle));
    }
}