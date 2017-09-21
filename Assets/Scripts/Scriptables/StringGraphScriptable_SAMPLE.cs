using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Custom Strings SAMPLE", menuName = "ScriptableObjects/Radar Graph/Samples/Custom Strings")]
public class StringGraphScriptable_SAMPLE : GraphScriptable<StringNode_SAMPLE>, IRadarGraph, IRadarGraph<StringNode_SAMPLE> {

    public StringGraph_SAMPLE radarGraph;
    public List<StringNode_SAMPLE> CustomNodes { get { return RadarGraph.CustomNodes; } }
    protected override RadarGraph<StringNode_SAMPLE> RadarGraph { get { return radarGraph; } }
}