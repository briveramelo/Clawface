using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Stats : MonoBehaviour, IModifiable
{
    #region Serialized Unity Inspector Fields
    [SerializeField] private StatsObject statsObject;
    #endregion

    #region Private Fields
    private StatsObject originalStats;
    private Dictionary<StatType, VariableReference> statDictionary;
    #endregion

    #region Unity LifeCycle
    void Awake()
    {
        statDictionary = new Dictionary<StatType, VariableReference>() {
            {StatType.Attack, new VariableReference(()=>statsObject.attack, statValue=> {statsObject.attack = (float)statValue; }) },
            {StatType.Defense, new VariableReference(()=>statsObject.defense, statValue=> {statsObject.defense = (float)statValue; }) },
            {StatType.Health, new VariableReference(()=>statsObject.health, statValue=> {statsObject.health = (float)statValue; }) },
            {StatType.MiniMapRange, new VariableReference(()=>statsObject.miniMapRange, statValue=> {statsObject.miniMapRange = (float)statValue; }) },
            {StatType.MoveSpeed, new VariableReference(()=>statsObject.moveSpeed, statValue=> {statsObject.moveSpeed = (float)statValue; }) },
            {StatType.RangedAccuracy, new VariableReference(()=>statsObject.rangedAccuracy, statValue=> {statsObject.rangedAccuracy = (float)statValue; }) },
        };
        originalStats = new StatsObject(statsObject);
    }
    #endregion

    #region Public Methods
    public void Modify(StatType statType, float statMultiplier)
    {
        statDictionary[statType].Set((float)statDictionary[statType].Get() * statMultiplier);
    }

    public void Modify(StatType statType, int statAddend)
    {
        statDictionary[statType].Set((float)statDictionary[statType].Get() + statAddend);
    }

    public float GetStat(StatType statType)
    {
        return (float)statDictionary[statType].Get();
    }

    public float TakeDamage(float damage)
    {
        float newHealthValue = (float)statDictionary[StatType.Health].Get() - damage;
        if (newHealthValue < 0) {
            newHealthValue = 0;
        }
        statDictionary[StatType.Health].Set(newHealthValue);
        return (float)statDictionary[StatType.Health].Get();
    }

    public void ResetStats() {
        statsObject = originalStats;
    }
    #endregion

    #region Internal Structures
    class VariableReference
    {
        public Func<object> Get { get; private set; }
        public Action<object> Set { get; private set; }
        public VariableReference(Func<object> getter, Action<object> setter)
        {
            Get = getter;
            Set = setter;
        }
    }
    [Serializable]
    struct StatsObject {
        public float attack, defense, health, moveSpeed, miniMapRange, rangedAccuracy;
        public StatsObject(StatsObject other) {
            attack = other.attack;
            defense = other.defense;
            health = other.health;
            moveSpeed = other.moveSpeed;
            miniMapRange = other.miniMapRange;
            rangedAccuracy = other.rangedAccuracy;
        }
    }
    #endregion
}
