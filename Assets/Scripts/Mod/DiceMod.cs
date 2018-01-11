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
    private float armTimeTilExplosion;
    #endregion

    private Animator animator;

    #region Unity Lifetime
    // Use this for initialization
    protected override void Awake () {
        type = ModType.Missile;
        category = ModCategory.Ranged;
        animator = GetComponentInChildren<Animator>();
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
        onActivate = ()=> { SFXManager.Instance.Play(shootSFX, transform.position);};
        base.Activate(onCompleteCoolDown, onActivate);
    }

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable)
    {
        base.AttachAffect(ref wielderStats, wielderMovable);
    }

    public override void DeActivate()
    {
    }


    protected override void DoWeaponActions()
    {
        blasterEffect.Play();
        DiceBlock diceBlock = SpawnDiceAndRoll(bulletSpawnPoint.forward);
        if (diceBlock) {
            diceBlock.PrimeExplosion(armTimeTilExplosion);
        }
    }

    #endregion

    #region Private Methods
    private DiceBlock SpawnDiceAndRoll(Vector3 direction)
    {
        GameObject go = ObjectPool.Instance.GetObject(PoolObjectType.DiceBlock);
        DiceBlock diceBlock = null;
        if (go) {
            diceBlock = go.GetComponent<DiceBlock>();
            if (diceBlock)
            {
                diceBlock.transform.position = bulletSpawnPoint.position;
                diceBlock.transform.rotation = transform.rotation;
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
        }
        animator.SetTrigger("Shoot");
        return diceBlock;
    }
    #endregion

}
