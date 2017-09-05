using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;
using MovementEffects;
using Turing.VFX;

public class DiceMod : Mod {

    #region Serialized Fields
    [SerializeField]
    private VFXOneOff blasterEffect;
    [SerializeField]
    private Transform bulletSpawnPoint;

    [SerializeField]
    private float legPlayerJumpForce;

    [SerializeField]
    private float armTimeTilExplosion;

    [SerializeField]
    private float legTimeTilExplosion;
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

    protected override void ActivateChargedArms()
    {
        blasterEffect.Play();
        DiceBlock diceBlock = SpawnDiceAndRoll(bulletSpawnPoint.forward);
        diceBlock.PrimeExplosion(armTimeTilExplosion);

        diceBlock = SpawnDiceAndRoll(Quaternion.Euler(0, -30, 0) * bulletSpawnPoint.forward);
        diceBlock.PrimeExplosion(armTimeTilExplosion);

        diceBlock = SpawnDiceAndRoll(Quaternion.Euler(0, 30, 0) * bulletSpawnPoint.forward);
        diceBlock.PrimeExplosion(armTimeTilExplosion);
    }

    protected override void ActivateChargedLegs()
    {
        blasterEffect.Play();
        wielderMovable.AddDecayingForce(Vector3.up * legPlayerJumpForce);
        DiceBlock diceBlock1 = SpawnDiceAtPosition(bulletSpawnPoint.position, Vector3.zero);
        diceBlock1.PrimeExplosion(legTimeTilExplosion);
        Timing.CallDelayed(0.05f, SpawnDiceAtLegsAndPrime);
        Timing.CallDelayed(0.1f, SpawnDiceAtLegsAndPrime);
        
    }

    protected override void ActivateStandardArms()
    {
        blasterEffect.Play();
        DiceBlock diceBlock = SpawnDiceAndRoll(bulletSpawnPoint.forward);
        diceBlock.PrimeExplosion(armTimeTilExplosion);
    }

    protected override void ActivateStandardLegs()
    {
        blasterEffect.Play();
        wielderMovable.AddDecayingForce(Vector3.up * legPlayerJumpForce);
        DiceBlock diceBlock = SpawnDiceAtPosition(bulletSpawnPoint.position, Vector3.zero);
        diceBlock.PrimeExplosion(legTimeTilExplosion);
        
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

    private void SpawnDiceAtLegsAndPrime()
    {
        DiceBlock diceBlock = SpawnDiceAtPosition(bulletSpawnPoint.position, Vector3.zero);
        diceBlock.PrimeExplosion(legTimeTilExplosion);
    }

    private DiceBlock SpawnDiceAtPosition(Vector3 position, Vector3 rotation)
    {
        DiceBlock diceBlock = ObjectPool.Instance.GetObject(PoolObjectType.DiceBlock).GetComponent<DiceBlock>();
        if (diceBlock)
        {
            diceBlock.transform.position = position;
            diceBlock.transform.rotation = Quaternion.Euler(rotation);
            shooterProperties.Initialize(GetWielderInstanceID(), Attack, wielderStats.shotSpeed, wielderStats.shotPushForce);
            diceBlock.SetShooterProperties(shooterProperties);

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
