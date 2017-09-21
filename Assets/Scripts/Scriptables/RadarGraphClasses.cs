using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public abstract class RadarGraph<T> : IRadarGraph where T : RadarNode {
    
    public abstract List<T> CustomNodes { get; set; }
    public abstract bool ShouldReset();
    public abstract void ResetNodes();
    public abstract RadarType RadarType { get; }
    public void InitializeRawValues() {
        ResetNodes();
        CustomNodes.ForEach(node => { node.rawValue = node.Value; });
    }
    public List<RadarNode> BaseNodes{
        get {
            baseNodes.Clear();
            CustomNodes.ForEach(node => { baseNodes.Add(node); });
            return baseNodes;
        }
        set {
            baseNodes = value;
        }
    }
    public Color Color { get { return color; } set { color = value; } }
    Color color;
    List<RadarNode> baseNodes=new List<RadarNode>();

}

public enum RadarType {
    String_SAMPLE,
    WeaponStats_SAMPLE,
    CharacterStats_SAMPLE,
    CharacterStats
}

[Serializable]
public class CharacterStatsNode : RadarNode {
    public CharacterStatType statType;
    public CharacterStatsNode(CharacterStatType statType) {
        this.statType = statType;
        this.name = statType.ToString();
    }
    public CharacterStatsNode() { }
}

[Serializable]
public class CharacterStatsGraph : RadarGraph<CharacterStatsNode> {

    [SerializeField, HideInInspector] List<CharacterStatsNode> nodes = new List<CharacterStatsNode>();

    public CharacterStatsNode exp = new CharacterStatsNode(CharacterStatType.EXP);
    public CharacterStatsNode health = new CharacterStatsNode(CharacterStatType.Health);
    public CharacterStatsNode moveSpeed = new CharacterStatsNode(CharacterStatType.MoveSpeed);

    public override List<CharacterStatsNode> CustomNodes {
        get {
            if (nodes.Count == 0) {
                ResetNodes();
            }
            return nodes;
        }
        set { nodes = value; }
    }
    public override bool ShouldReset() {
        if (exp.value != CustomNodes.Find(node => node.statType == CharacterStatType.EXP).value) return true;
        if (health.value != CustomNodes.Find(node => node.statType == CharacterStatType.Health).value) return true;
        if (moveSpeed.value != CustomNodes.Find(node => node.statType == CharacterStatType.MoveSpeed).value) return true;        
        return false;
    }
    public override void ResetNodes() {
        nodes.Clear();
        nodes.Add(exp);
        nodes.Add(health);        
        nodes.Add(moveSpeed);        
    }
    public override RadarType RadarType { get { return RadarType.CharacterStats; } }
}

[Serializable]
/// <summary>
/// Base class for all radar nodes
/// </summary>
public abstract class RadarNode {
    [HideInInspector]
    public string name;
    public FloatRange valueRange = new FloatRange(0, 1);
    public bool isRangeReversed;
    [Range(0, 1)]
    public float value = 0.5f;
    public float Value {
        get {
            if (isRangeReversed) return valueRange.Max - value * valueRange.Diff;
            return valueRange.Min + value * valueRange.Diff;
        }
        set {
            if (isRangeReversed) this.value = Mathf.Clamp01((valueRange.Max - value) / (valueRange.Diff));
            else this.value = Mathf.Clamp01((value - valueRange.Min) / (valueRange.Diff));
        }
    }
    [HideInInspector]
    public float rawValue;
    public float RawValue { get { return rawValue; } set { rawValue = value; } }
    [HideInInspector]
    public Vector3 Position { get { return center + direction * value; } }
    [HideInInspector]
    public Vector3 maxPosition;
    [HideInInspector]
    public Vector3 center;
    [HideInInspector]
    public Vector3 direction;
    public Vector3 GetPosition(float fraction) {
        fraction = Mathf.Clamp01(fraction);
        return center + direction * fraction;
    }
}
public interface IRadarGraph {
    List<RadarNode> BaseNodes { get; set; }
    Color Color { get; set; }
    void ResetNodes();
    bool ShouldReset();
    void InitializeRawValues();
    RadarType RadarType { get; }
}

public interface IRadarGraph<T> where T : RadarNode {
    List<T> CustomNodes { get; }
}

#region Samples

[Serializable]
public class StringGraph_SAMPLE : RadarGraph<StringNode_SAMPLE> {
    [SerializeField]
    List<StringNode_SAMPLE> nodes = new List<StringNode_SAMPLE>();
    public override List<StringNode_SAMPLE> CustomNodes {
        get {
            if (nodes.Count == 0) {
                ResetNodes();
            }
            return nodes;
        }
        set { nodes = value; }
    }
    public override bool ShouldReset() { return false; }
    public override void ResetNodes() {
        nodes.Clear();
    }
    public override RadarType RadarType { get { return RadarType.String_SAMPLE; } }
}

[Serializable]
public class StringNode_SAMPLE : RadarNode {
    public string Name;
    public StringNode_SAMPLE(string name) {
        this.Name = name;
        this.name = name;
    }

}

[Serializable]
public class WeaponGraph_SAMPLE : RadarGraph<WeaponStatNode_SAMPLE> {
    [SerializeField, HideInInspector] List<WeaponStatNode_SAMPLE> nodes = new List<WeaponStatNode_SAMPLE>();

    public WeaponStatNode_SAMPLE baseAttack = new WeaponStatNode_SAMPLE(WeaponStat_SAMPLE.BaseAttack);
    public WeaponStatNode_SAMPLE baseAttackExtra = new WeaponStatNode_SAMPLE(WeaponStat_SAMPLE.BaseAttackExtra);
    public WeaponStatNode_SAMPLE baseAttackExtraLikelihood = new WeaponStatNode_SAMPLE(WeaponStat_SAMPLE.BaseAttackExtraLikelihood);
    public WeaponStatNode_SAMPLE spellAttack = new WeaponStatNode_SAMPLE(WeaponStat_SAMPLE.SpellAttack);
    public WeaponStatNode_SAMPLE spellAttackExtra = new WeaponStatNode_SAMPLE(WeaponStat_SAMPLE.SpellAttackExtra);
    public WeaponStatNode_SAMPLE spellAttackExtraLikelihood = new WeaponStatNode_SAMPLE(WeaponStat_SAMPLE.SpellAttackExtraLikelihood);
    public WeaponStatNode_SAMPLE spellDuration = new WeaponStatNode_SAMPLE(WeaponStat_SAMPLE.SpellDuration);
    public WeaponStatNode_SAMPLE spellInterval = new WeaponStatNode_SAMPLE(WeaponStat_SAMPLE.SpellInterval);
    public WeaponStatNode_SAMPLE spellCooldown = new WeaponStatNode_SAMPLE(WeaponStat_SAMPLE.SpellCooldown);

    public override List<WeaponStatNode_SAMPLE> CustomNodes {
        get {
            if (nodes.Count == 0) {
                ResetNodes();
            }
            return nodes;
        }
        set { nodes = value; }
    }
    public override bool ShouldReset() {
        if (baseAttack.value != CustomNodes.Find(node => node.statType == WeaponStat_SAMPLE.BaseAttack).value) return true;
        if (baseAttackExtra.value != CustomNodes.Find(node => node.statType == WeaponStat_SAMPLE.BaseAttackExtra).value) return true;
        if (baseAttackExtraLikelihood.value != CustomNodes.Find(node => node.statType == WeaponStat_SAMPLE.BaseAttackExtraLikelihood).value) return true;
        if (spellAttack.value != CustomNodes.Find(node => node.statType == WeaponStat_SAMPLE.SpellAttack).value) return true;
        if (spellAttackExtra.value != CustomNodes.Find(node => node.statType == WeaponStat_SAMPLE.SpellAttackExtra).value) return true;
        if (spellAttackExtraLikelihood.value != CustomNodes.Find(node => node.statType == WeaponStat_SAMPLE.SpellAttackExtraLikelihood).value) return true;
        if (spellDuration.value != CustomNodes.Find(node => node.statType == WeaponStat_SAMPLE.SpellDuration).value) return true;
        if (spellInterval.value != CustomNodes.Find(node => node.statType == WeaponStat_SAMPLE.SpellInterval).value) return true;
        if (spellCooldown.value != CustomNodes.Find(node => node.statType == WeaponStat_SAMPLE.SpellCooldown).value) return true;        
        return false;
    }
    public override void ResetNodes() {
        nodes.Clear();
        nodes.Add(baseAttack);
        nodes.Add(baseAttackExtra);
        nodes.Add(baseAttackExtraLikelihood);
        nodes.Add(spellAttack);
        nodes.Add(spellAttackExtra);
        nodes.Add(spellAttackExtraLikelihood);
        nodes.Add(spellDuration);
        nodes.Add(spellInterval);
        nodes.Add(spellCooldown);        
    }
    public override RadarType RadarType { get { return RadarType.WeaponStats_SAMPLE; } }

}

[Serializable]
public class CharacterStatsGraph_SAMPLE : RadarGraph<CharacterStatsNode_SAMPLE> {
    [SerializeField, HideInInspector] List<CharacterStatsNode_SAMPLE> nodes = new List<CharacterStatsNode_SAMPLE>();
    public CharacterStatsNode_SAMPLE health = new CharacterStatsNode_SAMPLE(CharacterStatType_SAMPLE.Health);
    public CharacterStatsNode_SAMPLE speed = new CharacterStatsNode_SAMPLE(CharacterStatType_SAMPLE.Speed);
    public CharacterStatsNode_SAMPLE attack = new CharacterStatsNode_SAMPLE(CharacterStatType_SAMPLE.Attack);
    public CharacterStatsNode_SAMPLE defense = new CharacterStatsNode_SAMPLE(CharacterStatType_SAMPLE.Defense);
    public CharacterStatsNode_SAMPLE evasion = new CharacterStatsNode_SAMPLE(CharacterStatType_SAMPLE.Evasion);

    public override List<CharacterStatsNode_SAMPLE> CustomNodes {
        get {
            if (nodes.Count == 0) {
                ResetNodes();
            }
            return nodes;
        }
        set { nodes = value; }
    }
    public override bool ShouldReset() {
        if (health.value != CustomNodes.Find(node => node.statType == CharacterStatType_SAMPLE.Health).value) return true;
        if (speed.value != CustomNodes.Find(node => node.statType == CharacterStatType_SAMPLE.Speed).value) return true;
        if (attack.value != CustomNodes.Find(node => node.statType == CharacterStatType_SAMPLE.Attack).value) return true;
        if (defense.value != CustomNodes.Find(node => node.statType == CharacterStatType_SAMPLE.Defense).value) return true;
        if (evasion.value != CustomNodes.Find(node => node.statType == CharacterStatType_SAMPLE.Evasion).value) return true;
        return false;
    }
    public override void ResetNodes() {
        nodes.Clear();
        nodes.Add(health);
        nodes.Add(speed);
        nodes.Add(attack);
        nodes.Add(defense);
        nodes.Add(evasion);
    }
    public override RadarType RadarType { get { return RadarType.CharacterStats_SAMPLE; } }
}




[Serializable]
public class CharacterStatsNode_SAMPLE : RadarNode {
    public CharacterStatType_SAMPLE statType;
    public CharacterStatsNode_SAMPLE(CharacterStatType_SAMPLE statType) {
        this.statType = statType;
        this.name = statType.ToString();
    }
    public CharacterStatsNode_SAMPLE() { }
}

[Serializable]
public class WeaponStatNode_SAMPLE : RadarNode {
    public WeaponStat_SAMPLE statType;
    public WeaponStatNode_SAMPLE(WeaponStat_SAMPLE statType) {
        this.statType = statType;
        this.name = statType.ToString();
    }
    public WeaponStatNode_SAMPLE() { }
}

public enum WeaponStat_SAMPLE {
    BaseAttack,
    BaseAttackExtra,
    BaseAttackExtraLikelihood,
    SpellAttack,
    SpellAttackExtra,
    SpellAttackExtraLikelihood,
    SpellDuration,
    SpellInterval,
    SpellCooldown
}


public enum CharacterStatType_SAMPLE {
    Health,
    Speed,
    Attack,
    Defense,
    Evasion
}
#endregion
