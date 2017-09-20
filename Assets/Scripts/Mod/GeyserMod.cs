using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;
using MovementEffects;

public class GeyserMod : Mod {

    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField] private float geyserStartDistanceOffset;

    [SerializeField] private float fissureSpeed;

    [SerializeField] private SFXType shootSFX;

    #endregion

    #region Private Fields
    private ShooterProperties shooterProperties = new ShooterProperties();
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    protected override void Awake() {
        type = ModType.Geyser;
        category = ModCategory.Ranged;
        base.Awake();
    }

    protected override void Update()
    {
        if (wielderMovable != null)
        {
            transform.forward = wielderMovable.GetForward();
        }
        base.Update();
    }

    #endregion

    #region Public Methods
    public override void Activate(Action onCompleteCoolDown=null, Action onActivate=null){
        base.Activate(onCompleteCoolDown, onActivate);
    }

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable)
    {
        base.AttachAffect(ref wielderStats, wielderMovable);
        
    }

    public override void DeActivate() { }

    public override void DetachAffect()
    {
        base.DetachAffect();
    }
    
    #endregion


    #region Private Methods    
    protected override void ActivateStandardArms() { Erupt(); }

    private void Erupt()
    {
        
        if (GetGeyser())
        {
            SFXManager.Instance.Play(shootSFX, transform.position);
            // FinishFiring();
        }
    }

    private GameObject GetGeyser()
    {
        GameObject projectile = ObjectPool.Instance.GetObject(PoolObjectType.GeyserFissure);
        if (projectile)
        {
            shooterProperties.Initialize(GetWielderInstanceID(), Attack, fissureSpeed, 0f);

            projectile.transform.position = transform.position;
            projectile.transform.forward = transform.forward;
            projectile.transform.rotation = Quaternion.Euler(0f, projectile.transform.rotation.eulerAngles.y, 0f);

            projectile.GetComponent<GeyserFissure>().SetShooterProperties(shooterProperties);

        }
        return projectile;
    }

    #endregion

    #region Private Structures
    #endregion

}
