using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class StatsObject : ScriptableObject
{
    public float attack, defense, health, moveSpeed, miniMapRange, rangedAccuracy;

    public StatsObject(StatsObject oldStats) {
        attack = oldStats.attack;
        defense = oldStats.defense;
        health = oldStats.health;
        moveSpeed = oldStats.moveSpeed;
        miniMapRange = oldStats.miniMapRange;
        rangedAccuracy = oldStats.rangedAccuracy;
    }
}
