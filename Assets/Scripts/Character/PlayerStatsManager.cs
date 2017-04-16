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
    #endregion

    #region Private Fields
    [SerializeField] private Stats stats;
    float startHealth;
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    void Start () {
        startHealth = stats.GetStat(StatType.Health);
        AnalyticsManager.Instance.SetPlayerStats(this.stats);
}
	
	// Update is called once per frame
	void Update () {
		
	}
    #endregion

    #region Public Methods
    public void TakeDamage(Damager damager)
    {
        damageUI.DoDamageEffect();
        stats.TakeDamage(damageModifier * damager.damage);
        float healthFraction = stats.GetHealthFraction();
        HealthBar.Instance.SetHealth(healthFraction);
        cameraLock.Shake(.4f);
        float shakeIntensity = 1f-healthFraction;
        InputManager.Instance.Vibrate(VibrationTargets.BOTH, shakeIntensity);
        if (stats.GetStat(StatType.Health) <= 0){   
            Revive();
        }
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
        startHealth = stats.GetStat(StatType.Health);
        HealthBar.Instance.SetHealth(stats.GetHealthFraction());
        AnalyticsManager.Instance.PlayerDeath();
    }
    #endregion

    #region Private Structures
    #endregion

}
