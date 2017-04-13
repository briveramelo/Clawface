using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceMod : Mod {

    #region Serialized Fields
    [SerializeField]
    private VFXBlasterShoot blasterEffect;
    [SerializeField]
    private Transform bulletSpawnPoint;
    #endregion

    private ShooterProperties shooterProperties = new ShooterProperties();

    #region Unity Lifetime
    // Use this for initialization
    void Start () {
        type = ModType.Dice;
        category = ModCategory.Ranged;
	}

    #endregion

    #region Public Methods
    public override void Activate(Action onComplete = null)
    {
        base.Activate(onComplete);
    }

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable)
    {
        base.AttachAffect(ref wielderStats, wielderMovable);
    }

    public override void DeActivate()
    {
    }

    protected override void ActivateChargedArms()
    {
        blasterEffect.Emit();
        SpawnDice(bulletSpawnPoint.forward);
        SpawnDice(Quaternion.Euler(0, -30, 0) * bulletSpawnPoint.forward);
        SpawnDice(Quaternion.Euler(0, 30, 0) * bulletSpawnPoint.forward);
    }

    protected override void ActivateChargedLegs()
    {
    }

    protected override void ActivateStandardArms()
    {
        blasterEffect.Emit();
        SpawnDice(bulletSpawnPoint.forward);
    }

    protected override void ActivateStandardLegs()
    {
    }

    protected override void BeginChargingArms()
    {
    }

    protected override void BeginChargingLegs()
    {
    }

    protected override void RunChargingArms()
    {
    }

    protected override void RunChargingLegs()
    {
    }

    #endregion

    #region Private Methods
    private DiceBlock SpawnDice(Vector3 direction)
    {
        DiceBlock diceBlock = ObjectPool.Instance.GetObject(PoolObjectType.DiceBlock).GetComponent<DiceBlock>();
        if (diceBlock)
        {
            diceBlock.transform.position = bulletSpawnPoint.position;
            diceBlock.transform.rotation = transform.rotation;
            shooterProperties.Initialize(GetWielderInstanceID(), Attack, wielderStats.shotSpeed, wielderStats.shotPushForce);
            diceBlock.SetShooterProperties(shooterProperties);
            diceBlock.Roll(direction);
        }
        return diceBlock;
    }

    #endregion

}
