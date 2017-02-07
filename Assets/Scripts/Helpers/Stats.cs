using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Stats : MonoBehaviour, IModifiable {
    
    class VariableReference {
        public Func<object> Get { get; private set; }
        public Action<object> Set { get; private set; }
        public VariableReference(Func<object> getter, Action<object> setter){
            Get = getter;
            Set = setter;
        }
    }

    [SerializeField] StatsObject statsObject;
    Dictionary<StatType, VariableReference> statDictionary;

    void Awake() {
        StatsObject tempStatsObject = ScriptableObject.CreateInstance<StatsObject>();
        tempStatsObject.SetValues(statsObject);
        statsObject = tempStatsObject;

        statDictionary = new Dictionary<StatType, VariableReference>() {
            {StatType.Attack, new VariableReference(()=>statsObject.attack, statValue=> {statsObject.attack = (float)statValue; }) },
            {StatType.Defense, new VariableReference(()=>statsObject.defense, statValue=> {statsObject.defense = (float)statValue; }) },
            {StatType.Health, new VariableReference(()=>statsObject.health, statValue=> {statsObject.health = (float)statValue; }) },
            {StatType.MiniMapRange, new VariableReference(()=>statsObject.miniMapRange, statValue=> {statsObject.miniMapRange = (float)statValue; }) },
            {StatType.MoveSpeed, new VariableReference(()=>statsObject.moveSpeed, statValue=> {statsObject.moveSpeed = (float)statValue; }) },
            {StatType.RangedAccuracy, new VariableReference(()=>statsObject.rangedAccuracy, statValue=> {statsObject.rangedAccuracy = (float)statValue; }) },
        };
    }


    public void Modify(StatType statType, float statMultiplier) {
        statDictionary[statType].Set((float)statDictionary[statType].Get() * statMultiplier);
    }

    public void Modify(StatType statType, int statAddend) {
        statDictionary[statType].Set((float)statDictionary[statType].Get() + statAddend);
    }

    public float GetStat(StatType statType) {
        return (float)statDictionary[statType].Get();
    }

    public float TakeDamage(float damage){
        statDictionary[StatType.Health].Set((float)statDictionary[StatType.Health].Get() - damage);
        return (float)statDictionary[StatType.Health].Get();
    }
}
