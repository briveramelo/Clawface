﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Stats : MonoBehaviour, IModifiable {
    #region Serialized Unity Inspector Fields
    [HideInInspector]public float attack, defense, health, maxHealth, moveSpeed, rangedAccuracy, shotSpeed, shotPushForce, skinnableHealth, exp, invincibilityCoolDownTime;
    #endregion

    #region Private Fields
    private StatsMemento originalStats;
    private bool isInvincible = false;
    #endregion

    #region Public Methods

    public void SetStats()
    {
        originalStats = new StatsMemento(attack, defense, health, maxHealth, moveSpeed, rangedAccuracy, shotSpeed, shotPushForce, skinnableHealth, exp);
        if (maxHealth == 0) maxHealth = health;
    }

    public void Multiply(CharacterStatType statType, float statMultiplier) {
        switch (statType) {
            case CharacterStatType.Attack:
                attack*=statMultiplier;
                break;            
            case CharacterStatType.Health:
                health *= statMultiplier;
                break;
            case CharacterStatType.MoveSpeed:
                moveSpeed *= statMultiplier;
                break;            
        }
    }

    
    public void Add(CharacterStatType statType, int statAddend) {
        switch (statType) {
            case CharacterStatType.Attack:
                attack += statAddend;
                break;            
            case CharacterStatType.Health:
                health += statAddend;
                if (health>originalStats.health){
                    health=originalStats.health;
                }
                break;
            case CharacterStatType.MoveSpeed:
                moveSpeed += statAddend;
                break;            
        }
    }

    public float GetStat(CharacterStatType statType) {
        switch (statType) {
            case CharacterStatType.Attack:
                return attack;            
            case CharacterStatType.Health:
                return health;
            case CharacterStatType.MaxHealth:
                return maxHealth;
            case CharacterStatType.MoveSpeed:
                return moveSpeed;            
            case CharacterStatType.EXP:
                return exp;
        }
        return -1;
    }

    public float GetHealthFraction(){
        return health/maxHealth;    
    }

    public void SetMaxHealth(float newMax)
    {
        maxHealth = newMax;
        originalStats.maxHealth = newMax;
    }

    public void LockHealth()
    {
        health = 5.0f;
    }

    public float TakeDamage(float damage) {
        if (!isInvincible)
        {
            health -= damage;
            if (health < 0)
            {
                health = 0;
            }            
            if (invincibilityCoolDownTime > 0)
            {
                isInvincible = true;
                StartCoroutine(InvincibilityCoolDown());
            }
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

    #region Private functions
    private IEnumerator InvincibilityCoolDown()
    {
        yield return new WaitForSeconds(invincibilityCoolDownTime);
        isInvincible = false;
    }
    #endregion

    #region Internal Structures
    [Serializable]
    struct StatsMemento{
        public float attack, defense, health, maxHealth, moveSpeed, rangedAccuracy, shotSpeed, shotPushForce, skinnableHealth, exp;
        public StatsMemento(float attack, float defense, float health, float maxHealth, float moveSpeed, float rangedAccuracy, float shotSpeed, float shotPushForce, float skinnableHealth, float exp) {
            this.attack = attack;
            this.defense = defense;
            this.health = health;
            this.maxHealth = maxHealth;
            this.moveSpeed = moveSpeed;
            this.rangedAccuracy = rangedAccuracy;
            this.shotSpeed = shotSpeed;
            this.shotPushForce = shotPushForce;
            this.skinnableHealth=skinnableHealth;
            this.exp = exp;
        }
    }    
    #endregion
}
