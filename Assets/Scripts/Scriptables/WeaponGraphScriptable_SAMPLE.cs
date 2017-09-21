using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Stats SAMPLE", menuName = "ScriptableObjects/Radar Graph/Samples/Weapon Stats")]
public class WeaponGraphScriptable_SAMPLE : GraphScriptable<WeaponStatNode_SAMPLE>, IRadarGraph, IRadarGraph<WeaponStatNode_SAMPLE> {

    public WeaponGraph_SAMPLE radarGraph;
    public int attackCost, spellCost;

    public List<WeaponStatNode_SAMPLE> CustomNodes { get { return RadarGraph.CustomNodes; } }
    protected override RadarGraph<WeaponStatNode_SAMPLE> RadarGraph { get { return radarGraph; } }
}
