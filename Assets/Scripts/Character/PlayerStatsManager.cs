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
    [SerializeField]
    private DamageUI damageUI;
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
    public void TakeDamage(float damage)
    {
        damageUI.DoDamageEffect();
        stats.TakeDamage(damageModifier * damage);
        HealthBar.Instance.SetHealth(stats.GetStat(StatType.Health) / startHealth);
        if (stats.GetStat(StatType.Health) <= 0)
        {   
            transform.position = GameObject.Find("RespawnPoint").transform.position;
            stats.Modify(StatType.Health, (int)startHealth);
            startHealth = stats.GetStat(StatType.Health);
            HealthBar.Instance.SetHealth(stats.GetStat(StatType.Health) / startHealth);
            AnalyticsManager.Instance.PlayerDeath();
        }
    }

    public float GetStat(StatType type)
    {
        return stats.GetStat(type);
    }

    public bool ModifyStat(StatType type, float multiplier)
    {
        stats.Modify(type, multiplier);
        return true;
    }
    #endregion

    #region Private Methods
    #endregion

    #region Private Structures
    #endregion

}
