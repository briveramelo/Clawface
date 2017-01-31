using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class StatsObject : ScriptableObject
{
    public float attack, defense, health, moveSpeed, miniMapRange, rangedAccuracy;

    public void Modify(StatType statType, float mod) {
        switch (statType) {
            case StatType.Attack:
                attack *= mod;
                break;
            case StatType.Defense:
                defense *= mod;
                break;
            case StatType.Health:
                health *= mod;
                break;
            case StatType.MiniMapRange:
                moveSpeed *= mod;
                break;
            case StatType.MoveSpeed:
                miniMapRange *= mod;
                break;
            case StatType.RangedAccuracy:
                rangedAccuracy *= mod;
                break;
        }
    }
}
