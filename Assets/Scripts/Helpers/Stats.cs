using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Stats : MonoBehaviour {
    
    [SerializeField] StatsObject statsObject;
    Dictionary<StatType, float> statDictionary;

    void Awake() {        
        statDictionary = new Dictionary<StatType, float>() {
            {StatType.Attack, statsObject.attack },
            {StatType.Defense, statsObject.defense },
            {StatType.Health, statsObject.health },
            {StatType.MoveSpeed, statsObject.moveSpeed },
            {StatType.MiniMapRange, statsObject.miniMapRange },
            {StatType.RangedAccuracy, statsObject.rangedAccuracy }
        };
    }

    public void Modify(StatType statType, float statMultiplier) {
        Debug.Log("Before " + statsObject.attack);
        statDictionary[statType] *= statMultiplier;
        Debug.Log("After " + statsObject.attack);
    }

    public void Modify(StatType statType, int statAddend) {
        statDictionary[statType] += statAddend;
    }

    float GetStat(StatType statType) {
        float returnFloat = statDictionary[statType];
        return returnFloat;
    }
}
