using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Stats SAMPLE", menuName = "ScriptableObjects/Radar Graph/Samples/Character Stats SAMPLE")]
public class CharacterStatsScriptable_SAMPLE : GraphScriptable<CharacterStatsNode_SAMPLE>, IRadarGraph, IRadarGraph<CharacterStatsNode_SAMPLE> {
    
    public CharacterStatsGraph_SAMPLE radarGraph;
    public int coinValue;
    public List<CharacterStatsNode_SAMPLE> CustomNodes { get { return RadarGraph.CustomNodes; } }
    protected override RadarGraph<CharacterStatsNode_SAMPLE> RadarGraph { get { return radarGraph; } }
}
