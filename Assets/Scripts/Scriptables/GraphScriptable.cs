using System.Collections.Generic;
using UnityEngine;

public abstract class GraphScriptable<T> : ScriptableObject, IRadarGraph where T : RadarNode{

    protected abstract RadarGraph<T> RadarGraph { get; }


    public RadarType RadarType { get { return RadarGraph.RadarType; } }
    public void InitializeRawValues() { RadarGraph.InitializeRawValues(); }
    public void ResetNodes() { RadarGraph.ResetNodes(); }
    public bool ShouldReset() { if (RadarGraph != null) return RadarGraph.ShouldReset(); return false; }
    public Color Color { get { return color; } set { color = value; } }

    public List<RadarNode> BaseNodes {
        get {
            baseNodes.Clear();
            RadarGraph.CustomNodes.ForEach(node => { baseNodes.Add(node); });
            return baseNodes;
        }
        set {
            baseNodes = value;
        }
    }
    public Color color=Color.red;
    List<RadarNode> baseNodes = new List<RadarNode>();
}
