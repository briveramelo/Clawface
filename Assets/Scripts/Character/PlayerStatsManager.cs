using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Turing.VFX;

public class PlayerStatsManager : MonoBehaviour, IDamageable
{


    #region Public fields
    //[HideInInspector]
    public float damageModifier = 1.0F;
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField] private OnScreenScoreUI playerUI;
    [SerializeField] private CameraLock cameraLock;
    [SerializeField] private PlayerFaceController faceController;
    [SerializeField] private bool shake;
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
        startHealth = stats.GetStat(CharacterStatType.MaxHealth);
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
            playerUI.DoDamageEffect();
            stats.TakeDamage(damageModifier * damager.damage);
            float healthFraction = stats.GetHealthFraction();
            playerUI.SetHealth(healthFraction);
            cameraLock.Shake();
            float shakeIntensity = 1f - healthFraction;
            if (shake) {
                InputManager.Instance.Vibrate(VibrationTargets.BOTH, shakeIntensity);
            }
            SFXManager.Instance.Play(SFXType.PlayerTakeDamage, transform.position);

            faceController.SetTemporaryEmotion (PlayerFaceController.Emotion.Angry, 0.5f);

            if (stats.GetStat(CharacterStatType.Health) <= 0)
            {
                Revive();
            }
        }
    }

    public void UpdateMaxHealth()
    {
        float healthFraction = stats.GetHealthFraction();
        playerUI.SetHealth(healthFraction);
    }

    public void TakeSkin(int skinHealth) {
        stats.Add(CharacterStatType.Health, skinHealth);
        healthAtLastSkin = stats.health;
        lastSkinHealthBoost=skinHealth;
    }

    public float GetStat(CharacterStatType type)
    {
        return stats.GetStat(type);
    }

    public bool ModifyStat(CharacterStatType type, float multiplier)
    {
        stats.Multiply(type, multiplier);
        return true;
    }

    public float GetHealth()
    {
        return stats.GetStat(CharacterStatType.Health);
    }
    #endregion

    #region Private Methods
    private void Revive() {
        SFXManager.Instance.Play(SFXType.PlayerDeath, transform.position);
        transform.position = GameObject.Find("RespawnPoint").transform.position;
        stats.Add(CharacterStatType.Health, (int)startHealth);
        startHealth = stats.GetStat(CharacterStatType.MaxHealth);
        playerUI.SetHealth(stats.GetHealthFraction());
        AnalyticsManager.Instance.PlayerDeath();
    }
    #endregion

    #region Private Structures
    #endregion

}
