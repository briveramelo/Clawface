using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Stats : MonoBehaviour, IModifiable {
    #region Serialized Unity Inspector Fields
    public float attack, defense, health, moveSpeed, rangedAccuracy, shotSpeed, shotPushForce, skinnableHealth;
    #endregion

    #region Private Fields
    private StatsMemento originalStats;
    #endregion

    #region Unity LifeCycle
    void Awake() {
        originalStats = new StatsMemento(attack, defense, health, moveSpeed, rangedAccuracy, shotSpeed, shotPushForce, skinnableHealth);
    }
    #endregion

    #region Public Methods
    public void Modify(StatType statType, float statMultiplier) {
        switch (statType) {
            case StatType.Attack:
                attack*=statMultiplier;
                break;
            case StatType.Defense:
                defense *= statMultiplier;
                break;
            case StatType.Health:
                health *= statMultiplier;
                break;
            case StatType.MoveSpeed:
                moveSpeed *= statMultiplier;
                break;
            case StatType.RangedAccuracy:
                rangedAccuracy *= statMultiplier;
                break;
        }
    }

    public void Modify(StatType statType, int statAddend) {
        switch (statType) {
            case StatType.Attack:
                attack += statAddend;
                break;
            case StatType.Defense:
                defense += statAddend;
                break;
            case StatType.Health:
                health += statAddend;
                if (health>originalStats.health){
                    health=originalStats.health;
                }
                break;
            case StatType.MoveSpeed:
                moveSpeed += statAddend;
                break;
            case StatType.RangedAccuracy:
                rangedAccuracy += statAddend;
                break;
        }
    }

    public float GetStat(StatType statType) {
        switch (statType) {
            case StatType.Attack:
                return attack;
            case StatType.Defense:
                return defense;
            case StatType.Health:
                return health;
            case StatType.MoveSpeed:
                return moveSpeed;
            case StatType.RangedAccuracy:
                return rangedAccuracy;
        }
        return -1;
    }

    public float GetHealthFraction(){ 
        return health/originalStats.health;    
    }

    public float TakeDamage(float damage) {
        health-= damage;
        if (health < 0) {
            health = 0;
        }        
        return health;
    }

    public void ResetForRebirth() {
        attack = originalStats.attack;
        defense = originalStats.defense;
        health = originalStats.health;
        moveSpeed = originalStats.moveSpeed;
        rangedAccuracy = originalStats.rangedAccuracy;
    }
    #endregion

    #region Internal Structures
    [Serializable]
    struct StatsMemento{
        public float attack, defense, health, moveSpeed, rangedAccuracy, shotSpeed, shotPushForce, skinnableHealth;
        public StatsMemento(float attack, float defense, float health, float moveSpeed, float rangedAccuracy, float shotSpeed, float shotPushForce, float skinnableHealth) {
            this.attack = attack;
            this.defense = defense;
            this.health = health;
            this.moveSpeed = moveSpeed;
            this.rangedAccuracy = rangedAccuracy;
            this.shotSpeed = shotSpeed;
            this.shotPushForce = shotPushForce;
            this.skinnableHealth=skinnableHealth;
        }
    }    
    #endregion
}
