using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour, IDamageable
{


    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    #endregion

    #region Private Fields
    private Stats stats;
    float startHealth;
    #endregion

    #region Unity Lifecycle
    void Awake()
    {
        stats = GetComponent<Stats>();
    }

    // Use this for initialization
    void Start () {
        startHealth = stats.GetStat(StatType.Health);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    #endregion

    #region Public Methods
    public void TakeDamage(float damage)
    {
        stats.TakeDamage(damage);
        HealthBar.Instance.SetHealth(stats.GetStat(StatType.Health) / startHealth);
        if (stats.GetStat(StatType.Health) <= 0)
        {   
            transform.position = GameObject.Find("RespawnPoint").transform.position;
            stats.Modify(StatType.Health, (int)startHealth);
            startHealth = stats.GetStat(StatType.Health);
            HealthBar.Instance.SetHealth(stats.GetStat(StatType.Health) / startHealth);
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
