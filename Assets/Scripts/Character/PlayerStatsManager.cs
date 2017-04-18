using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour, IDamageable
{


    #region Public fields
    [HideInInspector] public float damageModifier = 1.0F;
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField] private DamageUI damageUI;
    [SerializeField] private CameraLock cameraLock;
    [SerializeField] private SkinningState skinningState;
    #endregion

    #region Private Fields
    [SerializeField] private Stats stats;
    float startHealth;
    float healthAtLastSkin;
    float lastSkinHealthBoost;
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    void Start()
    {
        stats.SetMaxHealth(UpgradeManager.Instance.GetHealthLevel());
        startHealth = stats.GetStat(StatType.MaxHealth);
        AnalyticsManager.Instance.SetPlayerStats(this.stats);
        UpgradeManager.Instance.SetPlayerStats(this.stats);
        UpgradeManager.Instance.SetPlayerStatsManager(this);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    #endregion

    #region Public Methods
    public void TakeDamage(Damager damager)
    {
        if (damageModifier > 0.0f)
        {
            damageUI.DoDamageEffect();
            stats.TakeDamage(damageModifier * damager.damage);
            float healthFraction = stats.GetHealthFraction();
            HealthBar.Instance.SetHealth(healthFraction);
            cameraLock.Shake(.4f);
            float shakeIntensity = 1f - healthFraction;
            InputManager.Instance.Vibrate(VibrationTargets.BOTH, shakeIntensity);

            if (stats.health < healthAtLastSkin-lastSkinHealthBoost) {
                skinningState.RemoveSkin();
            }

            if (stats.GetStat(StatType.Health) <= 0)
            {
                Revive();
            }
        }
    }

    public void UpdateMaxHealth()
    {
        float healthFraction = stats.GetHealthFraction();
        HealthBar.Instance.SetHealth(healthFraction);
    }

    public void TakeSkin(int skinHealth) {
        stats.Add(StatType.Health, skinHealth);
        healthAtLastSkin = stats.health;
        lastSkinHealthBoost=skinHealth;
    }

    public float GetStat(StatType type)
    {
        return stats.GetStat(type);
    }

    public bool ModifyStat(StatType type, float multiplier)
    {
        stats.Multiply(type, multiplier);
        return true;
    }

    public float GetHealth()
    {
        return stats.GetStat(StatType.Health);
    }
    #endregion

    #region Private Methods
    private void Revive() {
        transform.position = GameObject.Find("RespawnPoint").transform.position;
        stats.Add(StatType.Health, (int)startHealth);
        startHealth = stats.GetStat(StatType.MaxHealth);
        HealthBar.Instance.SetHealth(stats.GetHealthFraction());
        AnalyticsManager.Instance.PlayerDeath();
    }
    #endregion

    #region Private Structures
    #endregion

}
