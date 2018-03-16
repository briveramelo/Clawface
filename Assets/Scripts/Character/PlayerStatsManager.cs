﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Turing.VFX;

public class PlayerStatsManager : MonoBehaviour, IDamageable
{


    #region Public fields
    //[HideInInspector]
    public float damageModifier = 1.0F;
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField] private PlayerFaceController faceController;
    [SerializeField] private bool shake;
    [SerializeField] private GameObject playerMesh;
    [SerializeField] private GameObject modSockets;
    [SerializeField] private PoolObjectType deathVFX;
    [SerializeField] private SFXType deathSFX;
    [SerializeField] private List<HealthRatio> damageScaling = new List<HealthRatio>();
    [SerializeField] private Stats stats;

    [SerializeField]
    private bool playerTakesSetDamage;

    [SerializeField]
    private float setDamageToTake;

    [SerializeField]
    private DashState dashState;

    [SerializeField]
    private float invincibleTime;

    #endregion

    #region Private Fields
    float startHealth;
    float healthAtLastSkin;
    float lastSkinHealthBoost;
    HitFlasher hitFlasher;
    private float invincibleTimer;

    bool gotDashComboThisFrame;
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    void Start()
    {
        startHealth = stats.GetStat(CharacterStatType.MaxHealth);
        hitFlasher = GetComponentInChildren<HitFlasher>();
        stats.SetStats();
        EventSystem.Instance.TriggerEvent(Strings.Events.PLAYER_BIRTHED);
    }

    void Update()
    {
        invincibleTimer -= Time.deltaTime;
        gotDashComboThisFrame = false;
    }
	
    #endregion

    #region Public Methods
    public Vector3 GetPosition ()
    {
        return transform.position;
    }

    public void TakeDamage(Damager damager)
    {
        if(stats.health > 0)
        {
            if (dashState.CheckForIFrames())
            {
                if (dashState.CheckIfDashGivesCombo() && !gotDashComboThisFrame)
                {
                    gotDashComboThisFrame = true;
                    dashState.ResetDashComboTimer();
                    ScoreManager.Instance.AddToCombo();
                }
                return;
            }

            if (invincibleTimer >= 0f)
            {
                return;
            }

            


            float healthFraction = stats.GetHealthFraction();

            float damageToTake = setDamageToTake;
            switch (SettingsManager.Instance.Difficulty)
            {
                case Difficulty.EASY:
                    damageToTake *= 1.0f/6.0f;
                    break;
                case Difficulty.HARD:
                    damageToTake *= 3f;
                    break;
                case Difficulty.NORMAL:
                default:
                    break;
            }

            stats.TakeDamage(damageToTake);



            invincibleTimer = invincibleTime;
            EventSystem.Instance.TriggerEvent(Strings.Events.PLAYER_DAMAGED);
            healthFraction = stats.GetHealthFraction();
            EventSystem.Instance.TriggerEvent(Strings.Events.PLAYER_HEALTH_MODIFIED, healthFraction);

            float shakeIntensity = 1f - healthFraction;
            if (shake)
            {
                InputManager.Instance.Vibrate(VibrationTargets.BOTH, shakeIntensity);
            }
            SFXManager.Instance.Play(SFXType.PlayerTakeDamage, transform.position);

            faceController.SetTemporaryEmotion(PlayerFaceController.Emotion.Angry, 0.5f);

            hitFlasher.HitFlash();

            if (stats.GetStat(CharacterStatType.Health) <= 0)
            {
                EventSystem.Instance.TriggerEvent(Strings.Events.PLAYER_KILLED, SceneTracker.CurrentSceneName, AnalyticsManager.Instance.GetCurrentWave(), ModManager.leftArmOnLoad.ToString(), ModManager.rightArmOnLoad.ToString());
                //SFXManager.Instance.Play(SFXType.AnnounceDeath, Vector3.zero);
                //Revive(); //removed because of the inclusion of the game over menu
                if (playerMesh && modSockets)
                {
                    GameObject vfx = ObjectPool.Instance.GetObject(deathVFX);
                    if (vfx)
                    {
                        vfx.transform.position = playerMesh.transform.position;
                        vfx.SetActive(true);
                    }
                    CapsuleCollider collider = GetComponent<CapsuleCollider>();
                    if (collider)
                    {
                        collider.enabled = false;
                    }
                    playerMesh.SetActive(false);
                    modSockets.SetActive(false);
                    SFXManager.Instance.Play(deathSFX, playerMesh.transform.position);
                }
            }
        }
    }

    public void UpdateMaxHealth()
    {
        float healthFraction = stats.GetHealthFraction();

        EventSystem.Instance.TriggerEvent(Strings.Events.PLAYER_HEALTH_MODIFIED, healthFraction);
    }

    public void TakeHealth(int health) {
        stats.Add(CharacterStatType.Health, health);
        healthAtLastSkin = stats.health;
        lastSkinHealthBoost=health;
        EventSystem.Instance.TriggerEvent(Strings.Events.PLAYER_HEALTH_MODIFIED, stats.GetHealthFraction());
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

    public void MakeHappy()
    {
        faceController.SetTemporaryEmotion(PlayerFaceController.Emotion.Happy, 1.0f);
    }
    #endregion

    #region Private Methods
    private void Revive() {
        SFXManager.Instance.Play(SFXType.PlayerDeath, transform.position);
        transform.position = GameObject.Find(Strings.RESPAWN_POINT).transform.position;
        stats.Add(CharacterStatType.Health, (int)startHealth);
        startHealth = stats.GetStat(CharacterStatType.MaxHealth);

        EventSystem.Instance.TriggerEvent(Strings.Events.PLAYER_HEALTH_MODIFIED, stats.GetHealthFraction());

    }
    #endregion

    #region Private Structures

    [System.Serializable]
    public struct HealthRatio
    {
        public float healthPercentage;
        public float damageRatio;
    }
    #endregion

}
