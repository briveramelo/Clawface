using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;
using MovementEffects;

public class DiceMod : Mod {

    #region Serialized Fields
    [SerializeField]
    private VFXBlasterShoot blasterEffect;
    [SerializeField]
    private Transform bulletSpawnPoint;

    [SerializeField]
    private float armTimeTilExplosion;
    #endregion

    private ShooterProperties shooterProperties = new ShooterProperties();

    #region Unity Lifetime
    // Use this for initialization
    protected override void Awake () {
        type = ModType.Dice;
        category = ModCategory.Ranged;
        base.Awake();
	}

    #endregion

    protected override void Update () {
        if (wielderMovable != null){
            transform.forward = wielderMovable.GetForward();
        }
        base.Update();
    }

    #region Public Methods
    public override void Activate(Action onCompleteCoolDown=null, Action onActivate=null)
    {
        onActivate = ()=> { SFXManager.Instance.Play(SFXType.DiceLauncher_Shoot, transform.position);};
        base.Activate(onCompleteCoolDown, onActivate);
    }

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable)
    {
        base.AttachAffect(ref wielderStats, wielderMovable);
    }

    public override void DeActivate()
    {
    }

    protected override void ActivateStandardArms()
    {
        blasterEffect.Emit();
        DiceBlock diceBlock = SpawnDiceAndRoll(bulletSpawnPoint.forward);
        diceBlock.PrimeExplosion(armTimeTilExplosion);
    }
    #endregion

    #region Private Methods
    private DiceBlock SpawnDiceAndRoll(Vector3 direction)
    {
        DiceBlock diceBlock = ObjectPool.Instance.GetObject(PoolObjectType.DiceBlock).GetComponent<DiceBlock>();
        if (diceBlock)
        {
            diceBlock.transform.position = bulletSpawnPoint.position;
            diceBlock.transform.rotation = transform.rotation;
            shooterProperties.Initialize(GetWielderInstanceID(), Attack, wielderStats.shotSpeed, wielderStats.shotPushForce);
            diceBlock.SetShooterProperties(shooterProperties);
            diceBlock.Roll(direction);

            if (wielderStats.gameObject.CompareTag(Strings.Tags.PLAYER))
            {
                diceBlock.SetShooterType(true);
            }
            else
            {
                diceBlock.SetShooterType(false);
            }
        }
        return diceBlock;
    }
    #endregion

}
