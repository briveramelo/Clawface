using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Stats", menuName = "ScriptableObjects/Radar Graph/Character Stats")]
public class CharacterStatsScriptable : GraphScriptable<CharacterStatsNode>, IRadarGraph, IRadarGraph<CharacterStatsNode> {

    public CharacterStatsGraph radarGraph;
    public float skinnableHealth;
    
    public List<CharacterStatsNode> CustomNodes { get { return RadarGraph.CustomNodes; } }
    protected override RadarGraph<CharacterStatsNode> RadarGraph { get { return radarGraph; } }
}
