using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour, IModifiable {

	[SerializeField] CharacterStatsScriptable OG_Stats;
    CharacterStatsScriptable stats;    

    public float Health { get { return stats.radarGraph.health.RawValue; } set { stats.radarGraph.health.RawValue = value; } }
    public float MoveSpeed { get { return stats.radarGraph.moveSpeed.RawValue; } set { stats.radarGraph.moveSpeed.RawValue = value; } }
    public float Exp { get { return stats.radarGraph.exp.RawValue; } set { stats.radarGraph.exp.RawValue = value; } }

    public float MaxHealth { get { return OG_Stats.radarGraph.health.Value; } }
    
    public float SkinnableHealth { get { return stats.skinnableHealth; } }

    void Awake() {
        ResetForRebirth();        
    }

    public void ResetForRebirth() {
        stats = Instantiate(OG_Stats);
    }

    public void Multiply(CharacterStatType statType, float statMultiplier) {
        
    }

    public void Add(CharacterStatType statType, int statAddend) {
        
    }
}
