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


    public override void DeActivate()
    {
    }

    protected override void ActivateChargedArms()
    {
    }

    protected override void ActivateChargedLegs()
    {
    }

    protected override void ActivateStandardArms()
    {
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
    private DiceBlock SpawnDice()
    {
        DiceBlock diceBlock = ObjectPool.Instance.GetObject(PoolObjectType.DiceBlock).GetComponent<DiceBlock>();
        if (diceBlock)
        {
            diceBlock.transform.position = bulletSpawnPoint.position;
            diceBlock.transform.rotation = transform.rotation;
            diceBlock.Roll(this.gameObject.transform.position + bulletSpawnPoint.position);
        }
        return diceBlock;
    }

    #endregion

}
